namespace SVAuroraERP.WebUI.Pages.Inventory.Master
{
    public class CategoryModel : BasePageModel
    {
        private readonly ICategoryServiceRespository _repository = null;
        private readonly ILogger<CategoryModel> logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.Category; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;


        public CategoryModel(ICategoryServiceRespository respository,
                           ILogger<CategoryModel> _logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
            IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            logger = _logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
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

        public JsonResult OnPostSaveUpdateData([FromBody] Category CategoryData)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                CategoryData.LastUpdatedBy = LoggedUser.UserID;
                CategoryData.LoginAuditID = LoggedUser.LoginAuditID;

                if (CategoryData.CategoryID == 0)
                    resultdata = _repository.Save(CategoryData);
                else if (CategoryData.CategoryID > 0)
                    resultdata = _repository.Update(CategoryData);

                return new JsonResult(new { result = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = resultdata });
            }
        }
        public JsonResult OnGetCategoryList(int draw, int start, int length)
        {
            DataResponse dataResponse = new DataResponse();

            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = _repository.GetCategory();
            var resultdata = ((List<VCategory>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.CategoryCode.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.CategoryName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VCategory, object> orderByFunc = orderCol switch
            {
                1 => d => d.CategoryCode,
                2 => d => d.CategoryName,
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
        public JsonResult OnGetCategoryByID(int ID)
        {
            DataResponse resultdata = new DataResponse();

            resultdata = _repository.GetByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            DataResponse resultdata = new DataResponse();

            string message = string.Empty;

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
        public JsonResult OnPostCategoryDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Validate sort column
                var validColumns = new[] { "CategoryName","CategoryCode" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "CategoryName";

                dataResponse = _repository.GetCategoryDataTable(dataTableRequest);

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
                logger.LogError(ex, "Error loading Category  data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VCategory>()
                });
            }
        }
    }
}