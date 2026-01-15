namespace SVAuroraERP.WebUI.Pages.Online.SubUser
{
    public class OEMSubUserModel : BasePageModel
    {
        private readonly IHSRPUserServiceRepository _repository = null;
        private readonly IStateServiceRepository _staterepository = null;
        private readonly ILogger<IHSRPUserServiceRepository> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IHomeFitmentPincodeServiceRepository _homerepository = null;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.OEMSubUser; // ID for this specific page
        public OEMSubUserModel(IHSRPUserServiceRepository respository,
            IStateServiceRepository stateRepository,
                           ILogger<IHSRPUserServiceRepository> logger,
                           IAntiforgery antiforgery,
                           IPermissionServiceRepository permissionService,
                           SessionService sessionService, IHomeFitmentPincodeServiceRepository homerepository)
        {
            _repository = respository;
            _staterepository = stateRepository;
            _logger = logger;
            _antiforgery = antiforgery;
            _homerepository = homerepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> StateList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OEMList { get; set; } = new List<SelectListItem>();
 
 
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadStateList();
            LoadOEMList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadStateList()
        {
            DataResponse dataResponse = new DataResponse();
            StateList.Clear();
            dataResponse = _staterepository.GetState();
            StateList = ((List<VState>)dataResponse.Value)
                .OrderBy(o => o.StateName)
                .Select(s => new SelectListItem
                {
                    Value = s.StateID.ToString(),
                    Text = s.StateName
                }).ToList();

            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State --" });
        }
        public void LoadOEMList()
        {
            DataResponse dataResponse = new DataResponse();
            OEMList.Clear();
            dataResponse = _repository.GetOEM();
            OEMList = ((List<VHSRPUser>)dataResponse.Value)
                .OrderBy(o => o.CompanyName)
                .Select(s => new SelectListItem
                {
                    Value = s.HSRPUserID.ToString(),
                    Text = s.CompanyName
                }).ToList();

            OEMList.Insert(0, new SelectListItem { Value = "0", Text = "--Select OEM--" });
        }
        public JsonResult OnGetDistrictByStateID(int StateID)
        {
            var response = _homerepository.GetDistrictByStateID(StateID);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetOEMSubUserByID(int ID)
        {
            DataResponse resultdata = _repository.GetHSRPUserByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostSaveUpdateData([FromBody] HSRPUser HSRPUser)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                HSRPUser.LastUpdatedBy = LoggedUser.UserID;

                if (HSRPUser.HSRPUserID == 0)
                    resultdata = _repository.Save(HSRPUser);
                else if (HSRPUser.HSRPUserID > 0)
                    resultdata = _repository.Update(HSRPUser);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse dataResponse = null;

            try
            {
                dataResponse = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata = dataResponse });
            }
        }
        public JsonResult OnPostOEMSubUserList([FromForm] HSRPUserRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "CompanyName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "CompanyName";
                  dataTableRequest.UserTypeID = (byte)HSRPUserTypeEnum.OEMSubUsers;
                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetHSRPUserDataTableList(dataTableRequest);

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
                _logger.LogError(ex, "Error loading OEM Sub - User data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VHSRPUser>()
                });
            }
        }
    }
}