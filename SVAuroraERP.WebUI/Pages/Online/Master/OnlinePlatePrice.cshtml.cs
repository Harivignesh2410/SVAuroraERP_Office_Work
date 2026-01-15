namespace SVAuroraERP.WebUI.Pages.Online.Master
{
    public class OnlinePlatePriceModel : BasePageModel
    {
        private readonly IOnlinePlatePriceServiceRepository _repository = null;
        private readonly IVehicleClassServiceRepository _vehicleclassSrepository = null;
        private readonly IVehiclePlateColorServiceRepository _vehicleplatecolorrepository = null;
        private readonly IVehiclePlateSizeServiceRepository _vehicleplatesizerepository = null;
        private readonly ILogger<IOnlinePlatePriceServiceRepository> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.OnlinePlatePrice; // ID for this specific page
        public OnlinePlatePriceModel(IOnlinePlatePriceServiceRepository respository,
            IVehicleClassServiceRepository vehicleclassSrepository,
            IVehiclePlateColorServiceRepository vehicleplatecolorrepository,
            IVehiclePlateSizeServiceRepository vehicleplatesizerepository,
                           ILogger <IOnlinePlatePriceServiceRepository> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                            IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _vehicleclassSrepository = vehicleclassSrepository;
            _vehicleplatecolorrepository = vehicleplatecolorrepository;
            _vehicleplatesizerepository = vehicleplatesizerepository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> VehicleClassList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VehiclePlateColorList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VehiclePlateSizeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> FuelList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VehicleCategoryList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VehicleTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VehiclePlateTypeList { get; set; } = new List<SelectListItem>();
   
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadVehicleClassList();
            LoadVehiclePlateColorList();
            LoadVehiclePlateSizeList();
            LoadVehicleCategoryList();
            LoadVehicleTypeList();
            LoadVehiclePlateTypeList();
            LoadFuelList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadVehicleClassList()
        {
            var dataResponse = _vehicleclassSrepository.GetVehicleClass();

            VehicleClassList.Clear();
            var VehicleClass = dataResponse.Value as List<VVehicleClass>;
            if (VehicleClass != null)
            {
                VehicleClassList = VehicleClass
                .Select(s => new SelectListItem
                {
                    Value = s.VehicleClassID.ToString(),
                    Text = s.VehicleClassName
                }).ToList();
            }
            VehicleClassList.Insert(0, new SelectListItem { Value = "0", Text = "--Select VehicleClass--" });
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
            VehiclePlateSizeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Vehicle Plate Color--" });
        }
        public void LoadFuelList()
        {
            var dataResponse = _repository.GetFuel();

            FuelList.Clear();
            var Fuel = dataResponse.Value as List<Fuel>;
            if (Fuel != null)
            {
                FuelList = Fuel
                .Select(s => new SelectListItem
                {
                    Value = s.FuelID.ToString(),
                    Text = s.FuelName
                }).ToList();
            }
            FuelList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Fuel--" });
        }
        public void LoadVehicleCategoryList()
        {
            var dataResponse = _repository.GetVehicleCategory();

            VehicleCategoryList.Clear();
            var VehicleCategory = dataResponse.Value as List<VehicleCategory>;
            if (VehicleCategory != null)
            {
                VehicleCategoryList = VehicleCategory
                .Select(s => new SelectListItem
                {
                    Value = s.VehicleCategoryID.ToString(),
                    Text = s.VehicleCategoryName
                }).ToList();
            }
            VehicleCategoryList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Vehicle Category--" });
        }
        public void LoadVehicleTypeList()
        {
            var dataResponse = _repository.GetVehicleType();

            VehicleTypeList.Clear();
            var VehiclePlateType = dataResponse.Value as List<VehicleType>;
            if (VehiclePlateType != null)
            {
                VehicleTypeList = VehiclePlateType
                .Select(s => new SelectListItem
                {
                    Value = s.VehicleTypeID.ToString(),
                    Text = s.VehicleTypeName
                }).ToList();
            }
            VehicleTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Vehicle Type--" });
        }
        public void LoadVehiclePlateTypeList()
        {
            var dataResponse = _repository.GetVehiclePlateType();

            VehiclePlateTypeList.Clear();
            var VehiclePlateType = dataResponse.Value as List<VehiclePlateType>;
            if (VehiclePlateType != null)
            {
                VehiclePlateTypeList = VehiclePlateType
                .Select(s => new SelectListItem
                {
                    Value = s.VehiclePlateTypeID.ToString(),
                    Text = s.VehiclePlateTypeName
                }).ToList();
            }
            VehiclePlateTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Vehicle Plate Type--" });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] OnlinePlatePrice OnlinePlatePrice)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                OnlinePlatePrice.LastUpdatedBy = LoggedUser.UserID;

                if (OnlinePlatePrice.OnlinePlatePriceID == 0)
                    resultdata = _repository.Save(OnlinePlatePrice);
                else if (OnlinePlatePrice.OnlinePlatePriceID > 0)
                    resultdata = _repository.Update(OnlinePlatePrice);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetOnlinePlatePriceByID(int ID)
        {
            DataResponse resultdata = _repository.GetOnlinePlatePriceByID(ID);

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
        public JsonResult OnGetLoadSizeByClassAndPlateTypeID(int classID, int plateTypeID)
        {
            // Call repository with both params
            DataResponse dataResponse = _repository.GetSizeByPlateTypeID(classID, plateTypeID);

            var sizeList = dataResponse.Value as List<VVehiclePlateSizeMapping>;

            return new JsonResult(new { result = sizeList });
        }

        public JsonResult OnGetLoadPlateTypeByPlateTypeID(int ID)
        {
            DataResponse dataResponse = _repository.GetPlateTypeByVehicleClassID(ID);
            var PlateTypeList = dataResponse.Value as List<VVehiclePlateSizeMapping>;

            return new JsonResult(new { result = PlateTypeList });
        }
        public JsonResult OnPostOnlinePlatePriceList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "VehicleCategoryName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "VehicleCategoryName";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetOnlinePlatePriceDataTableList(dataTableRequest);

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
                _logger.LogError(ex, "Error loading OEM Pricing data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VHSRPReplacementReason>()
                });
            }
        }
    }
}