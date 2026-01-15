namespace SVAuroraERP.WebUI.Pages.Online.OEMVendorMapping
{
    public class OEMVendorDealerMappingModel : BasePageModel
    {
        private readonly IOEMVendorDealerMappingServiceRepository _repository;
        private readonly IHSRPUserServiceRepository _hSRPUserServiceRepository;
        private readonly ILogger<OEMVendorDealerMappingModel> _logger;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.OEMVendorDealerMapping; // ID for this specific page

        public readonly IAntiforgery _antiforgery;
        public OEMVendorDealerMappingModel(IOEMVendorDealerMappingServiceRepository respository,
                                           IHSRPUserServiceRepository hSRPUserServiceRepository,
                                        ILogger<OEMVendorDealerMappingModel> logger, IAntiforgery antiforgery,
                                        IDistrictServiceRepository districtServiceRepository,
                                        IStateServiceRepository stateServiceRepository,
                                        IHomeFitmentPincodeServiceRepository homerepository,
                                        IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _hSRPUserServiceRepository = hSRPUserServiceRepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> OEMList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> EmbossingStationList { get; set; } = new List<SelectListItem>();
 
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadOEMList();
            LoadEmbossingStation();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
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
        public void LoadEmbossingStation()
        {
            DataResponse dataResponse = new DataResponse();
            EmbossingStationList.Clear();
            dataResponse = _hSRPUserServiceRepository.GetEmbossingStation();
            EmbossingStationList = ((List<VHSRPUser>)dataResponse.Value)
                .OrderBy(o => o.CompanyName)
                .Select(s => new SelectListItem
                {
                    Value = s.HSRPUserID.ToString(),
                    Text = $"{s.CompanyName} - {s.DistrictName}, {s.StateName}"
                }).ToList();

            EmbossingStationList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Embossing Station--" });
        }
        public JsonResult OnGetOEMVendorDealerMappingByID(int ID)
        {
            DataResponse resultdata = _repository.GetOEMVendorDealerMappingByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostSaveUpdateData([FromBody] OEMVendorDealerMapping request)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                request.LastUpdatedBy = LoggedUser.UserID;

                if (request.OEMVendorDealerMappingID == 0)
                    resultdata = _repository.Save(request);
                else if (request.OEMVendorDealerMappingID > 0)
                    resultdata = _repository.Update(request);

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
        public JsonResult OnGetDealerByOEMID(int OEMID)
         {
            var response = _repository.GetDealerByOEMID(OEMID);

            return new JsonResult(new { result = response });
        }
        //public JsonResult OnGetEmbossingStationByDealerID(int DealerID)
        //{
        //    var response = _repository.GetEmbossingStationByDealerID(DealerID);

        //    return new JsonResult(new { result = response });
        //}
        public JsonResult OnGetVendorCodeByESID(int ESID)
        {
            var response = _repository.GetVendorCodeByEmbossingStationID(ESID);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnPostOEMVendorDealerMapping([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "DealerName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "DealerName";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetOEMVendorDealerMappingDataTableList(dataTableRequest);

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
