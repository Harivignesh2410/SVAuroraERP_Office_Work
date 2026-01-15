namespace SVAuroraERP.WebUI.Pages.Inventory.Purchase
{
    public class PurchaseEntryModel : BasePageModel
    {
        private readonly IPurchaseEntryServiceRepository _repository = null;
        private readonly IItemServiceRepository _itemrepository = null;
        private readonly IOtherChargesServiceRepository _otherchargesrespository = null;
        private readonly ITaxServiceRepository _taxrepository = null;
        private readonly ISupplierServiceRepository _Supplierrepository = null;
        private readonly ILogger<PurchaseEntry> logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPurchaseOrderServiceRepository _orderrepository = null;
        private readonly IDocumentGroupServiceRepository _documentgrouprepository = null;
        private readonly IDocumentTypeServiceRepository _documenttyperepository = null;
        private const int PageControlID = (int)Common.Pages.PurchaseEntry;
        private readonly IPermissionServiceRepository _permissionrepository;

        public PurchaseEntryModel(IPurchaseEntryServiceRepository respository,
                           ILogger<PurchaseEntry> _logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IItemServiceRepository itemrepository,
                           IOtherChargesServiceRepository otherchargesrespository,
                           ITaxServiceRepository taxrepository,
                           ISupplierServiceRepository Supplierrepository,
                           IPurchaseOrderServiceRepository orderrepository,
                           IDocumentGroupServiceRepository documentgrouprepository,
                           IDocumentTypeServiceRepository documenttyperepository,
                           IPermissionServiceRepository permissionService
                           )
        {
            _repository = respository;
            logger = _logger;
            _antiforgery = antiforgery;
            _itemrepository = itemrepository;
            _otherchargesrespository = otherchargesrespository;
            _itemrepository = itemrepository;
            _taxrepository = taxrepository;
            _Supplierrepository = Supplierrepository;
            _orderrepository = orderrepository;
            _documentgrouprepository = documentgrouprepository;
            _documenttyperepository = documenttyperepository;
            _permissionrepository = permissionService;

        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> SupplierList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> DocumentTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> DocumentGroupList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadSupplierList();
            LoadDocumentTypeList();
            LoadDocumentGroupList();

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        //PurchaseTrans
        public JsonResult OnGetItemList()
        {
            DataResponse dataResponse = null;
            dataResponse = _itemrepository.GetItem();
            var Itemlist = ((List<VItem>)dataResponse.Value)
            .OrderBy(o => o.ItemName).ToList();
            return new JsonResult(Itemlist);
        }

        public JsonResult OnGetOtherChargesList()
        {
            DataResponse dataResponse = new DataResponse();
            dataResponse = _otherchargesrespository.GetOtherCharges();
            var otherchargeslist=((List<VOtherCharges>)dataResponse.Value).
                OrderBy(o => o.OtherChargesDescription).ToList();
            return new JsonResult(otherchargeslist);
        }

        public JsonResult OnGetTaxList()
        {
          DataResponse dataResponse = new DataResponse();
            dataResponse = _taxrepository.GetTax();
            var taxlist=((List<VTax>)dataResponse.Value).OrderBy(o => o.TaxName).ToList();

            return new JsonResult(taxlist);
        }

        //PurchaseEntry
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
        public void LoadDocumentTypeList()
        {
            DataResponse dataResponse = new DataResponse();
            DocumentTypeList.Clear();
            dataResponse = _documenttyperepository.GetDocumentType();
            DocumentTypeList = ((List<VDocumentType>)dataResponse.Value)
               .OrderBy(o => o.DocumentTypeName)
                .Select(s => new SelectListItem
                {
                    Value = s.DocumentTypeID.ToString(),
                    Text = s.DocumentTypeName,
                }).ToList();

            DocumentTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Document Type--" });
        }
        public void LoadDocumentGroupList()
        {
            DataResponse dataResponse = new DataResponse(); ;
            DocumentGroupList.Clear();
            dataResponse = _documentgrouprepository.GetDocumentGroup();
            DocumentGroupList = ((List<VDocumentGroup>)dataResponse.Value)
                .OrderBy(o => o.DocumentGroupName)
                .Select(s => new SelectListItem
                {
                    Value = s.DocumentGroupID.ToString(),
                    Text = s.DocumentGroupName
                }).ToList();

            DocumentGroupList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Document Group--" });
        }

        //save the data
        public async Task<JsonResult> OnPostSaveUpdateData([FromBody] PurchaseEntry PurchaseEntry)
        {

            string message = string.Empty;
            Tuple<bool, bool, int> resultdata = null;
            try
            {
                PurchaseEntry.PurchaseInvoiceDate = (DateTime)ConvertDate(PurchaseEntry.sPurchaseInvoiceDate);

                PurchaseEntry.OtherChargesID = PurchaseEntry.OtherChargesID == 0 ? null : PurchaseEntry.OtherChargesID;
                PurchaseEntry.TaxID1 = PurchaseEntry.TaxID1 == 0 ? null : PurchaseEntry.TaxID1;
                PurchaseEntry.TaxID2 = PurchaseEntry.TaxID2 == 0 ? null : PurchaseEntry.TaxID2;

                foreach (PurchaseEntryTrans purchaseEntryTrans in PurchaseEntry.PurchaseEntryTransList)
                {
                    purchaseEntryTrans.OtherChargesID1 = purchaseEntryTrans.OtherChargesID1 == 0 ? null : purchaseEntryTrans.OtherChargesID1;
                    purchaseEntryTrans.OtherChargesID2 = purchaseEntryTrans.OtherChargesID2 == 0 ? null : purchaseEntryTrans.OtherChargesID2;
                    purchaseEntryTrans.OtherChargesID3 = purchaseEntryTrans.OtherChargesID3 == 0 ? null : purchaseEntryTrans.OtherChargesID3;
                    purchaseEntryTrans.TaxID1 = purchaseEntryTrans.TaxID1 == 0 ? null : purchaseEntryTrans.TaxID1;
                    purchaseEntryTrans.TaxID2 = purchaseEntryTrans.TaxID2 == 0 ? null : purchaseEntryTrans.TaxID2;
                    purchaseEntryTrans.LastUpdatedBy = LoggedUser.UserID;
                }
                PurchaseEntry.LastUpdatedBy = LoggedUser.UserID;
                if (PurchaseEntry.PurchaseEntryID == 0)
                {
                    resultdata = _repository.Save(PurchaseEntry);
                }
                else if (PurchaseEntry.PurchaseEntryID > 0)
                    resultdata = _repository.Update(PurchaseEntry);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2, ID = resultdata.Item3 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        private DateTime? ConvertDate(string sdate)
        {

            DateTime? dtConvertedDate = null;
            if (DateTime.TryParseExact(sdate, Domain.Constants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDt))
            {
                dtConvertedDate = parsedDt;
            }

            return dtConvertedDate;
        }
        public async Task<JsonResult> OnGetPurchaseList(int draw, int start, int length)
        {
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            var resultdata = (_repository.GetPurchaseEntry()).OrderByDescending(o => o.PurchaseEntryID).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.PurchaseInvoiceNo.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VPurchaseEntry, object> orderByFunc = orderCol switch
            {
                1 => d => d.PurchaseInvoiceNo,
                2 => d => d.PurchaseInvoiceDate,
                3 => d => d.SupplierName,
                4 => d => d.TotalPcs,
                5 => d => d.TotalQuantity,
                6 => d => d.TaxAmount,
                7 => d => d.GrossAmount,
                8 => d => d.PurchaseInvoiceAmount,
                9 => d => d.TotalItemTax,
                10 => d => d.PurchaseStatus,
                _ => null  // No sorting for other columns
            };

            if (orderByFunc != null)
            {
                filteredData = orderDir == "asc"
                    ? filteredData.OrderBy(orderByFunc).ToList()
                    : filteredData.OrderByDescending(orderByFunc).ToList();
            }

            // Paginate the filtered data
            var paginatedData = filteredData.Skip(start).Take(length).ToList();

            // Return the JSON result
            return new JsonResult(new
            {
                draw = draw,
                recordsTotal = resultdata.Count,
                recordsFiltered = filteredData.Count,
                data = paginatedData
            });
        }

        public JsonResult OnGetPurchaseEntryByID(int ID)
        {
            var resultdata = _repository.GetByID(ID);

            resultdata.sPurchaseInvoiceDate = resultdata.PurchaseInvoiceDate.ToString("dd/MM/yyyy");

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnPostDeleteData([FromBody] int id)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _repository.Delete(id);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public JsonResult OnGetPurchaseOrderByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            var resultdata = _orderrepository.GetPurchaseOrderByID(ID);
            return new JsonResult(new { resultdata });
        }
    }
}