namespace SVAuroraERP.WebUI.Pages.Inventory.Master
{
    public class OtherChargesModel : BasePageModel
    {
        private readonly IOtherChargesServiceRepository _repository = null;
        private readonly ILogger<OtherCharges> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.OtherCharges; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;
        public OtherChargesModel(IOtherChargesServiceRepository respository,
                           ILogger<OtherCharges> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,IPermissionServiceRepository permissionService)
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
        public JsonResult OnPostSaveUpdateData([FromBody] OtherCharges OtherChargesData)
        {
            string message = string.Empty;
           DataResponse resultdata = null;

            try
            {
                OtherChargesData.LastUpdatedBy = LoggedUser.UserID;
                OtherChargesData.LoginAuditID = LoggedUser.LoginAuditID;

                if (OtherChargesData.OtherChargesID == 0)
                    resultdata = _repository.Save(OtherChargesData);
                else if (OtherChargesData.OtherChargesID > 0)
                    resultdata = _repository.Update(OtherChargesData);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetOtherChargesList(int draw, int start, int length)
        {
            DataResponse dataResponse = new DataResponse();
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = (_repository.GetOtherCharges());
                var resultdata=((List<VOtherCharges>)dataResponse.Value).
                OrderBy(o => o.OtherChargesDescription).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => 
                                     (d.OtherChargesDescription ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VOtherCharges, object> orderByFunc = orderCol switch
            {
                1 => d => d.OtherChargesDescription,
                2 => d => d.TypeName,
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
        public JsonResult OnGetOtherChargesByID(int ID)
        {
            DataResponse resultdata = new DataResponse();
             resultdata = _repository.GetByID(ID);

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
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnPostOtherChargesDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Validate sort column
                var validColumns = new[] { "OtherChargesDescription","TypeName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "OtherChargesDescription";

                dataResponse = _repository.GetOtherChargesDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Other Charges data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VOtherCharges>()
                });
            }
        }
    }
}
