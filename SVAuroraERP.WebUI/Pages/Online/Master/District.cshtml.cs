namespace SVAuroraERP.WebUI.Pages.Online.Master
{
    public class DistrictModel : BasePageModel
    {
        private readonly IDistrictServiceRepository _repository = null;
        private readonly IStateServiceRepository _staterepository = null;
        private readonly ILogger<DistrictServiceRepository> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.District; // ID for this specific page
        public DistrictModel(IDistrictServiceRepository respository,
            IStateServiceRepository stateRepository,
                           ILogger<DistrictServiceRepository> logger,
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

        public JsonResult OnPostSaveUpdateData([FromBody] District District)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                District.LastUpdatedBy = LoggedUser.UserID;

                if (District.DistrictID == 0)
                    resultdata = _repository.Save(District);
                else if (District.DistrictID > 0)
                    resultdata = _repository.Update(District);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetDistrictByID(int ID)
        {
            DataResponse resultdata = _repository.GetDistrictByID(ID);

            return new JsonResult(resultdata);
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
        public JsonResult OnPostDistrictList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "DistrictName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "DistrictName";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetDistrictList(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Vehicle Plate Image data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VDistrict>()
                });
            }
        }
    }
}