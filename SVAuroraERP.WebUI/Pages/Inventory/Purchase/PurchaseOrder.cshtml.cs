namespace SVAuroraERP.WebUI.Pages.Inventory.Purchase
{
    public class PurchaseOrderModel : BasePageModel
    {
        private readonly IPurchaseOrderServiceRepository _repository = null;
        private readonly IItemServiceRepository _itemrepository = null;
        private readonly ISupplierServiceRepository _Supplierrepository = null;
        private readonly ILogger<PurchaseOrder> logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.PurchaseOrder;
        private readonly IPermissionServiceRepository _permissionrepository;

        public PurchaseOrderModel(IPurchaseOrderServiceRepository respository,
                           ILogger<PurchaseOrder> _logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IItemServiceRepository itemrepository,
                           ISupplierServiceRepository Supplierrepository,
                           IPermissionServiceRepository permissionrepository)
        {
            _repository = respository;
            logger = _logger;
            _antiforgery = antiforgery;
            _itemrepository = itemrepository;
            _Supplierrepository = Supplierrepository;
            _permissionrepository = permissionrepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> SupplierList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        //public List<SelectListItem> ItemList { get; set; } = new List<SelectListItem>();
        string DateFormat = "dd/MM/yyyy";


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
        //public void LoadItemList()
        //{
        //    ItemList.Clear();
        //    ItemList = _itemrepository.GetItem()
        //        .OrderBy(o => o.ItemName)
        //        .Select(s => new SelectListItem
        //        {
        //            Value = s.ItemID.ToString(),
        //            Text = s.ItemName,
        //        }).ToList();

        //    SupplierList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Item--" });
        //}
        public JsonResult OnGetItemList()
        {
            DataResponse dataResponse = null;
            dataResponse = _itemrepository.GetItem();
            var Itemlist = ((List<VItem>)dataResponse.Value)
            .OrderBy(o => o.ItemName).ToList();
            return new JsonResult(Itemlist);
        }

        public JsonResult OnPostSaveUpdateData([FromBody] PurchaseOrder PurchaseOrder)
        {

            string message = string.Empty;
            DataResponse dataResponse = null;
            try
            {
                PurchaseOrder.PurchaseOrderDate = (DateTime)ConvertDate(PurchaseOrder.sPurchaseOrderDate);



                foreach (PurchaseOrderTrans purchaseOrderTrans in PurchaseOrder.PurchaseOrderTransList)
                {
                    purchaseOrderTrans.LastUpdatedBy = LoggedUser.UserID;
                }
                PurchaseOrder.LastUpdatedBy = LoggedUser.UserID;
                if (PurchaseOrder.PurchaseOrderID == 0)
                {
                    dataResponse = _repository.Save(PurchaseOrder);
                }
                else if (PurchaseOrder.PurchaseOrderID > 0)
                    dataResponse = _repository.Update(PurchaseOrder);

                return new JsonResult(new { dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
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
        public JsonResult OnGetPurchaseOrderList(int draw, int start, int length)
        {
            DataResponse dataResponse = new DataResponse();

            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = _repository.GetPurchaseOrder();

            var resultdata = ((List<VPurchaseOrder>)dataResponse.Value).OrderByDescending(o => o.PurchaseOrderID).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.PurchaseOrderNo.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VPurchaseOrder, object> orderByFunc = orderCol switch
            {
                1 => d => d.PurchaseOrderNo,
                2 => d => d.PurchaseOrderDate,
                3 => d => d.SupplierName,
                4 => d => d.PurchaseOrderValue,
                5 => d => d.PurchaseOrderStatus,
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
        public JsonResult OnGetPurchaseOrderByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            dataResponse = _repository.GetPurchaseOrderByID(ID);

            return new JsonResult(new { dataResponse });
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();
            try
            {
                dataResponse = _repository.Delete(ID, LoggedUser.UserID);

                return new JsonResult(new { dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { dataResponse });
            }
        }
        public JsonResult OnPostPurchaseOrderDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Validate sort column
                var validColumns = new[] {"SupplierName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "SupplierName";

                dataResponse = _repository.GetPurchaseOrderDataTable(dataTableRequest);

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
                logger.LogError(ex, "Error loading Purchase Order  data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VPurchaseOrder>()
                });
            }
        }
    }
}
