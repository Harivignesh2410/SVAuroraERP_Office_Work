namespace SVAuroraERP.WebUI.Pages.Orders.Invoice
{
    public class ListInvoiceModel : HSRPBasePageModel
    {
        private readonly ILogger<ListInvoiceModel> _logger;
        private readonly IListInvoiceServiceRepository _listInvoiceServiceRepository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.ListInvoice; // ID for this specific page
        public ListInvoiceModel(ILogger<ListInvoiceModel> logger,
                                        IListInvoiceServiceRepository listInvoiceServiceRepository,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IAntiforgery antiforgery,
                                         IPermissionServiceRepository permissionService)
        {
            _logger = logger;
            _listInvoiceServiceRepository = listInvoiceServiceRepository;
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
        public JsonResult OnPostListInvoice([FromForm] ListInvoiceRequest dataTableRequest)
        {
            try
            {
                dataTableRequest.StartDate = ConvertDateonly(dataTableRequest.sStartDate)?.ToDateTime(TimeOnly.MinValue);
                dataTableRequest.EndDate = ConvertDateonly(dataTableRequest.sEndDate)?.ToDateTime(TimeOnly.MinValue);

                var validColumns = new[] { "DealerCode" };

                // Validate and set default sort column
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn)? dataTableRequest.SortColumn: "DealerCode";

                dataTableRequest.SortDirection = dataTableRequest.SortDirection?.ToLower() == "asc" ? "asc" : "desc";

                dataTableRequest.HsrpUserID = HSRPLoggedUser.HSRPUserID;

                DataResponse dataResponse = new DataResponse();
                dataResponse = _listInvoiceServiceRepository.GetListInvoice(dataTableRequest);

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
                    data = new List<VHSRPInvoice>()
                });
            }
        }
        public JsonResult OnPostOrderList([FromForm] HSRPInvoiceTransRequest tableRequest)
        {
            try
            {
                var validColumns = new[] { "RegNo" };

                // Validate and set default sort column
                tableRequest.SortColumn = validColumns.Contains(tableRequest.SortColumn) ? tableRequest.SortColumn : "RegNo";

                tableRequest.SortDirection = tableRequest.SortDirection?.ToLower() == "asc" ? "asc" : "desc";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _listInvoiceServiceRepository.GetListInvoiceTrans(tableRequest);

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
                    data = new List<VHSRPInvoiceTrans>()
                });
            }
        }
    }
}
