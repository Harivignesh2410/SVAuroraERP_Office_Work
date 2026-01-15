namespace SVAuroraERP.WebUI.Pages.Inventory.MaterialManagement
{
    public class StockReportModel : BasePageModel
    {
        private readonly IPurchaseEntryServiceRepository _repository = null;
        private readonly IPendingInspectionServiceRepository _materialRepository = null;
        private readonly ILogger<PendingInspection> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IItemServiceRepository _itemRepository = null;
        private readonly IComponentServiceRepository _componentrepository;
        public StockReportModel(IPurchaseEntryServiceRepository respository,
                           ILogger<PendingInspection> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IPendingInspectionServiceRepository materialRepository,
                           IComponentServiceRepository componentrepository,
                           IItemServiceRepository itemRepository)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _materialRepository = materialRepository;
            _componentrepository = componentrepository;
            _itemRepository = itemRepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> ItemList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ComponentList { get; set; } = new List<SelectListItem>();
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            LoadSupplierList();
            LoadComponentList();
        }
        public void LoadSupplierList()
        {
            DataResponse dataResponse = null;
            ItemList.Clear();
            dataResponse= _itemRepository.GetItem();
            ItemList=((List<VItem>)dataResponse.Value)
                .OrderBy(o => o.ItemName)
                .Select(s => new SelectListItem
                {
                    Value = s.ItemID.ToString(),
                    Text = s.ItemName,
                }).ToList();

            ItemList.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
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

            ComponentList.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
        }
        public JsonResult OnPostStockReportByFilter([FromBody] SearchPendingInwardFilter SearchPendingPurchase)
        {
            var resultdata = _materialRepository.GetPendingInspectionByFilter(SearchPendingPurchase);
            return new JsonResult(new { success = true, data = resultdata });
        }
    }
}
