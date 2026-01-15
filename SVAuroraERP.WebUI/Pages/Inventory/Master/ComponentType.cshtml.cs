namespace SVAuroraERP.WebUI.Pages.Inventory.Master
{
    public class ComponentTypeModel : BasePageModel
    {
        private readonly IComponentServiceRepository _repository = null;
        private readonly ILogger<ComponentTypeModel> logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.ComponentType; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;


        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public ComponentTypeModel(IComponentServiceRepository respository,
                           ILogger<ComponentTypeModel> _logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                             IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            logger = _logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public JsonResult OnPostSaveUpdateData([FromBody] ComponentType ComponentData)
        {
            DataResponse resultdata = new DataResponse();
            string message = string.Empty;
            try
            {
                ComponentData.LastUpdatedBy = LoggedUser.UserID;
                ComponentData.LoginAuditID = LoggedUser.LoginAuditID;

                if (ComponentData.ComponentTypeID == 0)
                    resultdata = _repository.Save(ComponentData);
                else if (ComponentData.ComponentTypeID > 0)
                    resultdata = _repository.Update(ComponentData);

                return new JsonResult(new { resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata });
            }
        }
        public JsonResult OnGetComponenetTypeList(int draw, int start, int length)
        {
            DataResponse dataResponse = new DataResponse();
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = _repository.GetComponentList();
            var resultdata = ((List<VComponentType>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.ComponentTypeCode.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.ComponentTypeName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VComponentType, object> orderByFunc = orderCol switch
            {
                1 => d => d.ComponentTypeCode,
                2 => d => d.ComponentTypeName,
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
        public JsonResult OnGetComponenetByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
             dataResponse = _repository.GetComponentByID(ID);

            return new JsonResult(new { dataResponse});
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();
            try
            {
                dataResponse = _repository.Delete(ID, LoggedUser.UserID);
                return new JsonResult(new { result = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { dataResponse });
            }
        }
        public JsonResult OnPostComponentTypeDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Validate sort column
                var validColumns = new[] { "ComponentTypeName", "ComponentTypeCode" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "ComponentTypeName";

                dataResponse = _repository.GetComponentDataTable(dataTableRequest);

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
                logger.LogError(ex, "Error loading Component Type data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VComponentType>()
                });
            }
        }
    }
}
