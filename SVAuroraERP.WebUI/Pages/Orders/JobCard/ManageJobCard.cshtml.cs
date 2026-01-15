namespace SVAuroraERP.WebUI.Pages.Orders.JobCard
{
    public class ManageJobCardModel : HSRPBasePageModel
    {
        private readonly ICreateJobCardServiceRepository _repository = null;
        private readonly IStateServiceRepository _staterepository = null;
        private readonly ILogger<CreateJobCardServiceRepository> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IHSRPUserServiceRepository _hsrpuserrepository = null;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.ManageJobCard; // ID for this specific page

        public ManageJobCardModel(ICreateJobCardServiceRepository respository,
            IStateServiceRepository stateRepository,
                           ILogger<CreateJobCardServiceRepository> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                             IPermissionServiceRepository permissionService,
                           IHSRPUserServiceRepository hSRPUserServiceRepository)
        {
            _repository = respository;
            _staterepository = stateRepository;
            _logger = logger;
            _antiforgery = antiforgery;
            _hsrpuserrepository = hSRPUserServiceRepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
 
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public static DateOnly? ConvertDateonly(string? sdate)
        {
            if (string.IsNullOrWhiteSpace(sdate))
                return null;

            string[] formats = {
                                    "dd/MM/yyyy",    // expected from frontend
                                    "yyyy-MM-dd",    // fallback for ISO
                                    "d MMM, yyyy"    // if flatpickr default used
                                };

            foreach (var format in formats)
            {
                if (DateOnly.TryParseExact(sdate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    return parsedDate;
                }
            }

            return null;
        }
        public JsonResult OnPostHSRPJobCardListData([FromForm] CreateJobRequest dataTableRequest)
        {
            try
            {
                dataTableRequest.StartDate = ConvertDateonly(dataTableRequest.sStartDate)?.ToDateTime(TimeOnly.MinValue);
                dataTableRequest.EndDate = ConvertDateonly(dataTableRequest.sEndDate)?.ToDateTime(TimeOnly.MinValue);
                // Validate sort column
                var validColumns = new[] { "JobCardNo" };

                // Validate and set default sort column
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn)
                    ? dataTableRequest.SortColumn
                    : "JobCardNo";
                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.EmbossingStation)
                {
                    dataTableRequest.EmbossingStationID = HSRPLoggedUser.EmbossingStationID;
                }
                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.Dealer)
                {
                    dataTableRequest.DealerID = HSRPLoggedUser.DealerID;
                }

                dataTableRequest.SortDirection = dataTableRequest.SortDirection?.ToLower() == "asc" ? "asc" : "desc";


                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetHsrpJobcard(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Vendor data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VHSRPOrder>()
                });
            }
        }
        public JsonResult OnGetJobCardByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            dataResponse = _repository.GetJobcardByID(ID);

            return new JsonResult(new { result = dataResponse });
        }
        
    }
}
