namespace SVAuroraERP.WebUI.Pages.Online.Master
{
    public class HSRPPartNumberModel : BasePageModel
    {
        private readonly IHSRPPartNumberServiceRepository _repository = null;
        private readonly IHSRPUserServiceRepository _staterepository = null;
        private readonly ILogger<HSRPPartNumber> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.HSRPPartNumber; // ID for this specific page
        public HSRPPartNumberModel(IHSRPPartNumberServiceRepository respository,
            IHSRPUserServiceRepository stateRepository,
                           ILogger<HSRPPartNumber> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                             IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _staterepository = stateRepository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;

        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> OEMList { get; set; } = new List<SelectListItem>();
   
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadOEMList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadOEMList()
        {
            DataResponse dataResponse = new DataResponse();
            OEMList.Clear();
            dataResponse = _staterepository.GetOEM();
            OEMList = ((List<VHSRPUser>)dataResponse.Value)
                .OrderBy(o => o.CompanyName)
                .Select(s => new SelectListItem
                {
                    Value = s.HSRPUserID.ToString(),
                    Text = s.CompanyName
                }).ToList();

            OEMList.Insert(0, new SelectListItem { Value = "0", Text = "--Select OEM --" });
        }
        public JsonResult OnGetHSRPPartNumberList(int draw, int start, int length)
        {
            DataResponse dataResponse = null;
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = (_repository.GetHSRPPartNumber());
            var resultdata = ((List<VHSRPPartNumber>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.PartNumber.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.OEMName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VHSRPPartNumber, object> orderByFunc = orderCol switch
            {
                1 => d => d.PartNumber,
                2 => d => d.OEMName,
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
        public JsonResult OnPostSaveUpdateData([FromBody] HSRPPartNumber HSRPPartNumber)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                HSRPPartNumber.LastUpdatedBy = LoggedUser.UserID;

                if (HSRPPartNumber.HSRPPartNumberID == 0)
                    resultdata = _repository.Save(HSRPPartNumber);
                else if (HSRPPartNumber.HSRPPartNumberID > 0)
                    resultdata = _repository.Update(HSRPPartNumber);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetHSRPPartNumberByID(int ID)
        {
            DataResponse resultdata = _repository.GetHSRPPartNumberByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();

            try
            {
                dataResponse = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnPostHSRPPartNumberList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "PartNumber" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "PartNumber";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetHSRPPartNumberDataTableList(dataTableRequest);

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
                _logger.LogError(ex, "Error loading State data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VState>()
                });
            }
        }
    }
}