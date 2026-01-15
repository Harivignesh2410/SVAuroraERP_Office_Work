namespace SVAuroraERP.WebUI.Pages.Dealer
{
    public class DealerListModel : HSRPBasePageModel
    {
        private readonly ILogger<DealerListModel> _logger;
        private readonly IHSRPUserServiceRepository _hSRPUserServiceRepository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.DealerList; // ID for this specific page
        public DealerListModel(ILogger<DealerListModel> logger,
                                        IHSRPUserServiceRepository hSRPUserServiceRepository,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IPermissionServiceRepository permissionService,
                                        IAntiforgery antiforgery)
        {
            _logger = logger;
            _hSRPUserServiceRepository = hSRPUserServiceRepository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
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
        public JsonResult OnPostDealerListByOEM([FromForm] OEMDataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "CompanyName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "CompanyName";
                dataTableRequest.OEMID = HSRPLoggedUser.HSRPUserID;
                DataResponse dataResponse = new DataResponse();
                //dataTableRequest.oem = LoggedUser.EmbossingStationID;
                dataResponse = _hSRPUserServiceRepository.GetDealerListByOEM(dataTableRequest);

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
    }
}
