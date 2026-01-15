namespace SVAuroraERP.WebUI.Pages.Inventory.MaterialManagement
{
    public class ComponentStockModel : BasePageModel
    {
        private readonly IPurchaseEntryServiceRepository _repository = null;
        private readonly IPendingInspectionServiceRepository _materialRepository = null;
        private readonly ILogger<PendingInspection> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IComponentServiceRepository _componentrepository;
        private readonly ISizeServiceRepository _sizeservicerepository;
        private readonly IColorServiceRespository _colorServiceRespository;
        private readonly IProcessTypeServiceRepository _processTypeServiceRepository;
        private const int PageControlID = (int)Common.Pages.ComponenetStock;
        private readonly IPermissionServiceRepository _permissionrepository;
        public ComponentStockModel(IPurchaseEntryServiceRepository respository,
                           ILogger<PendingInspection> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IPendingInspectionServiceRepository materialRepository,
                           IComponentServiceRepository componentrepository,
                           ISizeServiceRepository sizeservicerepository,
                           IColorServiceRespository colorServiceRespository,
                           IProcessTypeServiceRepository processTypeServiceRepository,
                           IPermissionServiceRepository permissionrepository)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _materialRepository = materialRepository;
            _componentrepository = componentrepository;
            _sizeservicerepository = sizeservicerepository;
            _colorServiceRespository = colorServiceRespository;
            _processTypeServiceRepository = processTypeServiceRepository;
            _permissionrepository = permissionrepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> ComponentList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SizeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ColorList { get; set; } = new List<SelectListItem>();
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
        {
            DataResponse dataResponse = new DataResponse();
            SizeList.Clear();
            dataResponse = _sizeservicerepository.GetSize();
            SizeList = ((List<VSize>)dataResponse.Value)
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
        public JsonResult OnPostBatckStocktByFilter([FromBody] FilterForBatchStock FilterForBatchStock)
        {
            var resultdata = _materialRepository.GetComponenetStock(FilterForBatchStock);
            return new JsonResult(new { success = true, data = resultdata });
        }
    }
}
