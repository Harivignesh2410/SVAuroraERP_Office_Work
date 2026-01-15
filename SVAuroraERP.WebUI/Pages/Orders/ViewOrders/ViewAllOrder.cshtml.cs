namespace SVAuroraERP.WebUI.Pages.Orders.ViewOrders
{
    public class ViewAllOrderModel : BasePageModel
    {
        private readonly ILogger<ViewAllOrderModel> _logger;
        private readonly IHSRPOrdersServiceRepository _hsrpOrdersServiceRepository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.ViewAllOrder; // ID for this specific page
        public ViewAllOrderModel(ILogger<ViewAllOrderModel> logger,
                                        IHSRPOrdersServiceRepository hsrpOrdersServiceRepository,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IPermissionServiceRepository permissionService,
                                        IAntiforgery antiforgery)
        {
            _logger = logger;
            _hsrpOrdersServiceRepository = hsrpOrdersServiceRepository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public int LoggedUserID { get; set; }
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");
            LoggedUserID = (int)HSRPLoggedUser.HSRPUserID;
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
        public JsonResult OnPostHSRPOrderListData([FromForm] HsrpOrderRequest dataTableRequest)
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
                else if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.Dealer)
                {
                    dataTableRequest.DealerID = HSRPLoggedUser.HSRPUserID;
                }
                else if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.OEM)
                {
                    dataTableRequest.OEMID = HSRPLoggedUser.HSRPUserID;
                }
                dataResponse = _hsrpOrdersServiceRepository.GetHsrporder(dataTableRequest);

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
        public JsonResult OnGetHSRPOrderSummaryData()
        {
            var Request = new SummaryFilterData();
            Request.UserID = (int)HSRPLoggedUser.HSRPUserID;
            Request.OrderStatusID = (int)OrderStatus.AllOrders;
            var response = _hsrpOrdersServiceRepository.SummaryOrdersByStatusID(Request);

            return new JsonResult(new { result = response });
        }
        public IActionResult OnPostExportData([FromBody] HsrpOrderRequest filterdata)
        {
            var dataResponse = _hsrpOrdersServiceRepository.GetHsrporderForExport(filterdata);
            var validColumns = new[] { "OrderNo" };
            filterdata.SortColumn = validColumns.Contains(filterdata.SortColumn) ? filterdata.SortColumn : "OrderNo";
            var orderData = dataResponse.Value as List<HSRPOrderDataExport>;

            if (orderData == null || !orderData.Any())
                return BadRequest("No data to export.");

            var finalData = orderData.Select(s => new
            {
                s.OrderNo,
                OrderDate = s.sOrderDate,
                s.DealerPONo,
                s.DealerSONo,
                s.Dealer,
                s.DealerCode,
                s.DealerCity,
                s.OEM,
                s.OEMCode,
                s.OEMCity,
                s.EmbossingStation,
                s.EmbossingStationCode,
                s.EmbossingStationCity,
                ProcessDate = s.sProcessDate,
                s.RegNo,
                RegDate = s.sRegDate,
                s.EngineNo,
                s.ChasisNo,
                s.PlateColor
            });

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("HSRP Order Data");
                var table = worksheet.Cell(1, 1).InsertTable(finalData);
                worksheet.ShowGridLines = false;

                // Rename columns (0-based)
                table.Field(0).Name = "Order Number";
                table.Field(1).Name = "Order Date";
                table.Field(2).Name = "Dealer PO No";
                table.Field(3).Name = "Dealer SO No";
                table.Field(4).Name = "Dealer";
                table.Field(5).Name = "Dealer Code";
                table.Field(6).Name = "Dealer City";
                table.Field(7).Name = "OEM";
                table.Field(8).Name = "OEM Code";
                table.Field(9).Name = "OEM City";
                table.Field(10).Name = "Embossing Station";
                table.Field(11).Name = "Embossing Station Code";
                table.Field(12).Name = "Embossing Station City";
                table.Field(13).Name = "Process Date";
                table.Field(14).Name = "Reg No";
                table.Field(15).Name = "Reg Date";
                table.Field(16).Name = "Engine No";
                table.Field(17).Name = "Chasis No";
                table.Field(18).Name = "Plate Color";

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string fileName = $"HSRPOrderData-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
        }
    }
}
