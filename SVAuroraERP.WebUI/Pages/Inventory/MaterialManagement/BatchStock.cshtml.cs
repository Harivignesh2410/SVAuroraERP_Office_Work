namespace SVAuroraERP.WebUI.Pages.Inventory.MaterialManagement
{
    public class BatchStockModel : BasePageModel
    {
        private readonly IPurchaseEntryServiceRepository _repository = null;
        private readonly IPendingInspectionServiceRepository _materialRepository = null;
        private readonly ILogger<PendingInspection> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IComponentServiceRepository _componentrepository;
        private readonly ISizeServiceRepository _sizeservicerepository;
        private readonly IColorServiceRespository _colorServiceRespository;
        private readonly IRackLocationServiceRepository _RackLocationServiceRepository;
        private readonly IWareHouseServiceRepository _warehouseServiceRepository;
        private const int PageControlID = (int)Common.Pages.BatchStock;
        private readonly IPermissionServiceRepository _permissionrepository;

        public BatchStockModel(IPurchaseEntryServiceRepository respository,
                           ILogger<PendingInspection> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IPendingInspectionServiceRepository materialRepository,
                           IComponentServiceRepository componentrepository,
                           ISizeServiceRepository sizeservicerepository,
                           IColorServiceRespository colorServiceRespository,
                           IRackLocationServiceRepository rackLocationServiceRepository,
                           IWareHouseServiceRepository warehouseServiceRepository,
                           IPermissionServiceRepository permissionrepository)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _materialRepository = materialRepository;
            _componentrepository = componentrepository;
            _sizeservicerepository = sizeservicerepository;
            _colorServiceRespository = colorServiceRespository;
            _RackLocationServiceRepository = rackLocationServiceRepository;
            _warehouseServiceRepository = warehouseServiceRepository;
            _permissionrepository = permissionrepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> ComponentList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SizeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ColorList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> RackLocationList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> WarehouseList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
  
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadComponentList();
            LoadSizeList();
            LoadColorList();
            LoadRackLocationList();
            LoadWarehouseList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadComponentList()
        {
            DataResponse dataResponse = null;
            ComponentList.Clear();
            dataResponse = _componentrepository.GetComponentList();
            ComponentList = ((List<VComponentType>)dataResponse.Value)
                            .OrderBy(o => o.ComponentTypeName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.ComponentTypeID.ToString(),
                                 Text = s.ComponentTypeName
                             }).ToList();

            ComponentList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Component--" });
        }
        public void LoadSizeList()
        {DataResponse dataResponse = null;
            SizeList.Clear();
            dataResponse = _sizeservicerepository.GetSize();
            SizeList= ((List<VSize>)dataResponse.Value)
                            .OrderBy(o => o.SizeName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.SizeID.ToString(),
                                 Text = s.SizeName
                             }).ToList();

            SizeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Size--" });
        }
        public void LoadColorList()
        {
            DataResponse dataResponse = null;
            ColorList.Clear();
            dataResponse = _colorServiceRespository.GetColor();
            ColorList = ((List<VColor>)dataResponse.Value).
                            OrderBy(o => o.ColorName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.ColorID.ToString(),
                                 Text = s.ColorName
                             }).ToList();

            ColorList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Color--" });
        }
        public void LoadRackLocationList()
        {
            DataResponse dataResponse = null;
            RackLocationList.Clear();
            dataResponse = _RackLocationServiceRepository.GetRackLocation();
            RackLocationList=((List<VRackLocation>)dataResponse.Value)
                            .OrderBy(o => o.RackLocationName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.RackLocationID.ToString(),
                                 Text = s.RackLocationName
                             }).ToList();

            RackLocationList.Insert(0, new SelectListItem { Value = "0", Text = "--Select RackLocation--" });
        }
        public void LoadWarehouseList()
        {
            DataResponse dataResponse =null;
            WarehouseList.Clear();
            dataResponse = _warehouseServiceRepository.GetWareHouse();
            WarehouseList=((List<VWareHouse>)dataResponse.Value)
                            .OrderBy(o => o.WareHouseName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.WareHouseID.ToString(),
                                 Text = s.WareHouseName
                             }).ToList();

            WarehouseList.Insert(0, new SelectListItem { Value = "0", Text = "--Select WareHouse--" });
        }
        public JsonResult OnPostBatckStocktByFilter([FromBody] FilterForBatchStock FilterForBatchStock)
        {
            var resultdata = _materialRepository.GetCompletedBatchStock(FilterForBatchStock);
            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnGetRackLocationByWareHouseID(int WareHouseID)
        {
            var resultdata = _RackLocationServiceRepository.GetRackLocationByWareHouseID(WareHouseID);
            return new JsonResult(new { success = true, data = resultdata });
        }
    }
}