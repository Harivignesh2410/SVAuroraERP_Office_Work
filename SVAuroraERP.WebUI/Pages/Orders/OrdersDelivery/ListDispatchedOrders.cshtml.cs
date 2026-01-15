namespace SVAuroraERP.WebUI.Pages.Orders.OrdersDelivery
{
    public class ListDispatchedOrdersModel : BasePageModel
    {
        private readonly ILogger<ListDispatchedOrdersModel> _logger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private readonly IGenerateDeliveryDataServiceRepository _generateDeliveryDataServiceRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IListDispatchedOrdersServiceRepository _listDispatchedOrdersServiceRepository;

        private const int PageControlID = (int)Common.Pages.GenerateDeliveryData; // ID for this specific page
        public ListDispatchedOrdersModel(ILogger<ListDispatchedOrdersModel> logger,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IAntiforgery antiforgery,
                                         IWebHostEnvironment webHostEnvironment,
                                         IPermissionServiceRepository permissionService,
                                        IGenerateDeliveryDataServiceRepository generateDeliveryDataServiceRepository,
                                        IListDispatchedOrdersServiceRepository listDispatchedOrdersServiceRepository)
        {
            _logger = logger;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
            _webHostEnvironment = webHostEnvironment;
            _generateDeliveryDataServiceRepository = generateDeliveryDataServiceRepository;
            _listDispatchedOrdersServiceRepository = listDispatchedOrdersServiceRepository;
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
        public JsonResult OnPostListDispatchOrderData([FromForm] DataTableRequest tableRequest)
        {
            try
            {
                var validColumns = new[] { "CompanyName" };

                // Validate and set default sort column
                tableRequest.SortColumn = validColumns.Contains(tableRequest.SortColumn) ? tableRequest.SortColumn : "CompanyName";

                tableRequest.SortDirection = tableRequest.SortDirection?.ToLower() == "asc" ? "asc" : "desc";


                DataResponse dataResponse = new DataResponse();
                dataResponse = _listDispatchedOrdersServiceRepository.GetListDispatchOrdersDetails(tableRequest);

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
                _logger.LogError(ex, "Error loading List Dispatch data");

                return new JsonResult(new
                {
                    draw = tableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VListDispatchOrder>()
                });
            }
        }

        public JsonResult OnPostListDeliveryData([FromBody] int GenerateDeliveryID)
        {
            var dataResponse = _listDispatchedOrdersServiceRepository.GetDispatchTransData(GenerateDeliveryID);
            return new JsonResult(dataResponse.Value);
        }  
    }
}
