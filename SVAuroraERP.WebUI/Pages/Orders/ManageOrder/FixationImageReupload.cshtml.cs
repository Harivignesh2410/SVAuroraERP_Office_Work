namespace SVAuroraERP.WebUI.Pages.Orders.ManageOrder
{
    public class FixationImageReuploadModel : HSRPBasePageModel
    {
        private readonly ILogger<FixationImageReuploadModel> _logger;
        private readonly IFixationImageReuploadServiceRepository _repository;
        private readonly IHSRPOrdersServiceRepository _Orderrepository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly ICreateInvoiceServiceRepository _createInvoiceServiceRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.FixationImageReupload; // ID for this specific page
        public FixationImageReuploadModel(ILogger<FixationImageReuploadModel> logger,
                                        IFixationImageReuploadServiceRepository repository,
                                        IErrorLoggerService errorLoggerService,
                                        ICreateInvoiceServiceRepository createInvoiceServiceRepository, IAuditLogger auditLogger,
                                         IPermissionServiceRepository permissionService,
                                        IAntiforgery antiforgery, IWebHostEnvironment webHostEnvironment, IHSRPOrdersServiceRepository orderrepository)
        {
            _logger = logger;
            _repository = repository;
            _createInvoiceServiceRepository = createInvoiceServiceRepository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
            _webHostEnvironment = webHostEnvironment;
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

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");
            LoggedUserID = (int)HSRPLoggedUser.HSRPUserID;
            if (HSRPLoggedUser.HSRPUserTypeID != (byte)Common.HSRPUserType.EmbossingStation && HSRPLoggedUser.HSRPUserTypeID != (byte)Common.HSRPUserType.Admin)
            {
                return RedirectToPage("/Orders/ViewOrders/ViewFixationImageReupload");
            }
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
        public JsonResult OnGetSummaryForFittedOrder()
        {
            var Request = new SummaryFilterData();
            Request.UserID = (int)HSRPLoggedUser.HSRPUserID;
            Request.OrderStatusID = (int)OrderStatus.FixationReUpload;
            var response = _Orderrepository.SummaryOrdersByStatusID(Request);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnPostFixationImageReuploadListData([FromForm] FixationImageReuploadRequest dataTableRequest)
        {
            try
            {
                dataTableRequest.StartDate = ConvertDateonly(dataTableRequest.sStartDate)?.ToDateTime(TimeOnly.MinValue);
                dataTableRequest.EndDate = ConvertDateonly(dataTableRequest.sEndDate)?.ToDateTime(TimeOnly.MinValue);
                // Validate sort column
                var validColumns = new[] { "OrderNo", "RegNo", "Dealer", "DealerCode", "OEM", "ChasisNo", "EngineNo", "RearLaserSerialNo", "FrontLaserSerialNo", "FrontPlateDimension", "RearPlateDimension" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "OrderNo";

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


                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetFixationImageReuploadOrders(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Create Job Data data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VFixationImageReupload>()
                });
            }
        }
        public JsonResult OnGetHSRPDataByID(int HSRPOrderID)
        {
            DataResponse dataResponse = new DataResponse();

            dataResponse = _repository.GetHsrporderByID(HSRPOrderID);

            return new JsonResult(dataResponse.Value);
        }
        public JsonResult OnPostSaveFittedOrders([FromBody] HSRPVehiclePlateImage request)
        {
            string message = string.Empty;
            request.LastUpdatedBy = (int)HSRPLoggedUser.UserID;
            try
            {
                var dataResponse = _repository.SaveHSRPPlateImage(request);
                return new JsonResult(new
                {
                    dataResponse
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnPostUploadFittedImage(IFormFile file, string OrderYear, string OrderMonth)
        {
            if (file == null || file.Length == 0)
                return new JsonResult(new { success = false, message = "No file uploaded." });

            try
            {
                // Allowed extensions
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var ext = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                    return new JsonResult(new { success = false, message = "Invalid File Type" });

                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "FittedOrders", OrderYear, OrderMonth);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // File name
                string fileName = $"{Guid.NewGuid()}{ext}";
                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Return DB-Safe Path
                string dbPath = $"/uploads/FittedOrders/{OrderYear}/{OrderMonth}/{fileName}";

                return new JsonResult(new
                {
                    success = true,
                    filePath = dbPath,
                    fileName = fileName,
                    year = OrderYear,
                    month = OrderMonth
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}