namespace SVAuroraERP.WebUI.Pages.Inventory.Master
{
    public class SizeModel : BasePageModel
    {
        private readonly ISizeServiceRepository _repository = null;
        private readonly ILogger<Size> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.Size; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;
        public SizeModel(ISizeServiceRepository respository,
                           ILogger<Size> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
         
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public JsonResult OnPostSaveUpdateData([FromBody] Size SizeData)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                SizeData.LastUpdatedBy = LoggedUser.UserID;
                SizeData.LoginAuditID = LoggedUser.LoginAuditID;

                if (SizeData.SizeID == 0)
                    resultdata = _repository.Save(SizeData);
                else if (SizeData.SizeID > 0)
                    resultdata = _repository.Update(SizeData);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetSizeList(int draw, int start, int length)
        {
            DataResponse dataResponse = null;
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

           dataResponse = (_repository.GetSize());
            var resultdata=((List<VSize>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.SizeCode.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.SizeName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VSize, object> orderByFunc = orderCol switch
            {
                1 => d => d.SizeCode,
                2 => d => d.SizeName,
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
        public JsonResult OnGetSizeByID(int ID)
        {
            DataResponse resultdata = _repository.GetByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                resultdata = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnPostSizeDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Validate sort column
                var validColumns = new[] { "SizeName" ,"SizeCode" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "SizeName";

                dataResponse = _repository.GetSizeDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Size  data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VSize>()
                });
            }
        }
    }
}