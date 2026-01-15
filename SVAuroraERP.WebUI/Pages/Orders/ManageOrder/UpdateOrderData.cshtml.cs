namespace SVAuroraERP.WebUI.Pages.Orders.ManageOrder
{
    public class UpdateOrderDataModel : HSRPBasePageModel
    {
        private readonly IUpdateOrderDataServiceRepository _repository;
        private readonly ILogger<VCreateJobCard> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionRepository;
        private const int PageControlID = (int)Common.Pages.UpdateOrderData;

        public UpdateOrderDataModel(
            IUpdateOrderDataServiceRepository repository,
            ILogger<VCreateJobCard> logger,
            IAntiforgery antiforgery,
            SessionService sessionService,
            IPermissionServiceRepository permissionRepository)
        {
            _repository = repository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionRepository = permissionRepository;
        }

        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }

        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken;
            Permissions = _permissionRepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);

            //if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");
            //return Page();
        }

        public JsonResult OnGetOrderDetailsById(int ID)
        {
            var response = _repository.GetByID(ID);
            var usertypeid = (int)HSRPLoggedUser.HSRPUserTypeID;

            return new JsonResult(new
            {
                Data = response,
                UserTypeID = usertypeid
            });
        }
        private DateOnly? ConvertDateonly(string sdate)
        {
            DateOnly? dtConvertedDate = null;
           // const string DateFormat = "dd-MM-yyyy";

            const string DateFormat = "dd/MM/yyyy"; // Matches "28/04/2025"

            if (DateOnly.TryParseExact(sdate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsedDt))
            {
                dtConvertedDate = parsedDt;
            }

            return dtConvertedDate;
        }

        public JsonResult OnPostSaveUpdateData([FromBody] LaserNoUpdateRequest request)
        {
            DataResponse result = new DataResponse();
            try
            {
                request.OrderDate = (DateOnly)ConvertDateonly(request.sOrderDate);
                request.LastUpdatedBy = (int)HSRPLoggedUser.UserID;
                if (request.HSRPOrderID > 0)
                    result = _repository.SaveLaserNoForOrder(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");
            }
            return new JsonResult(result);
        }
        public JsonResult OnGetLaserNoByPartNo(string PartNo)
        {
            var response = _repository.GetLaserNoByPartNo(PartNo);
            return new JsonResult(response);
        }

        public JsonResult OnPostSaveandUpdateRectifyOrder([FromBody] RectifyLaserPlate request)
        {
            DataResponse result = new DataResponse();
            try
            {

                request.LastUpdatedBy = (int)HSRPLoggedUser.UserID;
                request.LastUpdatedDate = DateTime.UtcNow;
                result = _repository.SaveRectification(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");
            }
            return new JsonResult(result);
        }
    }
}
