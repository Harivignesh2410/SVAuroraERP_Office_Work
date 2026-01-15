namespace SVAuroraERP.WebUI.Pages.Inventory.Purchase
{
    public class SearchPurchaseEntryModel : BasePageModel
    {
        private readonly IPurchaseEntryServiceRepository _repository = null;
        private readonly ILogger<PendingInspection> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IComponentServiceRepository _componentrepository;
        private readonly ISupplierServiceRepository _supplierrepository;
        private const int PageControlID = (int)Common.Pages.SearchPurchaseEntry;
        private readonly IPermissionServiceRepository _permissionrepository;
        public SearchPurchaseEntryModel(IPurchaseEntryServiceRepository respository,
                           ILogger<PendingInspection> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IComponentServiceRepository componentrepository,
                            ISupplierServiceRepository supplierrepository,
                            IPermissionServiceRepository permissionrepository
                           )
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _componentrepository = componentrepository;
            _supplierrepository = supplierrepository;
            _permissionrepository = permissionrepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> SupplierList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        const string DateFormat = "dd MMM, yyyy";

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadSupplierList();


            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadSupplierList()
        {
            DataResponse dataResponse = new DataResponse();
            SupplierList.Clear();
            dataResponse = _supplierrepository.GetSupplier();
            SupplierList = ((List<VSupplier>)dataResponse.Value)
                .OrderBy(o => o.SupplierName)
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierID.ToString(),
                    Text = s.SupplierName,
                }).ToList();

            SupplierList.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
        }
        public JsonResult OnPostPurchaseEntryByFilter([FromBody] SearchPurchaseEntryFilter SearchPurchaseEntryFilter)
        {
            if (!string.IsNullOrEmpty(SearchPurchaseEntryFilter.sStartDate)) SearchPurchaseEntryFilter.StartDate = (DateTime)ConvertDate(SearchPurchaseEntryFilter.sStartDate);
            if (!string.IsNullOrEmpty(SearchPurchaseEntryFilter.sEndDate)) SearchPurchaseEntryFilter.EndDate = (DateTime)ConvertDate(SearchPurchaseEntryFilter.sEndDate);
            var resultdata = _repository.GetPurchaseEntryByFilter(SearchPurchaseEntryFilter);
            return new JsonResult(new { success = true, data = resultdata });
        }
        private DateTime? ConvertDate(string sdate)
        {

            DateTime? dtConvertedDate = null;

            if (DateTime.TryParseExact(sdate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDt))
            {
                dtConvertedDate = parsedDt;
            }

            return dtConvertedDate;
        }
    }
}
