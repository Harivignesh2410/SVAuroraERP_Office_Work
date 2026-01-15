namespace SVAuroraERP.WebUI.Pages.Inventory.Master
{
    public class BoxModel : BasePageModel
    {
        private readonly IBoxServiceRepository _repository;
        private readonly ISizeServiceRepository _sizeServiceRepository;
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger<BoxModel> logger = null;
        private const int PageControlID = (int)Common.Pages.Box; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;

        public BoxModel(IBoxServiceRepository boxServiceRepository, 
                        ISizeServiceRepository sizeServiceRepository, 
                        IAntiforgery antiforgery,
                         IPermissionServiceRepository permissionService,
                          ILogger<BoxModel> _logger
                         )
        {
            _repository = boxServiceRepository;
            _sizeServiceRepository = sizeServiceRepository;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;  
            logger = _logger;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> SizeList { get; set; } = new List<SelectListItem>();
   
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadSizeList();

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadSizeList()
        {
            DataResponse dataResponse = null;
            SizeList.Clear();
            dataResponse = _sizeServiceRepository.GetSize();
            SizeList = ((List<VSize>)dataResponse.Value)
                .OrderBy(o => o.SizeName)
                .Select(s => new SelectListItem
                {
                    Value = s.SizeID.ToString(),
                    Text = s.SizeName
                }).ToList();

            SizeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Size--" });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] Box BoxData)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                BoxData.LastUpdatedBy = LoggedUser.UserID;

                if (BoxData.BoxID == 0)
                    resultdata = _repository.Save(BoxData);
                else if (BoxData.BoxID > 0)
                    resultdata = _repository.Update(BoxData);

                return new JsonResult(new{ result = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = resultdata });
            }
        }
        public JsonResult OnGetBoxList(int draw, int start, int length)
        {
            DataResponse dataResponse = new DataResponse();
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = _repository.GetBox();
            var resultdata = ((List<VBox>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.BoxName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.BoxName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VBox, object> orderByFunc = orderCol switch
            {
                1 => d => d.BoxName,
                2 => d => d.SizeName,
                3 => d => d.MaxCapacity,
                _ => null  // No sorting for other columns
            };

            if (orderByFunc != null)
            {
                filteredData = orderDir == "asc"
                    ? filteredData.OrderBy(orderByFunc).ToList()
                    : filteredData.OrderByDescending(orderByFunc).ToList();
            }

            // Paginate the filtered data
            var paginatedData = filteredData.Skip(start).Take(length).ToList();

            // Return the JSON result
            return new JsonResult(new
            {
                draw = draw,
                recordsTotal = resultdata.Count,
                recordsFiltered = filteredData.Count,
                data = paginatedData
            });
        }

        public JsonResult OnGetBoxByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();

            dataResponse = _repository.GetByID(ID);

            return new JsonResult(dataResponse.Value);
        }

        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                resultdata = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { result = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = resultdata });
            }
        }
        public JsonResult OnPostBoxtoDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Validate sort column
                var validColumns = new[] { "BoxName", "SizeName", "MaxCapacity", "InnerBoxCount", "InnerBoxQuantity"};
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "BoxName";

                dataResponse = _repository.GetBoxtoDataTable(dataTableRequest);

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = dataResponse.recordsTotal,
                    recordsFiltered = dataResponse.recordsFiltered,
                    data = dataResponse.Value
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading Box data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VBox>()
                });
            }
        }
    }
}