namespace SVAuroraERP.WebUI.Pages.Inventory.MaterialManagement
{
    public class CompletedInspectionModel : BasePageModel
    {
        private readonly IPurchaseEntryServiceRepository _repository = null;
        private readonly IPendingInspectionServiceRepository _materialRepository = null;
        private readonly ILogger<CompletedInspectionModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly ISupplierServiceRepository _Supplierrepository = null;
        private readonly IComponentServiceRepository _componentrepository;
        private const int PageControlID = (int)Common.Pages.CompletedInspection;
        private readonly IPermissionServiceRepository _permissionrepository;
        public CompletedInspectionModel(IPurchaseEntryServiceRepository respository,
                           ILogger<CompletedInspectionModel> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           ISupplierServiceRepository supplierrepository,
                           IPendingInspectionServiceRepository materialRepository,
                           IComponentServiceRepository componentrepository,
                            IPermissionServiceRepository permissionrepository)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _Supplierrepository = supplierrepository;
            _materialRepository = materialRepository;
            _componentrepository = componentrepository;
            _permissionrepository = permissionrepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> SupplierList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ComponentList { get; set; } = new List<SelectListItem>();
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
            LoadComponentList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadSupplierList()
        {
            DataResponse dataResponse = new DataResponse();
            SupplierList.Clear();
            dataResponse = _Supplierrepository.GetSupplier();
            SupplierList=((List<VSupplier>)dataResponse.Value)
                .OrderBy(o => o.SupplierName)
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierID.ToString(),
                    Text = s.SupplierName,
                }).ToList();

            SupplierList.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
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
        public JsonResult OnPostPendingPurchaseEntryByFilter([FromBody] SearchPendingPurchase SearchPendingPurchase)
        {
            if (!string.IsNullOrEmpty(SearchPendingPurchase.sStartDate)) SearchPendingPurchase.StartDate = (DateTime)ConvertDate(SearchPendingPurchase.sStartDate);
            if (!string.IsNullOrEmpty(SearchPendingPurchase.sEndDate)) SearchPendingPurchase.EndDate = (DateTime)ConvertDate(SearchPendingPurchase.sEndDate);

            var resultdata = _materialRepository.GetCompletedPurchaseEntryByFilter(SearchPendingPurchase);
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
        public JsonResult OnGetPurchaseListByID(int ID)
        {
            var resultdata = _repository.GetMaterialInspectionByPurchaseEntryID(ID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _materialRepository.Delete(ID, LoggedUser.UserID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
