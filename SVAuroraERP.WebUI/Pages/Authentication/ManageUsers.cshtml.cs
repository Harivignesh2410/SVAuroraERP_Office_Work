namespace SVAuroraERP.WebUI.Pages.Authentication
{
    public class ManageUsersModel : BasePageModel
    {
        private readonly ILogger<RolesModel> logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IRoleServiceRepository _repository;
        private readonly IAuthenticaionServiceRepository _authenrepository;
        private readonly IRoleConfigurationServiceRepository _configrepository;
        private readonly IUserServiceRepository _userservicerepository;
        private const int PageControlID = (int)Common.Pages.ManageUsers; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;

        public ManageUsersModel(
                           ILogger<RolesModel> _logger,
                           IAntiforgery antiforgery,
                           IRoleServiceRepository repository,
                           IAuthenticaionServiceRepository authenrepository,
                           IRoleConfigurationServiceRepository configrepository,
                           IUserServiceRepository userservicerepository,
                           IPermissionServiceRepository permissionService)
        {

            logger = _logger;
            _antiforgery = antiforgery;
            _repository = repository;
            _authenrepository = authenrepository;
            _configrepository = configrepository;
            _userservicerepository = userservicerepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PageList { get; set; } = new List<SelectListItem>();
        public User UserData { get; set; }
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token for the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadRoleList();
        }

        public void LoadRoleList()
        {
            RoleList.Clear();
            RoleList = _repository.GetList()
                .OrderBy(o => o.RoleName)
                .Select(s => new SelectListItem
                {
                    Value = s.RoleID.ToString(),
                    Text = s.RoleName
                }).ToList();

            RoleList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Role--" });
        }

        public JsonResult OnGetPageControlList(int RoleID)
        {
            var pageList = _configrepository.GetRoleConfigurationByRoleID(RoleID)
                             .OrderBy(d => d.PageName)
                             .Select(d => new SelectListItem
                             {
                                 Value = d.PageControlID.ToString(),
                                 Text = d.PageName
                             }).ToList();
            
            return new JsonResult(pageList);
        }


        public async Task<JsonResult> OnPostSaveUpdateData([FromBody] User user)
        {
            Tuple<bool, string,int> resultdata = null;

            try
            {
                //if (ModelState.IsValid)
                //{
                    user.LastUpdatedBy = LoggedUser.UserID;
                    user.lastupdateddate = DateTime.UtcNow;


                    if (user.UserID == 0)
                        resultdata = await _userservicerepository.SaveUser(user);
                    else if (user.UserID > 0)
                        resultdata = await _userservicerepository.UpdateUser(user);
                //}
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }

            return new JsonResult(new { success = resultdata.Item1, message = resultdata.Item2 });
        }

        public async Task<JsonResult> OnPostDeleteData([FromBody] int UserID)
        {
            string message = string.Empty;
            Tuple<bool, string> resultdata = null;

            try
            {
                resultdata = await _userservicerepository.DeleteUser(UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<JsonResult> OnGetUserByID(int UserID)
        {
            var resultdata = await _userservicerepository.GetUserByID(UserID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public  JsonResult OnGetRoleByApplicationID(int Application)
        {
            var resultdata =  _repository.GetRoleByApplicationID(Application);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public async Task<JsonResult> OnPostChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (request.UserID <= 0)
                {
                    return new JsonResult(new { result = new DataResponse { Error = true, Success = false, Message = "Invalid User ID." } });
                }

                if (string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return new JsonResult(new { result = new DataResponse { Error = true, Success = false, Message = "New password is required." } });
                }

                DataResponse dataResponse = await _userservicerepository.ChangePasswordAdminAsync(request.UserID, request.NewPassword);

                return new JsonResult(new { result = dataResponse });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error changing password");
                return new JsonResult(new { result = new DataResponse { Error = true, Success = false, Message = ex.Message } });
            }
        }

        public JsonResult OnPostUsersList([FromForm] UserDataTableRequest dataTableRequest)
        {
            try
            {
                var validColumns = new[] { "UserName" };

                // Validate and set default sort column
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "UserName";

                dataTableRequest.SortDirection = dataTableRequest.SortDirection?.ToLower() == "asc" ? "asc" : "desc";


                DataResponse dataResponse = new DataResponse();
                dataResponse = _userservicerepository.GetUserDataTable(dataTableRequest);

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    dataResponse.recordsTotal,
                    dataResponse.recordsFiltered,
                    data = dataResponse.Value
                });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error loading Vendor data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VHSRPInvoice>()
                });
            }
        }
    }
}
