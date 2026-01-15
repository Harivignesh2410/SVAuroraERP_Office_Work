namespace SVAuroraERP.WebUI.Pages.Orders.ManageOrder
{
    public class FittedOrdersModel : HSRPBasePageModel
    {
        private readonly ILogger<FittedOrdersModel> _logger;
        private readonly ICreateJobCardServiceRepository _createJobCardServiceRepository;
        private readonly IQualityProcessingServiceRepository _qualityProcessingServiceRepository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.HSRRPVehiclePlateImage;
        private readonly IHSRPOrdersServiceRepository _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFittedOrdersServiceRepository _fittedOrdersServiceRepository;
        private readonly IHSRPOrdersServiceRepository _Orderrepository;

        public FittedOrdersModel(ILogger<FittedOrdersModel> logger,
                                        ICreateJobCardServiceRepository createJobCardServiceRepository,
                                        IErrorLoggerService errorLoggerService,
                                        IAuditLogger auditLogger,
                                        IAntiforgery antiforgery,
                                         IPermissionServiceRepository permissionService,
                                        IQualityProcessingServiceRepository qualityProcessingServiceRepository,
                                        IHSRPOrdersServiceRepository repository,
                                        IWebHostEnvironment webHostEnvironment,
                                        IFittedOrdersServiceRepository fittedOrdersServiceRepository,
                                        IHSRPOrdersServiceRepository orderrepository
                                        )
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
            _fittedOrdersServiceRepository = fittedOrdersServiceRepository;
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
                return RedirectToPage("/Orders/ViewOrders/ViewFittedOrders");
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
        public JsonResult OnPostFittedOrderListData([FromForm] FittedOrderRequest dataTableRequest)
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
                dataResponse = _fittedOrdersServiceRepository.GetFittedOrder(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Vehicle Plate Image data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VCreateJobCard>()
                });
            }
        }
        public JsonResult OnGetSummaryForFittedOrder()
        {
            var Request = new SummaryFilterData();
            Request.UserID = (int)HSRPLoggedUser.HSRPUserID;
            Request.OrderStatusID = (int)OrderStatus.FittedOrders;
            var response = _Orderrepository.SummaryOrdersByStatusID(Request);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] SaveFittedOrderRequest request)
        {
            var dataResponse = new DataResponse();

            try
            {
                if (request == null)
                {
                    return new JsonResult(new { success = false, message = "HSRPVehiclePlateImageID cannot be empty" });
                }

                request.LastUpdatedBy = (int)HSRPLoggedUser.UserID;
                request.LastUpdatedDate = DateTime.UtcNow;

                dataResponse = _fittedOrdersServiceRepository.UpdateVehiclePlateStatus(request);

                if (dataResponse.Success)
                {
                    return new JsonResult(new
                    {
                        success = true,
                        jobCardID = dataResponse.ID
                    });
                }

                return new JsonResult(new { success = false, message = dataResponse.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnGetVehicleImageDataByID(int ID)
        {
            DataResponse resultdata = _fittedOrdersServiceRepository.GetVehicleImageData(ID);

            return new JsonResult(resultdata);
        }
    }
}