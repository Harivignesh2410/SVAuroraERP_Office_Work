namespace SVAuroraERP.WebUI.Pages.Online.Master
{
    public class HomeFitmentPincodeModel : BasePageModel
    {
        private readonly IHomeFitmentPincodeServiceRepository _repository = null;
        private readonly IStateServiceRepository _staterepository = null;
        private readonly ILogger<HomeFitmentPincodeServiceRepository> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.HomeFitmentPincode; // ID for this specific page
        public HomeFitmentPincodeModel(IHomeFitmentPincodeServiceRepository respository,
            IStateServiceRepository stateRepository, IDistrictServiceRepository districtrepository,
                           ILogger<HomeFitmentPincodeServiceRepository> logger,
                           IAntiforgery antiforgery,
                            IPermissionServiceRepository permissionService,
                           SessionService sessionService)
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
        public List<SelectListItem> StateList { get; set; } = new List<SelectListItem>();
    
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadStateList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadStateList()
        {
            var dataResponse = _staterepository.GetState();

            StateList.Clear();
            var district = dataResponse.Value as List<VState>;
            if (district != null)
            {
                StateList = district
                .Select(s => new SelectListItem
                {
                    Value = s.StateID.ToString(),
                    Text = s.StateName
                }).ToList();
            }
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select District--" });
        }
        public JsonResult OnGetDistrictByStateID(int StateID)
        {
            var response = _repository.GetDistrictByStateID(StateID);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] HomeFitmentPincode HomeFitmentPincode)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                HomeFitmentPincode.LastUpdatedBy = LoggedUser.UserID;

                if (HomeFitmentPincode.HomeFitmentPincodeID == 0)
                    resultdata = _repository.Save(HomeFitmentPincode);
                else if (HomeFitmentPincode.HomeFitmentPincodeID > 0)
                    resultdata = _repository.Update(HomeFitmentPincode);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetHomeFitmentPincodeIDByID(int ID)
        {
            DataResponse resultdata = _repository.GetHomeFitmentPincodeByID(ID);

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
                return new JsonResult(new { resultdata = dataResponse });
            }
        }
        public JsonResult OnPostHomeFitmentPincodeList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "Pincode" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "Pincode";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetHomeFitmentPincodeDataTableList(dataTableRequest);

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
