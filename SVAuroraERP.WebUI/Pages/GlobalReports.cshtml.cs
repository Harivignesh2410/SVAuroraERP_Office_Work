namespace SVAuroraERP.WebUI.Pages
{
    public class GlobalReportsModel : BasePageModel
    {
        private readonly IGlobalConfigServiceRepository _globalConfigServiceRepository;
        private readonly IHSRPOrdersServiceRepository _hSRPOrdersServiceRepository;
        private readonly ICreateJobCardServiceRepository _createJobCardServiceRepository;
        public GlobalReportsModel(IGlobalConfigServiceRepository globalConfigServiceRepository,
                                  IHSRPOrdersServiceRepository hSRPOrdersServiceRepository,
                                  ICreateJobCardServiceRepository createJobCardServiceRepository)
        {
            _globalConfigServiceRepository = globalConfigServiceRepository;
            _hSRPOrdersServiceRepository = hSRPOrdersServiceRepository;
            _createJobCardServiceRepository = createJobCardServiceRepository;
        }
        public void OnGet()
        {
        }

        private async Task<IActionResult> FetchPdfReport(string reportName, string endpointPath, int FilterByID = 0)
        {
            try
            {
               // var ReportApiURL = "https://localhost:44315/";
                var config = await _globalConfigServiceRepository.GetGlobalConfig();
                if (config == null) return BadRequest("Configuration not found.");

                string pdfSourceUrl = $"{config.ReportApiURL}/{endpointPath}";
               // string pdfSourceUrl = $"{ReportApiURL}/{endpointPath}";

                if (FilterByID != 0) pdfSourceUrl = pdfSourceUrl + $"/{FilterByID}";

                // Add APIKey as a query parameter
                var apiKey = config.ReportAPIKey;
                if (!string.IsNullOrEmpty(apiKey))
                {
                    var separator = pdfSourceUrl.Contains("?") ? "&" : "?";
                    pdfSourceUrl = $"{pdfSourceUrl}{separator}APIKey={Uri.EscapeDataString(apiKey)}";
                }

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(5);
                    var result = await client.GetAsync(pdfSourceUrl);

                    if (!result.IsSuccessStatusCode)
                        return BadRequest($"Failed to fetch PDF from report service. Status: {result.StatusCode}");

                    var pdfBytes = await result.Content.ReadAsByteArrayAsync();

                    if (pdfBytes == null || pdfBytes.Length == 0)
                        return BadRequest("Empty PDF received from report service.");

                    Response.Headers.Add("Content-Disposition", $"inline; filename=\"{reportName}\"");
                    Response.Headers.Add("Content-Type", "application/pdf");

                    return File(pdfBytes, "application/pdf", reportName);
                }
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(500, $"Network error while fetching PDF: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public async Task<IActionResult> OnGetOrderReport(int OrderID)
        {
            DataResponse dataResponse = new DataResponse();
            dataResponse = _hSRPOrdersServiceRepository.GetHsrporderByID(OrderID);
            var OderNO = dataResponse.Value as VHSRPOrder;
            string fileName = $"{OderNO.OrderNo}_{OderNO.sOrderDate}.pdf";
           // string fileName = $"HSrpOrder.pdf";
            try
            {
                var result = await FetchPdfReport(fileName, "GetOrderReport", OrderID);
                return result;
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                    //Message = _errorLoggerService.LogException(ex, JournalEntryID, "GlobalReportsModel.OnGetJournalEntryReportByID")
                });
            }
        }
        public async Task<IActionResult> OnGetTLPSticker(int OrderID)
        {
            DataResponse dataResponse = new DataResponse();
            //dataResponse = _hSRPOrdersServiceRepository.GetHsrporderByID(OrderID);
            //var OderNO = dataResponse.Value as VHSRPOrder;
           // string fileName = $"{OderNO.OrderNo}_TLPSticker.pdf";
            string fileName = "TLPSticker.pdf";
            try
            {
                var result = await FetchPdfReport(fileName, "GetTLPSticker", OrderID);
                return result;
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                });
            }
        }
        public async Task<IActionResult> OnGetJobcardReport(int OrderID)
        {
            DataResponse dataResponse = new DataResponse();
            dataResponse = _createJobCardServiceRepository.GetJobcardByID(OrderID);
            var JobCard = dataResponse.Value as VHSRPJobCard;
            string datastramp = DateTime.Now.ToString("ddMMyyyyHHmm");
            string fileName = $"{JobCard.JobCardNo}_{datastramp}.pdf";
            try
            {
                var result = await FetchPdfReport(fileName, "GetJobcardReport", OrderID);
                return result;
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                });
            }
        }
        public async Task<IActionResult> OnGetGenerateInvoice(int InvoiceID)
        {
            DataResponse dataResponse = new DataResponse();
            //dataResponse = _createJobCardServiceRepository.GetJobcardByID(InvoiceID);
            //var JobCard = dataResponse.Value as VHSRPJobCard;
            string datastramp = DateTime.Now.ToString("ddMMyyyyHHmm");
            string fileName = $"Invoice_{datastramp}.pdf";
            try
            {
                var result = await FetchPdfReport(fileName, "GetInvoiceReport", InvoiceID);
                return result;
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                });
            }
        }
    }
}