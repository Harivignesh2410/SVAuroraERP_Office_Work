using SVAuroraERP.Application.Interfaces.Persistance.OnlineOrders;
using SVAuroraERP.Domain.OnlineOrders;
using SVAuroraERP.WebUI.Pages.Orders.Invoice;

namespace SVAuroraERP.WebUI.Pages.OnlineOrders
{
    public class ApproveReplacementOrdersModel : HSRPBasePageModel
    {
        private readonly ILogger<CreateInvoiceModel> _logger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.ApproveReplacementOrders;
        private readonly IOnlineReplacementOrderServiceRepository _replacementOrderServiceRepository;
        
        public ApproveReplacementOrdersModel(ILogger<CreateInvoiceModel> logger,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IAntiforgery antiforgery,
                                         IPermissionServiceRepository permissionService,
                                            IOnlineReplacementOrderServiceRepository replacementOrderServiceRepository)
        {
            _logger = logger;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
            _replacementOrderServiceRepository = replacementOrderServiceRepository;
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
        
        public JsonResult OnPostReplacementOrderListData([FromForm] ReplacementOrderDTRequest dataTableRequest)
        {
            try
            {
                dataTableRequest.StartDate = ConvertDateonly(dataTableRequest.sStartDate)?.ToDateTime(TimeOnly.MinValue);
                dataTableRequest.EndDate = ConvertDateonly(dataTableRequest.sEndDate)?.ToDateTime(TimeOnly.MinValue);
                // Validate sort column
                var validColumns = new[] { "OrderNo" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "OrderNo";

                DataResponse dataResponse = new DataResponse();

                dataResponse = _replacementOrderServiceRepository.GetReplacementOrderList(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Replacement Order data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VOnlineReplacementOrderDetails>()
                });
            }
        }

        public JsonResult OnGetViewReplacementOrderById(int replacementOrderId)
        {
            var order = _replacementOrderServiceRepository
                            .GetReplacementOrderByID(replacementOrderId);

            if (order?.Value == null)
            {
                return new JsonResult(new { Success = false, Message = "Order not found" });
            }

            return new JsonResult(new { Success = true, data = order });
        }
        
        public JsonResult OnPostApproveReplacementOrder([FromBody] ApproveReplacementOrderData request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                request.LastUpdatedBy = (int)HSRPLoggedUser.UserID;
                dataResponse = _replacementOrderServiceRepository.ApproveReplacementOrder(request);
                return new JsonResult(new { success = true, result = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

