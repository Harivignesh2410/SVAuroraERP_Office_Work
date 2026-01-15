namespace SVAuroraERP.WebUI.Pages.Online.OEMVendorMapping
{
    public class OEMVendorCodeMappingModel : BasePageModel
    {
        private readonly IOEMVendorCodeMappingServiceRepository _repository;
        private readonly IHSRPUserServiceRepository _hSRPUserServiceRepository;
        private readonly IStateServiceRepository _stateServiceRepository;
        private readonly ILogger<OEMVendorCodeMappingModel> _logger;
        private readonly IDistrictServiceRepository _districtServiceRepository;
        public readonly IAntiforgery _antiforgery;  
        public readonly IHomeFitmentPincodeServiceRepository _homerepository = null;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.OEMVendorCodeMapping; // ID for this specific page
        public OEMVendorCodeMappingModel(IOEMVendorCodeMappingServiceRepository respository, 
            IHSRPUserServiceRepository hSRPUserServiceRepository,
                         ILogger<OEMVendorCodeMappingModel> logger, IAntiforgery antiforgery, 
                         IDistrictServiceRepository districtServiceRepository, 
                         IStateServiceRepository stateServiceRepository, 
                         IHomeFitmentPincodeServiceRepository homerepository,
                          IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _hSRPUserServiceRepository = hSRPUserServiceRepository;
            _districtServiceRepository = districtServiceRepository;
            _stateServiceRepository = stateServiceRepository;
            _homerepository = homerepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }

        public List<SelectListItem> StateList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OEMList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
   
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
            dataResponse = _stateServiceRepository.GetState();
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
            dataResponse = _hSRPUserServiceRepository.GetOEM();
            OEMList = ((List<VHSRPUser>)dataResponse.Value)
                .OrderBy(o => o.CompanyName)
                .Select(s => new SelectListItem
                {
                    Value = s.HSRPUserID.ToString(),
                    Text = s.CompanyName
                }).ToList();

            OEMList.Insert(0, new SelectListItem { Value = "0", Text = "--Select OEM--" });
        }
        public JsonResult OnGetOEMVendorCodeMappingByID(int ID)
        {
            DataResponse resultdata = _repository.GetOEMVendorCodeMappingByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostSaveUpdateData([FromBody] OEMVendorCodeMapping OEMVendorCodeMapping)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                OEMVendorCodeMapping.LastUpdatedBy = LoggedUser.UserID;

                if (OEMVendorCodeMapping.OEMVendorCodeMappingID == 0)
                    resultdata = _repository.Save(OEMVendorCodeMapping);
                else if (OEMVendorCodeMapping.OEMVendorCodeMappingID > 0)
                    resultdata = _repository.Update(OEMVendorCodeMapping);

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
                dataResponse = _repository.Delete(ID, LoggedUser.UserID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata = dataResponse });
            }
        }
        public JsonResult OnGetDistrictByStateID(int StateID)
        {
            var response = _homerepository.GetDistrictByStateID(StateID);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnPostOEMVendorCodeMappingList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "VendorCode" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "VendorCode";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetOEMVendorCodeMappingDataTableList(dataTableRequest);

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