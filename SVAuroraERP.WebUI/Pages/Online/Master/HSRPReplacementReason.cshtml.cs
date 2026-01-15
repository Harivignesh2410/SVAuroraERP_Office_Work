namespace SVAuroraERP.WebUI.Pages.Online.Master
{
    public class HSRPReplacementReasonModel : BasePageModel
    {
        private readonly IHSRPReplacementReasonServiceRepository _repository = null;
        private readonly ILogger<HSRPReplacementReasonServiceRepository> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.HSRPReplacementReason; // ID for this specific page
        public HSRPReplacementReasonModel(IHSRPReplacementReasonServiceRepository respository,
                           ILogger<HSRPReplacementReasonServiceRepository> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                             IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
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
        public JsonResult OnGetHSRPReplacementReasonList(int draw, int start, int length)
        {
            DataResponse dataResponse = null;
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = (_repository.GetHSRPReplacementReason());
            var resultdata = ((List<VHSRPReplacementReason>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.ReplacementReasonName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.Code ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VHSRPReplacementReason, object> orderByFunc = orderCol switch
            {
                1 => d => d.Code,
                2 => d => d.ReplacementReasonName,
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
        public JsonResult OnPostSaveUpdateData([FromBody] HSRPReplacementReason HSRPReplacementReason)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                HSRPReplacementReason.LastUpdatedBy = LoggedUser.UserID;

                if (HSRPReplacementReason.HSRPReplacementReasonID == 0)
                    resultdata = _repository.Save(HSRPReplacementReason);
                else if (HSRPReplacementReason.HSRPReplacementReasonID > 0)
                    resultdata = _repository.Update(HSRPReplacementReason);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetHSRPReplacementReasonByID(int ID)
        {
            DataResponse resultdata = _repository.GetHSRPReplacementReasonByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();

            try
            {
                dataResponse = _repository.Delete(ID, LoggedUser.UserID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata = dataResponse });
            }
        }
        public JsonResult OnPostHSRPReplacementReasonList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "ReplacementReasonName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "ReplacementReasonName";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetHSRPReplacementReasonDataTableList(dataTableRequest);

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
                _logger.LogError(ex, "Error loading HSRP Replacement Reason data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VHSRPReplacementReason>()
                });
            }
        }
    }
}
