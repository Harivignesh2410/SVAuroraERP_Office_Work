namespace SVAuroraERP.WebUI.Pages.Authentication
{
    public class RolesModel : BasePageModel
    {
        private readonly IRoleServiceRepository _repository = null;
        private readonly ILogger<RolesModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.Roles; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;

        public RolesModel(IRoleServiceRepository respository,
                           ILogger<RolesModel> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            CurrentPageControlID = PageControlID;
        }

        public JsonResult OnGetRolesList(int draw, int start, int length)
        {
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            var resultdata = (_repository.GetList()).OrderBy(o => o.RoleName).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.RoleName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.Description ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VRole, object> orderByFunc = orderCol switch
            {
                1 => d => d.RoleName,
                2 => d => d.Description,
                3 => d => d.ApplicationName,
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

        public JsonResult OnPostSaveUpdateData([FromBody] Role roleData)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                roleData.LastUpdatedBy = LoggedUser.UserID;
                roleData.LoginAuditID = LoggedUser.LoginAuditID;

                if (roleData.RoleID == 0)
                    resultdata = _repository.Save(roleData);
                else if (roleData.RoleID > 0)
                    resultdata = _repository.Update(roleData);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public JsonResult OnGetRoleByID(int ID)
        {
            var resultdata = _repository.GetByID(ID);

            return new JsonResult(new { success = true, data = resultdata });
        }

        public JsonResult OnGetRoleModule(int ID)
        {
            var resultdata = _repository.GetModuleListByApplicationID(ID).OrderBy(o => o.OrdinalNo);

            return new JsonResult(new { success = true, data = resultdata });
        }
    }
}