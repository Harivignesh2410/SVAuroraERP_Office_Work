namespace SVAuroraERP.WebUI.Pages.Orders.OrdersDelivery
{
    public class GenerateDeliveryDataModel : BasePageModel
    {
        private readonly ILogger<GenerateDeliveryDataModel> _logger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private readonly IGenerateDeliveryDataServiceRepository _generateDeliveryDataServiceRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private const int PageControlID = (int)Common.Pages.GenerateDeliveryData; // ID for this specific page
        public GenerateDeliveryDataModel(ILogger<GenerateDeliveryDataModel> logger,
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
                var validColumns = new[] { "DealerCode" };

                // Validate and set default sort column
                tableRequest.SortColumn = validColumns.Contains(tableRequest.SortColumn) ? tableRequest.SortColumn : "DealerCode";

                tableRequest.SortDirection = tableRequest.SortDirection?.ToLower() == "asc" ? "asc" : "desc";


                DataResponse dataResponse = new DataResponse();
                dataResponse = _generateDeliveryDataServiceRepository.GetListInvoice(tableRequest);

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
        public JsonResult OnPostOrderList([FromForm] HSRPInvoiceTransByDealerRequest tableRequest)
        {
            try
            {
                var validColumns = new[] { "RegNo" };

                // Validate and set default sort column
                tableRequest.SortColumn = validColumns.Contains(tableRequest.SortColumn) ? tableRequest.SortColumn : "RegNo";

                tableRequest.SortDirection = tableRequest.SortDirection?.ToLower() == "asc" ? "asc" : "desc";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _generateDeliveryDataServiceRepository.GetListInvoiceTrans(tableRequest);

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
     
        public JsonResult OnPostHSRPInvoiceByDealer([FromBody] int dealerId)
        {
            var dataResponse = _generateDeliveryDataServiceRepository.GetHSRPInvoiceTransByDealer(dealerId);
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
        public async Task<JsonResult> OnPostSaveUpdateData(IFormFile UploadImage,[FromForm] GenerateDeliveryRequest request)
        {
            var dataResponse = new DataResponse();

            try
            {
                string imageUrl = null;
                string imageName = null;

                if (UploadImage != null && UploadImage.Length > 0)
                {
                    const long maxFileSize = 10 * 1024 * 1024; // 10MB
                    if (UploadImage.Length > maxFileSize)
                        return new JsonResult(new { success = false, message = "File size exceeds 10MB limit." });

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
                    var fileExtension = Path.GetExtension(UploadImage.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                        return new JsonResult(new { success = false, message = "Invalid file type." });

                    string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "DeliveryImages", request.FK_DealerID.ToString());
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    // ? Use original uploaded file name
                    imageName = Path.GetFileName(UploadImage.FileName);
                    string fullPath = Path.Combine(folderPath, imageName);
                    imageUrl = $"/Uploads/DeliveryImages/{request.FK_DealerID}/{imageName}";

                    // If file already exists, add suffix to avoid overwrite
                    if (System.IO.File.Exists(fullPath))
                    {
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(imageName);
                        string newFileName = $"{fileNameWithoutExt}_{Guid.NewGuid():N}{fileExtension}";
                        fullPath = Path.Combine(folderPath, newFileName);
                        imageUrl = $"/Uploads/DeliveryImages/{request.FK_DealerID}/{newFileName}";
                        imageName = newFileName;
                    }

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await UploadImage.CopyToAsync(stream);
                    }
                }
                request.DispatchDate = (DateOnly)ConvertDateonly(request.sDispatchDate);
                request.LastUpdatedBy = LoggedUser.UserID;
                request.UploadImageUrl = imageUrl;
                request.ImageName = imageName;

                // ?? 3?? Save data
                dataResponse = _generateDeliveryDataServiceRepository.SaveGenerateDeliveryData(request);

                return new JsonResult(new
                {
                    success = dataResponse.ID > 0,
                    message = dataResponse.Message,
                    fileUrl = imageUrl,
                    fileName = imageName
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }


    }
}
