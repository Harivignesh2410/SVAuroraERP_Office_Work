namespace SVAuroraERP.WebUI.Pages.Inventory.MaterialManagement
{
    public class PendingInspectionModel : BasePageModel
    {
        private readonly IPurchaseEntryServiceRepository _repository = null;
        private readonly IPendingInspectionServiceRepository _materialRepository = null;
        private readonly ILogger<PendingInspection> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly ISupplierServiceRepository _Supplierrepository = null;
        private readonly IComponentServiceRepository _componentrepository;
        private const int PageControlID = (int)Common.Pages.PendingInspection;
        private readonly IPermissionServiceRepository _permissionrepository;
        public PendingInspectionModel(IPurchaseEntryServiceRepository respository,
                           ILogger<PendingInspection> logger,
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
        public JsonResult OnGetMaterialList()
        {
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            var resultdata = (_repository.GetPurchaseEntry()).OrderByDescending(o => o.PurchaseEntryID).ToList();

            var filteredData = string.IsNullOrWhiteSpace(searchValue)
               ? resultdata
               : resultdata.Where(d => d.PurchaseInvoiceNo.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();
            return new JsonResult(new
            {
                recordsTotal = resultdata.Count,
                recordsFiltered = filteredData.Count,
                data = resultdata
            });
        }
        public void LoadSupplierList()
        {
            DataResponse dataResponse = new DataResponse();
            SupplierList.Clear();
            dataResponse = _Supplierrepository.GetSupplier();
            SupplierList = ((List<VSupplier>)dataResponse.Value)
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
            dataResponse = _materialRepository.GetComponenetListdropdown();
            ComponentList = ((List<VComponentExceptType>)dataResponse.Value)
                            .OrderBy(o => o.ComponentTypeName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.ComponentTypeID.ToString(),
                                 Text = s.ComponentTypeName
                             }).ToList();

            ComponentList.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
        }
        public JsonResult OnGetPurchaseListByID(int ID)
        {
            var resultdata = _repository.GetByID(ID);

            resultdata.sPurchaseInvoiceDate = resultdata.PurchaseInvoiceDate.ToString("dd/MM/yyyy");

            return new JsonResult(new { success = true, data = resultdata });
        }

        public JsonResult OnPostSaveUpdateData([FromBody] List<PendingInspection> MaterialInwardInspection)
        {

            string message = string.Empty;
            Tuple<DataResponse> resultdata = null;
            try
            {
                foreach (PendingInspection materialInwardInspection1 in MaterialInwardInspection)
                {
                    materialInwardInspection1.LastUpdatedBy = LoggedUser.UserID;
                    //  materialInwardInspection1.LastUpdatedDate = DateTime.UtcNow;
                }

                resultdata = _materialRepository.SaveMaterialInward(MaterialInwardInspection);
                return new JsonResult(new { resultdata.Item1 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata.Item1 });
            } 
        }

        public JsonResult OnGetMaterialListByPurchaseID(int ID)
        {
            var resultdata = _materialRepository.GetMaterialInwardListByID(ID);

            return new JsonResult(new { result = resultdata });
        }

        public JsonResult OnGetNextBatchNumber()
        {
            try
            {
                string nextBatchNumber = _materialRepository.GenerateNextBatchNumber();
                return new JsonResult(new { batchNumber = nextBatchNumber });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { message = "Error generating batch number", error = ex.Message });
            }
        }
        public JsonResult OnPostPendingPurchaseEntryByFilter([FromBody] SearchPendingPurchase SearchPendingPurchase)
        {
            if (!string.IsNullOrEmpty(SearchPendingPurchase.sStartDate)) SearchPendingPurchase.StartDate = (DateTime)ConvertDate(SearchPendingPurchase.sStartDate);
            if (!string.IsNullOrEmpty(SearchPendingPurchase.sEndDate)) SearchPendingPurchase.EndDate = (DateTime)ConvertDate(SearchPendingPurchase.sEndDate);

            var resultdata = _repository.GetPendingPurchaseEntryByFilter(SearchPendingPurchase);
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
