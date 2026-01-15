namespace SVAuroraERP.WebUI.Pages.Orders.ManageOrder
{
    public class QualityProcessingModel : HSRPBasePageModel
    {
        private readonly ILogger<QualityProcessingModel> _logger;
        private readonly ICreateJobCardServiceRepository _createJobCardServiceRepository;
        private readonly IHSRPOrdersServiceRepository _Orderrepository;
        private readonly IQualityProcessingServiceRepository _qualityProcessingServiceRepository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.QualityProcessing; // ID for this specific page
        private readonly  IHSRPOrdersServiceRepository _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public QualityProcessingModel(ILogger<QualityProcessingModel> logger,
                                        ICreateJobCardServiceRepository createJobCardServiceRepository,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IAntiforgery antiforgery,
                                         IPermissionServiceRepository permissionService,
                                        IQualityProcessingServiceRepository qualityProcessingServiceRepository,
                                        IHSRPOrdersServiceRepository repository,
                                        IHSRPOrdersServiceRepository Orderrepository,
                                        IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _createJobCardServiceRepository = createJobCardServiceRepository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _antiforgery = antiforgery;
            _qualityProcessingServiceRepository = qualityProcessingServiceRepository;
            _permissionrepository = permissionService;
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _Orderrepository = Orderrepository;
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
                return RedirectToPage("/Orders/ViewOrders/ViewQualityProcessing");
            }
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
        public JsonResult OnPostQualityProcessingListData([FromForm] QualityProcessingRequest dataTableRequest)
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
                dataResponse = _qualityProcessingServiceRepository.GetQualityProcessing(dataTableRequest);

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
                    data = new List<VGetReadyforQualityProcessing>()
                });
            }

        }
    
        public JsonResult OnGetSummaryForQualityProcessing()
        {
            var Request = new SummaryFilterData();
            Request.UserID = (int)HSRPLoggedUser.HSRPUserID;
            Request.OrderStatusID = (int)OrderStatus.JobCardGenerated;
            var response = _Orderrepository.SummaryOrdersByStatusID(Request);
            return new JsonResult(new { result = response });
        }

        public JsonResult OnPostUploadQualityImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new JsonResult(new { success = false, message = "No file uploaded." });

            try
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var ext = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                    return new JsonResult(new { success = false, message = "Invalid File Type" });

                // Directory: /wwwroot/Uploads/QualityCheck/
                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "QualityCheck");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fileName = "QC_" + Guid.NewGuid().ToString("N") + ext;
                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // RETURN CORRECT Path to JS
                string dbPath = $"/Uploads/QualityCheck/{fileName}";

                return new JsonResult(new { success = true, filePath = dbPath, fileName = fileName });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }



        public JsonResult OnPostSaveUpdateData([FromBody] QualityProcessRequest request)
        {
            try
            {
                if (request == null)
                    return new JsonResult(new { success = false, message = "OrderIds cannot be empty" });

                request.LastUpdatedBy = (int)HSRPLoggedUser.UserID;

                var dataResponse = _qualityProcessingServiceRepository.Save(request);

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

        public JsonResult OnGetHSRPDataByID(int HSRPOrderID)
        {
            DataResponse dataResponse = new DataResponse();

            dataResponse = _repository.GetHsrporderByID(HSRPOrderID);

            return new JsonResult(dataResponse.Value);
        }
        public JsonResult OnGetRejectQualityProcess(int LaserNoPlateID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                if (LaserNoPlateID <= 0)
                    return new JsonResult(new { success = false, message = "Invalid Laser No Plate ID" });
                int LastUpdatedBy = (int)HSRPLoggedUser.UserID;
              dataResponse = _qualityProcessingServiceRepository.Reject(LaserNoPlateID, LastUpdatedBy);
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

    }
}
