namespace SVAuroraERP.WebUI.Pages.Inventory.Master
{
    public class ItemModel : BasePageModel
    {
        private readonly IItemServiceRepository _repository=null;
        private readonly ICategoryServiceRespository _catrepository=null;
        private readonly ILogger<Item> _logger=null;
        private readonly IAntiforgery _antiforgery=null;
        private readonly IUnitServiceRespository _unitrepository=null;
        private readonly IColorServiceRespository _colorrepository=null;
        private readonly ISizeServiceRepository _sizerepository=null;
        private readonly IComponentServiceRepository _componentrepository=null;
        private const int PageControlID = (int)Common.Pages.Item; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;

        public ItemModel(IItemServiceRepository respository,
                           ILogger<Item> logger,
                           IAntiforgery antiforgery,
                           IUnitServiceRespository unitrepository,
                           IColorServiceRespository colorrepository,
                           ISizeServiceRepository sizerepository,
                           IComponentServiceRepository componentrepository,
                           ICategoryServiceRespository catrepository,
                           IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _unitrepository = unitrepository;
            _colorrepository = colorrepository;
            _sizerepository = sizerepository;
            _componentrepository = componentrepository;
            _catrepository = catrepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> UnitList { get; set; } = new List<SelectListItem>();

        //Added on 2025.01.05 by Sivakumar
        public List<SelectListItem> ItemCategoryList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ColorList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SizeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ComponentList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }


        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadUnitList();
            LoadItemCategoryList();
            LoadColorList();
            LoadSizeList();
            LoadComponentList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadUnitList()
        {
            DataResponse dataResponse = null;
            UnitList.Clear();
            dataResponse = _unitrepository.GetUnit();
            UnitList=((List<VUnit>)dataResponse.Value)
                .OrderBy(o => o.UnitName)
                .Select(s => new SelectListItem
                {
                    Value = s.UnitID.ToString(),
                    Text = s.UnitName
                }).ToList();

            UnitList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Unit--" });
        }

        //Added on 2025.01.05 by Sivakumar
        public void LoadItemCategoryList()
        {
            DataResponse dataResponse = new DataResponse();
            ItemCategoryList.Clear();
            dataResponse = _catrepository.GetCategory();
            ItemCategoryList=((List<VCategory>)dataResponse.Value)
                .OrderBy(o => o.CategoryName)
                .Select(s => new SelectListItem
                {
                    Value = s.CategoryID.ToString(),
                    Text = s.CategoryName
                }).ToList();

            ItemCategoryList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Category--" });
        }
        public void LoadColorList()
        {
            DataResponse dataResponse = new DataResponse();

            ColorList.Clear();
            dataResponse = _colorrepository.GetColor();
            ColorList = ((List<VColor>)dataResponse.Value)
                .OrderBy(o => o.ColorName)
                .Select(s => new SelectListItem
                {
                    Value = s.ColorID.ToString(),
                    Text = s.ColorName
                }).ToList();

            ColorList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Color--" });
        }
        public void LoadSizeList()
        {
            DataResponse dataResponse = new DataResponse();
            SizeList.Clear();
            dataResponse = _sizerepository.GetSize();
            SizeList= ((List<VSize>)dataResponse.Value)
                .OrderBy(o => o.SizeName)
                .Select(s => new SelectListItem
                {
                    Value = s.SizeID.ToString(),
                    Text = s.SizeName
                }).ToList();

            SizeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Size--" });
        }
        public void LoadComponentList()
        {
            DataResponse dataResponse = new DataResponse();
            ComponentList.Clear();
            dataResponse = _componentrepository.GetComponentList();
            ComponentList = ((List<VComponentType>)dataResponse.Value)
                            .OrderBy(o => o.ComponentTypeName)
                             .Select(s => new SelectListItem
                             {
                               Value = s.ComponentTypeID.ToString(),
                               Text = s.ComponentTypeName
                             }).ToList();

            ComponentList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Component Type--" });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] Item ItemData)
        {
            DataResponse resultdata = new DataResponse();
            try
            {
                ItemData.LastUpdatedBy = LoggedUser.UserID;
                ItemData.LastUpdatedDate = DateTime.UtcNow;
                ItemData.LoginAuditID = LoggedUser.LoginAuditID;

                if (ItemData.ItemID == 0)
                    resultdata = _repository.Save(ItemData);
                else if (ItemData.ItemID > 0)
                    resultdata = _repository.Update(ItemData);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetItemList(int draw, int start, int length)
        {
            DataResponse response = new DataResponse();
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            response = _repository.GetItem();
            var resultdata= ((List<VItem>)response.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.ItemCode.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.ItemName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VItem, object> orderByFunc = orderCol switch
            {
                1 => d => d.ItemCode,
                2 => d => d.HSNCode,
                3 => d => d.ItemName,
                4 => d => d.Description,
                5 => d => d.Price,
                6 => d => d.UnitName,
                7 => d => d.ComponentTypeName,

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
        public JsonResult OnGetItemByID(int ID)
        {
            DataResponse resultdata = new DataResponse();
            resultdata = _repository.GetByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                resultdata = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { result = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = resultdata });
            }
        }
        public JsonResult OnPostItemDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Validate sort column
                var validColumns = new[] {"ItemName","ItemCode","HSNCode","ItemCategoryName","UnitName","ColorName","SizeName","ComponentTypeName"};
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "ItemName";

                dataResponse = _repository.GetItemDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Item data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VItem>()
                });
            }
        }
    }
}
