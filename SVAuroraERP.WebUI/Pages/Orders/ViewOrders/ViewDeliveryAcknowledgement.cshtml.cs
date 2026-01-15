namespace SVAuroraERP.WebUI.Pages.Orders.ViewOrders
{
    public class ViewDeliveryAcknowledgementModel : HSRPBasePageModel
    {
        private readonly ILogger<ViewDeliveryAcknowledgementModel> _logger;
        private readonly IDeliveryAcknowledgementServiceRepository _repository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IHSRPOrdersServiceRepository _Orderrepository;
        private readonly ICreateInvoiceServiceRepository _createInvoiceServiceRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.ViewDeliveryAcknowledgement; // ID for this specific page
        public ViewDeliveryAcknowledgementModel(ILogger<ViewDeliveryAcknowledgementModel> logger,
                                        IDeliveryAcknowledgementServiceRepository repository,
                                        IErrorLoggerService errorLoggerService,
                                        ICreateInvoiceServiceRepository createInvoiceServiceRepository,
                                        IHSRPOrdersServiceRepository Orderrepository, IAuditLogger auditLogger,
                                         IPermissionServiceRepository permissionService,
                                        IAntiforgery antiforgery, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _repository = repository;
            _Orderrepository = Orderrepository;
            _createInvoiceServiceRepository = createInvoiceServiceRepository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
            _webHostEnvironment = webHostEnvironment;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
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
            if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.EmbossingStation)
            {
                RedirectToPage("/Orders/ViewOrders/ViewDeliverAcknowledgement");
            }
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
        public JsonResult OnPostDeliveryAcknowledgementOrdersListData([FromForm] DeliveryAcknowledgementOrdersRequest dataTableRequest)
        {
            try
            {
                dataTableRequest.StartDate = ConvertDateonly(dataTableRequest.sStartDate)?.ToDateTime(TimeOnly.MinValue);
                dataTableRequest.EndDate = ConvertDateonly(dataTableRequest.sEndDate)?.ToDateTime(TimeOnly.MinValue);
                // Validate sort column
                var validColumns = new[] { "OrderNo" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "OrderNo";


                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.Dealer)
                {
                    dataTableRequest.DealerID = HSRPLoggedUser.HSRPUserID;
                }

                DataResponse dataResponse = new DataResponse();

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
                dataResponse = _repository.GetDeliveryAcknowledgementOrders(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Delivery Acknowledgement data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VDeliveryAcknowledgement>()
                });
            }
        }
        public JsonResult OnGetSummaryForDeliveryAcknowledgementOrders()
        {
            var Request = new SummaryFilterData();
            Request.UserID = (int)HSRPLoggedUser.HSRPUserID;
            Request.OrderStatusID = (int)OrderStatus.Delivered;
            var response = _Orderrepository.SummaryOrdersByStatusID(Request);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetHSRPDataByID(int HSRPOrderID)
        {
            DataResponse dataResponse = new DataResponse();

            dataResponse = _Orderrepository.GetHsrporderByID(HSRPOrderID);

            return new JsonResult(dataResponse.Value);
        }
    }
}
