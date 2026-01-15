namespace SVAuroraERP.WebUI.Pages.Inventory.Master
{
    public class RackLocationModel : BasePageModel
    {
        private readonly IRackLocationServiceRepository _repository;
        private readonly IWareHouseServiceRepository _grouprepository;
        private readonly ILogger<RackLocation> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly IComponentServiceRepository _componenet = null;
        private readonly ISizeServiceRepository _sizerepository = null;
        private const int PageControlID = (int)Common.Pages.RackLocation; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;


        public RackLocationModel(IRackLocationServiceRepository respository,
                           ILogger<RackLocation> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IWareHouseServiceRepository grouprepository,
                           IComponentServiceRepository componenet,
                           ISizeServiceRepository sizerepository, IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _grouprepository = grouprepository;
            _componenet = componenet;
            _sizerepository = sizerepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> WareHouseList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ComponentTypeList { get; set; } = new List<SelectListItem>();
  
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadWareHouseList();
            LoadComponentTypeList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadWareHouseList()
        {
            DataResponse dataResponse=new DataResponse();
            WareHouseList.Clear();
            dataResponse = _grouprepository.GetWareHouse();
            WareHouseList=((List<VWareHouse>)dataResponse.Value)
                .OrderBy(o => o.WareHouseName)
                .Select(s => new SelectListItem
                {
                    Value = s.WareHouseID.ToString(),
                    Text = s.WareHouseName
                }).ToList();

            WareHouseList.Insert(0, new SelectListItem { Value = "0", Text = "--Select WareHouse--" });
        }
        public void LoadComponentTypeList()
        {
            ComponentTypeList.Clear();

            var districtResponse = _componenet.GetComponentList();

            if (districtResponse.Value is List<VComponentType> districts && districts.Any())
            {
                ComponentTypeList = districts
                    .OrderBy(o => o.ComponentTypeName)
                    .Select(s => new SelectListItem
                    {
                        Value = s.ComponentTypeID.ToString(),
                        Text = s.ComponentTypeName
                    }).ToList();
            }

            ComponentTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Coponent Type--" });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] RackLocation RackLocationData)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                RackLocationData.LastUpdatedBy = LoggedUser.UserID;
                RackLocationData.LoginAuditID = LoggedUser.LoginAuditID;

                if (RackLocationData.RackLocationID == 0)
                    resultdata = _repository.Save(RackLocationData);
                else if (RackLocationData.RackLocationID > 0)
                    resultdata = _repository.Update(RackLocationData);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnGetRackLocationList(int draw, int start, int length)
        {
            DataResponse dataResponse = null;
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = _repository.GetRackLocation();
            var resultdata=((List<VRackLocation>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.RackLocationCode.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.RackLocationName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VRackLocation, object> orderByFunc = orderCol switch
            {
                1 => d => d.RackLocationCode,
                2 => d => d.RackLocationName,
                3 => d => d.WareHouseName,
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
        public JsonResult OnGetRackLocationByID(int ID)
        {

            DataResponse resultdata = _repository.GetByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                resultdata = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnGetSizeList()
        {
            var resultdata = _sizerepository.GetSize();

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnPostUpdateRackCapacity([FromBody] List<RackLocationSizeCapacity> RackLocationSizeCapacity)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {

                foreach (var rackcapacity in RackLocationSizeCapacity)
                {

                    rackcapacity.LastUpdatedBy = LoggedUser.UserID;
                }
                
                    resultdata = _repository.SaveCapacity(RackLocationSizeCapacity);
                

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnPostRackLocationDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Validate sort column
                var validColumns = new[] {"RackLocationName","RackLocationCode","WareHouseName","ComponentTypeName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "RackLocationName";

                dataResponse = _repository.GetRackLocationDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Rack Loaction data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VRackLocation>()
                });
            }
        }

    }
}
