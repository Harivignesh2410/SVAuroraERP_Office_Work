namespace SVAuroraERP.WebUI.Pages.Online.Master
{
    public class HSRPPlateDimensionModel : BasePageModel
    {
        private readonly IHSRPPlateDimensionServiceRepository _repository = null;
        private readonly IVehiclePlateColorServiceRepository _vehicleplatecolorrepository = null;
        private readonly IVehiclePlateSizeServiceRepository _vehicleplatesizerepository = null;
        private readonly ILogger<IOnlinePlatePriceServiceRepository> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.HSRPPlateDimension; // ID for this specific page

        public HSRPPlateDimensionModel(IHSRPPlateDimensionServiceRepository respository,
            IVehiclePlateColorServiceRepository vehicleplatecolorrepository,
            IVehiclePlateSizeServiceRepository vehicleplatesizerepository,
                           ILogger<IOnlinePlatePriceServiceRepository> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                                IPermissionServiceRepository permissionService
            )
        {
            _repository = respository;
            _vehicleplatecolorrepository = vehicleplatecolorrepository;
            _vehicleplatesizerepository = vehicleplatesizerepository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> VehiclePlateColorList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VehiclePlateSizeList { get; set; } = new List<SelectListItem>();
 
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadVehiclePlateColorList();
            LoadVehiclePlateSizeList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadVehiclePlateColorList()
        {
            var dataResponse = _vehicleplatecolorrepository.GetVehiclePlateColor();

            VehiclePlateColorList.Clear();
            var VehiclePlateColor = dataResponse.Value as List<VVehiclePlateColor>;
            if (VehiclePlateColor != null)
            {
                VehiclePlateColorList = VehiclePlateColor
                .Select(s => new SelectListItem
                {
                    Value = s.VehiclePlateColorID.ToString(),
                    Text = s.VehiclePlateColorName
                }).ToList();
            }
            VehiclePlateColorList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Vehicle Plate Color--" });
        }
        public void LoadVehiclePlateSizeList()
        {
            var dataResponse = _vehicleplatesizerepository.GetVehiclePlateSize();

            VehiclePlateSizeList.Clear();
            var VehiclePlateSize = dataResponse.Value as List<VVehiclePlateSize>;
            if (VehiclePlateSize != null)
            {
                VehiclePlateSizeList = VehiclePlateSize
                .Select(s => new SelectListItem
                {
                    Value = s.VehiclePlateSizeID.ToString(),
                    Text = s.VehiclePlateSizeName
                }).ToList();
            }
            VehiclePlateSizeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select VehiclePlate Size--" });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] HSRPPlateDimension HSRPPlateDimension)
         {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                HSRPPlateDimension.LastUpdatedBy = LoggedUser.UserID;

                if (HSRPPlateDimension.HSRPPlateDimensionID == 0)
                    resultdata = _repository.Save(HSRPPlateDimension);
                else if (HSRPPlateDimension.HSRPPlateDimensionID > 0)
                    resultdata = _repository.Update(HSRPPlateDimension);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetHSRPPlateDimensionByID(int ID)
        {
            DataResponse resultdata = _repository.GetHSRPPlateDimensionByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();

            try
            {
                dataResponse = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata = dataResponse });
            }
        }
        public JsonResult OnPostHSRPPlateDimensionList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "VehiclePlateSizeName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "VehiclePlateSizeName";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetHSRPPlateDimensionDataTableList(dataTableRequest);

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = dataResponse.recordsTotal,
                    recordsFiltered = dataResponse.recordsFiltered,
                    data = dataResponse.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading HSRP Plate Dimension data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VHSRPReplacementDocument>()
                });
            }
        }
    }
}
