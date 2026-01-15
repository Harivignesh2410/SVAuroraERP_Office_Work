using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SVAuroraERP.WebUI.Pages.Orders.ManageOrder;

namespace SVAuroraERP.WebUI.Pages.Orders.Invoice
{
    public class ExportInvoiceModel : HSRPBasePageModel
    {
        private readonly ILogger<ExportInvoiceModel> _logger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.ExportInvoice;
        private readonly IListInvoiceServiceRepository _listInvoiceServiceRepository;

        public ExportInvoiceModel(ILogger<ExportInvoiceModel> logger,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IAntiforgery antiforgery,
                                         IPermissionServiceRepository permissionService,
                                        IListInvoiceServiceRepository listInvoiceServiceRepository
                                        )
        {
            _logger = logger;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
            _listInvoiceServiceRepository = listInvoiceServiceRepository;
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
        public JsonResult OnPostExportInvoice([FromForm] ExportInvoiceRequest dataTableRequest)
        {
            try
            {
                dataTableRequest.StartDate = ConvertDateonly(dataTableRequest.sStartDate)?.ToDateTime(TimeOnly.MinValue);
                dataTableRequest.EndDate = ConvertDateonly(dataTableRequest.sEndDate)?.ToDateTime(TimeOnly.MinValue);

                var validColumns = new[] { "DealerCode", "InvoiceNo", "Dealer", "DealerPONo", "PartNo", "RegNo", "FrontLaserSerialNo", "RearLaserSerialNo", "PlateColor", "OEM" };

                // Validate and set default sort column
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "DealerCode";

                dataTableRequest.SortDirection = dataTableRequest.SortDirection?.ToLower() == "asc" ? "asc" : "desc";

             //   dataTableRequest.HsrpUserID = HSRPLoggedUser.HSRPUserID;

                DataResponse dataResponse = new DataResponse();
                dataResponse = _listInvoiceServiceRepository.GetExportInvoiceList(dataTableRequest);

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

        public IActionResult OnPostExportNormalExcel([FromBody] ExportInvoiceRequest filter)
        {
            filter.StartDate = ConvertDateonly(filter.sStartDate)
                ?.ToDateTime(TimeOnly.MinValue);

            filter.EndDate = ConvertDateonly(filter.sEndDate)
                ?.ToDateTime(TimeOnly.MinValue);

            var response = _listInvoiceServiceRepository.GetExportInvoiceExcel(filter);
            var list = response.Value as List<VExportInvoiceList>;

            if (list == null || !list.Any())
                return BadRequest("No data to export");

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Invoice");

            // ---------------- HEADERS ----------------
            ws.Cell(1, 1).Value = "Invoice No";
            ws.Cell(1, 2).Value = "Invoice Date";
            ws.Cell(1, 3).Value = "Dealer";
            ws.Cell(1, 4).Value = "Dealer PO No";
            ws.Cell(1, 5).Value = "Part No";
            ws.Cell(1, 6).Value = "Qty";
            ws.Cell(1, 7).Value = "Reg No";
            ws.Cell(1, 8).Value = "Front Laser No";
            ws.Cell(1, 9).Value = "Rear Laser No";
            ws.Cell(1, 10).Value = "Plate Color";
            ws.Cell(1, 11).Value = "OEM";

            int row = 2;

            // ---------------- DATA ----------------
            foreach (var item in list)
            {
                ws.Cell(row, 1).Value = item.InvoiceNo;
                ws.Cell(row, 2).Value = item.InvoiceDate?.ToString("dd/MM/yyyy");
                ws.Cell(row, 3).Value = item.Dealer;
                ws.Cell(row, 4).Value = item.DealerPONo;
                ws.Cell(row, 5).Value = item.PartNo;
                ws.Cell(row, 6).Value = item.Qty;
                ws.Cell(row, 7).Value = item.RegNo;
                ws.Cell(row, 8).Value = item.FrontLaserSerialNo;
                ws.Cell(row, 9).Value = item.RearLaserSerialNo;
                ws.Cell(row, 10).Value = item.PlateColor;
                ws.Cell(row, 11).Value = item.OEM;

                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Invoice-{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }




        public IActionResult OnPostExportHSRPExcel([FromBody] ExportInvoiceRequest filter)
        {
            filter.StartDate = ConvertDateonly(filter.sStartDate)
                ?.ToDateTime(TimeOnly.MinValue);

            filter.EndDate = ConvertDateonly(filter.sEndDate)
                ?.ToDateTime(TimeOnly.MinValue);

            var response = _listInvoiceServiceRepository.GetExportInvoiceExcel(filter);
            var list = response.Value as List<VExportInvoiceList>;

            if (list == null || !list.Any())
                return BadRequest("No data to export");

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("HSRP");

            // ✅ HEADERS (EXACT MATCH WITH TEMPLATE)
            ws.Cell(1, 1).Value = "VENDOR_INV_NO";
            ws.Cell(1, 2).Value = "VENDOR_INV_DATE";
            ws.Cell(1, 3).Value = "TVS_PO_NO";
            ws.Cell(1, 4).Value = "MATERIAL_NO";
            ws.Cell(1, 5).Value = "VENDOR_QUANTITY";
            ws.Cell(1, 6).Value = "HSRP_EHV_REG_NO";
            ws.Cell(1, 7).Value = "HSRP_F";
            ws.Cell(1, 8).Value = "HSRP_B";
            ws.Cell(1, 9).Value = "HSRP_COLOUR";

            int row = 2;

            foreach (var item in list)
            {
                ws.Cell(row, 1).Value = item.InvoiceNo;                          // VENDOR_INV_NO
                ws.Cell(row, 2).Value = item.InvoiceDate?.ToString("dd.MM.yyyy"); // VENDOR_INV_DATE
                ws.Cell(row, 3).Value = item.DealerPONo;                          // TVS_PO_NO
                ws.Cell(row, 4).Value = item.PartNo;                              // MATERIAL_NO
                ws.Cell(row, 5).Value = item.Qty;                                 // VENDOR_QUANTITY
                ws.Cell(row, 6).Value = item.RegNo;                               // HSRP_EHV_REG_NO
                ws.Cell(row, 7).Value = item.FrontLaserSerialNo;                  // HSRP_F
                ws.Cell(row, 8).Value = item.RearLaserSerialNo;                   // HSRP_B
                ws.Cell(row, 9).Value = item.PlateColor;                          // HSRP_COLOUR

                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"HSRP_Excel-{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }






    }
}


