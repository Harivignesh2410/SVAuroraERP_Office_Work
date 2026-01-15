namespace SVAuroraERP.WebUI.Pages.Authentication
{
    public class HSRPConfigModel : BasePageModel
    {
        private readonly IHSRPConfigServiceRepository _repository;
        private readonly ILogger<HSRPRoleConfig> _logger;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.HSRPConfig; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;

            
        public HSRPConfigModel(IHSRPConfigServiceRepository respository,
                           ILogger<HSRPRoleConfig> logger,
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
        public List<SelectListItem> HSRPUserList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();

        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadHSRPUserList();
            LoadRoleList();
        }
        public void LoadHSRPUserList()
        {
            DataResponse dataResponse = new DataResponse();
            HSRPUserList.Clear();
            dataResponse = _repository.GetHSRPUser();
            HSRPUserList = ((List<VHSRPUser>)dataResponse.Value)
                .Where(w => w.HSRPUserTypeID == 3)
                .OrderBy(o => o.HSRPUserCode)
                .Select(s => new SelectListItem
                {
                    Value = s.HSRPUserID.ToString(),
                    Text = s.CompanyName
                }).ToList();

            HSRPUserList.Insert(0, new SelectListItem { Value = "0", Text = "--Select HSRPUser--" });
        }
        public void LoadRoleList()
        {
            DataResponse dataResponse = new DataResponse();
            RoleList.Clear();
            dataResponse = _repository.GetRole();
            RoleList = ((List<VRole>)dataResponse.Value)
                 .Where(w => w.ApplicationID == 2)
                .OrderBy(o => o.RoleName)
                .Select(s => new SelectListItem
                {
                    Value = s.RoleID.ToString(),
                    Text = s.RoleName
                }).ToList();

            RoleList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Role--" });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] OEMConfig BoxData)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                BoxData.LastUpdatedBy = LoggedUser.UserID;

                    resultdata = _repository.Save(BoxData);

                return new JsonResult(new { result = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = resultdata });
            }
        }

        public JsonResult OnPostSaveUpdateDataRole([FromBody] HSRPRoleConfig HSRPRoleData)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                HSRPRoleData.LastUpdatedBy = LoggedUser.UserID;

                resultdata = _repository.SaveRole(HSRPRoleData);

                return new JsonResult(new { result = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = resultdata });
            }
        }
        public JsonResult OnGetHSRPConfig()
        {
            DataResponse dataResponse = new DataResponse();

            dataResponse = _repository.GetHSRPConfig();

            return new JsonResult(dataResponse.Value);
        }
    }
}
