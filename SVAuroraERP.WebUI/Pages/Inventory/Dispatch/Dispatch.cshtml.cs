namespace SVAuroraERP.WebUI.Pages.Inventory.Dispatch
{
    public class DispatchModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private readonly ILogger<DispatchModel> logger = null;
       //private const int PageControlID = (int)Common.Pages.Dispatch; // ID for this specific page
        private readonly ICourierServiceRepository _courierServiceRepository;
        private readonly IPackingServiceRepository _packingServiceRepository;
        public DispatchModel(IAntiforgery antiforgery,
                            ICourierServiceRepository courierServiceRepository,
                            IPackingServiceRepository packingServiceRepository)
        {
            _antiforgery = antiforgery;
            _courierServiceRepository = courierServiceRepository;
            _packingServiceRepository = packingServiceRepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> CourierList { get; set; } = new List<SelectListItem>();
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            LoadCourierList();
        }
        public void LoadCourierList()
        {
            DataResponse dataResponse = null;

            CourierList.Clear();
            dataResponse = _courierServiceRepository.GetCourier();
            CourierList = ((List<VCourier>)dataResponse.Value)
                .OrderBy(o => o.CourierName)
                .Select(s => new SelectListItem
                {
                    Value = s.CourierID.ToString(),
                    Text = s.CourierName
                }).ToList();

            CourierList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Courier--" });
        }
        public JsonResult OnGetListData()
        {
            var resultdata = _packingServiceRepository.GetPackingList();

            return new JsonResult(new { success = true, data = resultdata });
        }
    }
}