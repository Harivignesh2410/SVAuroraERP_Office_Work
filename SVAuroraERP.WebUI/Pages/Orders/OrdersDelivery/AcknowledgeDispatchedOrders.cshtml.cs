namespace SVAuroraERP.WebUI.Pages.Orders.OrdersDelivery
{
    public class AcknowledgeDispatchedOrdersModel : BasePageModel
    {
        private readonly ILogger<AcknowledgeDispatchedOrdersModel> _logger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private readonly IGenerateDeliveryDataServiceRepository _generateDeliveryDataServiceRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private const int PageControlID = (int)Common.Pages.AcknowledgeDispatchedOrders; // ID for this specific page
        public AcknowledgeDispatchedOrdersModel(ILogger<AcknowledgeDispatchedOrdersModel> logger,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IAntiforgery antiforgery,
                                         IWebHostEnvironment webHostEnvironment,
                                         IPermissionServiceRepository permissionService,
                                        IGenerateDeliveryDataServiceRepository generateDeliveryDataServiceRepository)
        {
            _logger = logger;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
            _webHostEnvironment = webHostEnvironment;
            _generateDeliveryDataServiceRepository = generateDeliveryDataServiceRepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public JsonResult OnPostListInvoice([FromForm] DataTableRequest tableRequest)
        {
            try
            {
                var validColumns = new[] { "CompanyName" };

                // Validate and set default sort column
                tableRequest.SortColumn = validColumns.Contains(tableRequest.SortColumn) ? tableRequest.SortColumn : "CompanyName";

                tableRequest.SortDirection = tableRequest.SortDirection?.ToLower() == "asc" ? "asc" : "desc";


                DataResponse dataResponse = new DataResponse();
                dataResponse = _generateDeliveryDataServiceRepository.GetDispatchDetails(tableRequest);

                return new JsonResult(new
                {
                    draw = tableRequest.Draw,
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
                    draw = tableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VHSRPInvoice>()
                });
            }
        }
        public JsonResult OnPostGenerateDeliveryData([FromBody] int GenerateDeliveryID)
        {
            var dataResponse = _generateDeliveryDataServiceRepository.GetListDispatchDataTrans(GenerateDeliveryID);
            return new JsonResult(dataResponse.Value);
        }
        private DateOnly? ConvertDateonly(string sdate)
        {
            DateOnly? dtConvertedDate = null;

            const string DateFormat = "dd/MM/yyyy"; // Matches "28/04/2025"

            if (DateOnly.TryParseExact(sdate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsedDt))
            {
                dtConvertedDate = parsedDt;
            }

            return dtConvertedDate;
        }
        public async Task<JsonResult> OnPostSaveUpdateData(AcknowlegdeGenerateDeliveryRequest request)
        {
            var dataResponse = new DataResponse();

            try
            {
                request.DeliveryDate = (DateOnly)ConvertDateonly(request.sDeliveryDate);
                request.LastUpdatedBy = LoggedUser.UserID;

                dataResponse = _generateDeliveryDataServiceRepository.AcknowledgeGenerateDeliveryData(request);

                return new JsonResult(new
                {
                    success = dataResponse.ID > 0,
                    message = dataResponse.Message,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }


    }
}
