namespace SVAuroraERP.WebUI.Pages.HR
{
    public class DesignationModel : BasePageModel
    {
        private readonly IDesignationServiceRepository _respository = null;
        private readonly ILogger<Designation> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.Designation; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;
        public DesignationModel(IDesignationServiceRepository respository,
                           ILogger<Designation> logger,
                           IAntiforgery antiforgery, IPermissionServiceRepository permissionService,
                           SessionService sessionService)
        {
            _respository = respository;
            _logger = logger;
            _permissionrepository = permissionService;
            _antiforgery = antiforgery;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
        }
        public JsonResult OnPostSaveUpdateData([FromBody] Designation DesignationData)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                DesignationData.LastUpdatedBy = LoggedUser.UserID;
               DesignationData.LoginAuditID = LoggedUser.LoginAuditID;

                if (DesignationData.DesignationID == 0)
                    resultdata = _respository.Save(DesignationData);
                else if (DesignationData.DesignationID > 0)
                    resultdata = _respository.Update(DesignationData);

                return new JsonResult(new { resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata });
            }
        }
        public JsonResult OnGetDesignationList(int draw, int start, int length)
        {
            DataResponse dataResponse = new DataResponse();
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = _respository.GetDesignation();
            var resultdata = ((List<VDesignation>)dataResponse.Value).OrderBy(o => o.DesignationName).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => (d.Description ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();


            // Handle sorting based on the column index and direction
            Func<VDesignation, object> orderByFunc = orderCol switch
            {
                1 => d => d.DesignationName,
                2 => d => d.Description,
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
        public JsonResult OnGetDesignationByID(int ID)
        {
            var resultdata = _respository.GetByID(ID);

            return new JsonResult(new { resultdata });
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                resultdata = _respository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata });
            }
        }
        public JsonResult OnPostDesignationoDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Validate sort column
                var validColumns = new[] { "DesignationName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "DesignationName";

                dataResponse = _respository.GetDesignationDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Box data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VDesignation>()
                });
            }
        }
    }
}