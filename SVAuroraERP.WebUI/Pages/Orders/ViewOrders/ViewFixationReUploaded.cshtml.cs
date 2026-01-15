namespace SVAuroraERP.WebUI.Pages.Orders.ViewOrders
{
    public class ViewFixationReUploadedModel : HSRPBasePageModel
    {
        private readonly ILogger<ViewFixationReUploadedModel> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.ViewFixationReUploaded; // ID for this specific page
        private readonly IFittedOrdersServiceRepository _fittedOrdersServiceRepository;
        private readonly IFixationReUploadedServiceRepository _fixationReUploadedServiceRepository;
        private readonly IHSRPOrdersServiceRepository _Orderrepository;

        public ViewFixationReUploadedModel(ILogger<ViewFixationReUploadedModel> logger,
                                        IAntiforgery antiforgery,
                                         IPermissionServiceRepository permissionService,
                                        IFittedOrdersServiceRepository fittedOrdersServiceRepository,
                                        IFixationReUploadedServiceRepository fixationReUploadedServiceRepository,
                                        IHSRPOrdersServiceRepository orderrepository
                                        )
        {
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
            _fittedOrdersServiceRepository = fittedOrdersServiceRepository;
            _fixationReUploadedServiceRepository = fixationReUploadedServiceRepository;
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
        public JsonResult OnPostFixationReUploadedListData([FromForm] FixationReUploadedRequest dataTableRequest)
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
                dataResponse = _fixationReUploadedServiceRepository.GetFixationReUploaded(dataTableRequest);

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
                    data = new List<VFixationReUploaded>()
                });
            }
        }
        public JsonResult OnGetSummaryForFixationReUploaded()
        {
            var Request = new SummaryFilterData();
            Request.UserID = (int)HSRPLoggedUser.HSRPUserID;
            Request.OrderStatusID = (int)OrderStatus.FixationReUploaded;
            var response = _Orderrepository.SummaryOrdersByStatusID(Request);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetVehicleImageDataByID(int ID)
        {
            DataResponse resultdata = _fixationReUploadedServiceRepository.GetVehicleImageData(ID);

            return new JsonResult(resultdata);
        }
    }
}
