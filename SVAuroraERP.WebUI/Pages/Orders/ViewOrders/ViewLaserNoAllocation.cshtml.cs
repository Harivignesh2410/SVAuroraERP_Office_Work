namespace SVAuroraERP.WebUI.Pages.Orders.ViewOrders
{
    public class ViewLaserNoAllocationModel : HSRPBasePageModel
    {
        private readonly ILogger<ViewLaserNoAllocationModel> _logger;
        private readonly ILaserNoAllocationServiceRepository _laserNoAllocationServiceRepository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.ViewLaserNoAllocation; // ID for this specific page
        private readonly IHSRPOrdersServiceRepository _Orderrepository;

        public ViewLaserNoAllocationModel(ILogger<ViewLaserNoAllocationModel> logger,
                                        ILaserNoAllocationServiceRepository laserNoAllocationServiceRepository,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IAntiforgery antiforgery,
                                        IPermissionServiceRepository permissionrepository,
                                        IHSRPOrdersServiceRepository orderrepository)
        {
            _logger = logger;
            _laserNoAllocationServiceRepository = laserNoAllocationServiceRepository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionrepository;
            _Orderrepository = orderrepository;
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
            LoggedUserID = (int)HSRPLoggedUser.HSRPUserID;
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
        public JsonResult OnPostLaserNoAllocationListData([FromForm] ReadyforProcessingOrdersRequest dataTableRequest)
        {
            try
            {
                dataTableRequest.StartDate = ConvertDateonly(dataTableRequest.sStartDate)?.ToDateTime(TimeOnly.MinValue);
                dataTableRequest.EndDate = ConvertDateonly(dataTableRequest.sEndDate)?.ToDateTime(TimeOnly.MinValue);
                // Validate sort column
                var validColumns = new[] { "OrderNo" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "OrderNo";


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
                dataResponse = _laserNoAllocationServiceRepository.GetLaserNoAllocation(dataTableRequest);

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
        //public JsonResult OnGetHSRPOrderSummaryData()
        //{
        //    var response = _hsrpOrdersServiceRepository.GetOrderSummary();

        //    return new JsonResult(new { result = response });
        //}
        //public JsonResult OnPostSaveUpdateData([FromBody] HSRPlaserStockRequest request)
        //{
        //    DataResponse dataResponse = new DataResponse();
        //    try
        //    {
        //        if (request == null || string.IsNullOrWhiteSpace(request.OrderIds))
        //        {
        //            return new JsonResult(new { success = false, message = "OrderIds cannot be empty" });
        //        }

        //        request.LastUpdatedBy = (int)HSRPLoggedUser.UserID;
        //        request.LastUpdatedDate = DateTime.UtcNow;
        //        // Save or update
        //        if (request.OrderIds.Length > 0)
        //        {
        //            dataResponse = _laserNoAllocationServiceRepository.Save(request);
        //        }
        //        return new JsonResult(new { success = true, result = dataResponse });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new { success = false, message = ex.Message });
        //    }
        //}
        public JsonResult OnGetSummaryForLaserNoAllocation()
        {
            var Request = new SummaryFilterData();
            Request.UserID = (int)HSRPLoggedUser.HSRPUserID;
            Request.OrderStatusID = (int)OrderStatus.ReadyForProcessing;
            var response = _Orderrepository.SummaryOrdersByStatusID(Request);

            return new JsonResult(new { result = response });
        }
    }
}
