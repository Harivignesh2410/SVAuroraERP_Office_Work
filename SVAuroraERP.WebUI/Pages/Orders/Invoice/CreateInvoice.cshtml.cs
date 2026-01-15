using SVAuroraERP.Domain;

namespace SVAuroraERP.WebUI.Pages.Orders.Invoice
{
    public class CreateInvoiceModel : HSRPBasePageModel
    {
        private readonly ILogger<CreateInvoiceModel> _logger;
        private readonly ICreateJobCardServiceRepository _createJobCardServiceRepository;
        private readonly ICreateInvoiceServiceRepository _createInvoiceServiceRepository;
        private readonly IListInvoiceServiceRepository _listInvoiceServiceRepository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.CreateInvoice; // ID for this specific page
        public CreateInvoiceModel(ILogger<CreateInvoiceModel> logger,
                                        ICreateJobCardServiceRepository createJobCardServiceRepository,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IAntiforgery antiforgery ,
                                        ICreateInvoiceServiceRepository createInvoiceServiceRepository,
                                         IPermissionServiceRepository permissionService,
                                        IListInvoiceServiceRepository listInvoiceServiceRepository)
        {
            _logger = logger;
            _createJobCardServiceRepository = createJobCardServiceRepository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _createInvoiceServiceRepository = createInvoiceServiceRepository;
            _listInvoiceServiceRepository = listInvoiceServiceRepository;
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
        public JsonResult OnGetSummaryForQualityProcessing()
        {
            var response = _createInvoiceServiceRepository.SummaryForQCCompleted();
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         

            return new JsonResult(new { result = response });
        }

        public JsonResult OnPostGenerateInvoice([FromBody] GenerateInvoiceRequest request) 
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                request.LastUpdatedBy = (int)HSRPLoggedUser.UserID;
                dataResponse = _createInvoiceServiceRepository.GenerateInvoice(request);
                return new JsonResult(new { success = true, result = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnPostOrderList([FromForm] InvoiceTransRequest tableRequest)
        {
            try
            {
                var validColumns = new[] { "RegNo" };

                // Validate and set default sort column
                tableRequest.SortColumn = validColumns.Contains(tableRequest.SortColumn) ? tableRequest.SortColumn : "RegNo";

                tableRequest.SortDirection = tableRequest.SortDirection?.ToLower() == "asc" ? "asc" : "desc";

                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.EmbossingStation)
                {
                    tableRequest.EmbossingStationID = (int)HSRPLoggedUser.EmbossingStationID;
                }
                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.Dealer)
                {
                    tableRequest.DealerID = (int)HSRPLoggedUser.DealerID;
                }
               


                DataResponse dataResponse = new DataResponse();
                dataResponse = _createInvoiceServiceRepository.GetListInvoiceTrans(tableRequest);

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