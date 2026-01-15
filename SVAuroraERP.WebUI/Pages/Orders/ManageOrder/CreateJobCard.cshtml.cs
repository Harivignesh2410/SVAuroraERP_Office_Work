namespace SVAuroraERP.WebUI.Pages.Orders.ManageOrder
{
    public class CreateJobCardModel : HSRPBasePageModel
    {
        private readonly ILogger<CreateJobCardModel> _logger;
        private readonly ICreateJobCardServiceRepository _createJobCardServiceRepository;
        private readonly IHSRPOrdersServiceRepository _hsrpOrdersServiceRepository;
        private readonly IHSRPUserServiceRepository _hsrpuserrepository = null;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.CreateJobCard; // ID for this specific page
        public CreateJobCardModel(ILogger<CreateJobCardModel> logger,
                                        ICreateJobCardServiceRepository createJobCardServiceRepository,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IHSRPUserServiceRepository hsrpuserrepository,
                                         IPermissionServiceRepository permissionService,
                                        IAntiforgery antiforgery,
                                        IHSRPOrdersServiceRepository hsrpOrdersServiceRepository)
        {
            _logger = logger;
            _createJobCardServiceRepository = createJobCardServiceRepository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
            _hsrpuserrepository = hsrpuserrepository;
            _hsrpOrdersServiceRepository = hsrpOrdersServiceRepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> EmbossingStationList { get; set; } = new List<SelectListItem>();
        public int LoggedUserID { get; set; }
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");
            LoggedUserID = (int)HSRPLoggedUser.HSRPUserID;
            if (HSRPLoggedUser.HSRPUserTypeID != (byte)Common.HSRPUserType.EmbossingStation && HSRPLoggedUser.HSRPUserTypeID != (byte)Common.HSRPUserType.Admin)
            {
                return RedirectToPage("/Orders/ViewOrders/ViewCreateJobCard");
            }
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
        public JsonResult OnPostCreateJobCardsListData([FromForm] CreateJobCardRequest dataTableRequest)
        {
            try
            {
                dataTableRequest.StartDate = ConvertDateonly(dataTableRequest.sStartDate)?.ToDateTime(TimeOnly.MinValue);
                dataTableRequest.EndDate = ConvertDateonly(dataTableRequest.sEndDate)?.ToDateTime(TimeOnly.MinValue);
                // Validate sort column
                var validColumns = new[] { "OrderNo", "RegNo", "Dealer", "OEM", "DealerCode", "RearLaserSerialNo", "FrontLaserSerialNo", "FrontPlateDimension", "FrontPlateDimension" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "OrderNo";
                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.EmbossingStation)
                {
                    dataTableRequest.EmbossingStationID = HSRPLoggedUser.HSRPUserID;
                }
                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.Dealer)
                {
                    dataTableRequest.DealerID = HSRPLoggedUser.HSRPUserID;
                }
                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.OEM)
                {
                    dataTableRequest.OEMID = HSRPLoggedUser.HSRPUserID;
                }


                DataResponse dataResponse = new DataResponse();
                dataResponse = _createJobCardServiceRepository.GetCreateJobCard(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Create Job Data data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VCreateJobCard>()
                });
            }
        }
        public JsonResult OnPostSaveUpdateData([FromBody] HSRPJobCardRequest request)
        {
            var dataResponse = new DataResponse();

            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.OrderIds))
                {
                    return new JsonResult(new { success = false, message = "OrderIds cannot be empty" });
                }

                request.LastUpdatedBy = (int)HSRPLoggedUser.UserID;
                request.LastUpdatedDate = DateTime.UtcNow;

                dataResponse = _createJobCardServiceRepository.Save(request);

                if (dataResponse.Success)
                {
                    return new JsonResult(new
                    {
                        success = true,
                        message = $"Job Card generated successfully",
                        jobCardID = dataResponse.ID
                    });
                }

                return new JsonResult(new { success = false, message = dataResponse.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public JsonResult OnGetSummaryForLaserNoAllocation()

        {
            var Request = new SummaryFilterData();
            Request.UserID = (int)HSRPLoggedUser.HSRPUserID;
            Request.OrderStatusID = (int)OrderStatus.LaserNoAssigned;
            var response = _hsrpOrdersServiceRepository.SummaryOrdersByStatusID(Request);

            return new JsonResult(new { result = response });
        }
    }
}