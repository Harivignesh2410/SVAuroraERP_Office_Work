namespace SVAuroraERP.WebUI.Pages.Inventory.Production
{
    public class ProductionConfigurationModel : BasePageModel
    {
        private readonly IProductionConfigurationServiceRepository _repository = null;
        private readonly ILogger<ProductionConfiguration> logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IComponentServiceRepository _componentServiceRepository;
        private readonly IProcessTypeServiceRepository _processTypeServiceRepository;
        private const int PageControlID = (int)Common.Pages.ProductionConfiguration;
        private readonly IPermissionServiceRepository _permissionrepository;

        public ProductionConfigurationModel(IProductionConfigurationServiceRepository respository,
                           ILogger<ProductionConfiguration> _logger,
                           IAntiforgery antiforgery,
                           IComponentServiceRepository componentServiceRepository,
                           IProcessTypeServiceRepository processTypeServiceRepository,
                           SessionService sessionService,
                            IPermissionServiceRepository permissionrepository)
        {
            _repository = respository;
            logger = _logger;
            _antiforgery = antiforgery;
            _componentServiceRepository = componentServiceRepository;
            _processTypeServiceRepository = processTypeServiceRepository;
            _permissionrepository = permissionrepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> ComponentList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ProcessTypeList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }


        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadComponentList();
            LoadProcessTypeList();

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadComponentList()
        {
            DataResponse dataResponse = null;
            ComponentList.Clear();
            dataResponse = _componentServiceRepository.GetComponentList();
            ComponentList = ((List<VComponentType>)dataResponse.Value)
                            .OrderBy(o => o.ComponentTypeName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.ComponentTypeID.ToString(),
                                 Text = s.ComponentTypeName
                             }).ToList();

            ComponentList.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
        }
        public void LoadProcessTypeList()
        {
            var processTypes = _processTypeServiceRepository.GetProcessTypeList().OrderBy(o => o.OrdinalNo);
            if (processTypes != null && processTypes.Any())
            {
                ProcessTypeList = processTypes.Select(s => new SelectListItem
                {
                    Value = s.ProcessTypeID.ToString(),
                    Text = s.ProcessTypeName
                }).ToList();

                ProcessTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
            }
            else
            {
                Console.WriteLine("Process Type List is empty!");
            }

        }
        public JsonResult OnPostSaveUpdateDataInputConfig([FromBody] ProductionConfiguration InputData)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                InputData.LastUpdatedBy = LoggedUser.UserID;

                if (InputData.ProductionConfigurationID == 0)
                    resultdata = _repository.Save(InputData);
                else if (InputData.ProductionConfigurationID > 0)
                    resultdata = _repository.Update(InputData);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public JsonResult OnGetProductionConfigurationList(int draw, int start, int length)
        {
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            var resultdata = (_repository.GetProductionConfigurationList()).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.ProcessTypeName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.ComponentTypeName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VProductionConfiguration, object> orderByFunc = orderCol switch
            {
                1 => d => d.ProcessTypeName,
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
        public JsonResult OnGetProcessTypeList(int draw, int start, int length)
        {
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            var resultdata = (_processTypeServiceRepository.GetProcessTypeList()).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.ProcessTypeName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.ComponentTypeName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VProcessType, object> orderByFunc = orderCol switch
            {
                1 => d => d.ProcessTypeName,
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
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _repository.Delete(ID, LoggedUser.UserID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnGetProductionConfigurationByID(int ID)
        {
            var resultdata = _repository.GetProductionConfigurationByID(ID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnGetProcessTypeByID(int ID)
        {
            var resultdata = _processTypeServiceRepository.GetByID(ID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnPostUpdateDataOutputConfig([FromBody] ProcessType OutputData)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _processTypeServiceRepository.Update(OutputData);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}