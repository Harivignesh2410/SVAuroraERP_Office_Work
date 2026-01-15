namespace SVAuroraERP.WebUI.Pages.Inventory.Purchase
{
    public class SupplierModel : BasePageModel
    {
        private readonly ISupplierServiceRepository _repository = null;
        private readonly ILogger<Supplier> logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.LaserNoMarking;
        private readonly IPermissionServiceRepository _permissionrepository;

        public SupplierModel(ISupplierServiceRepository respository,
                           ILogger<Supplier> _logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IPermissionServiceRepository permissionrepository)
        {
            _repository = respository;
            logger = _logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionrepository;
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
        public JsonResult OnPostSaveUpdateData([FromBody] Supplier SupplierData)
        {
            string message = string.Empty;
          DataResponse resultdata = new DataResponse();

            try
            {
                SupplierData.LastUpdatedBy = LoggedUser.UserID;
                SupplierData.LoginAuditID = LoggedUser.LoginAuditID;

                if (SupplierData.SupplierID == 0)
                    resultdata = _repository.Save(SupplierData);
                else if (SupplierData.SupplierID > 0)
                    resultdata = _repository.Update(SupplierData);

                return new JsonResult(new {resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new {resultdata});
            }
        }

        public JsonResult OnGetSupplierList(int draw, int start, int length)
        {
            DataResponse dataResponse = new DataResponse();
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = (_repository.GetSupplier());
            var resultdata =((List<VSupplier>)dataResponse.Value).OrderBy(o => o.SupplierCode).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.SupplierCode.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.SupplierName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VSupplier, object> orderByFunc = orderCol switch
            {
                1 => d => d.SupplierCode,
                2 => d => d.SupplierName,
                3 => d => d.City,
                4 => d => d.MobileNo,
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

        public JsonResult OnGetSupplierByID(int ID)
        {
            var resultdata = _repository.GetByID(ID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
           DataResponse resultdata = new DataResponse();

            try
            {
                resultdata = _repository.Delete(ID);

                return new JsonResult(new {resultdata});
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata });
            }
        }


        public JsonResult OnPostCheckForDuplicate([FromBody] Supplier supplierData)
        {
            DataResponse resultdata = new DataResponse();
            resultdata= _repository.GetSupplier();
            var suppliers = resultdata.Value as List<Supplier> ?? new List<Supplier>();


            // Check each field individually to provide more specific feedback
            var duplicateCode = suppliers.Any(s =>
                s.SupplierID != supplierData.SupplierID &&
                string.Equals(s.SupplierCode, supplierData.SupplierCode, StringComparison.OrdinalIgnoreCase));

            var duplicateName = suppliers.Any(s =>
                s.SupplierID != supplierData.SupplierID &&
                string.Equals(s.SupplierName, supplierData.SupplierName, StringComparison.OrdinalIgnoreCase));

            var duplicateMobile = suppliers.Any(s =>
                s.SupplierID != supplierData.SupplierID &&
                string.Equals(s.MobileNo, supplierData.MobileNo, StringComparison.OrdinalIgnoreCase));

            bool isDuplicate = duplicateCode || duplicateName || duplicateMobile;

            return new JsonResult(new
            {
                success = true,
                isDuplicate,
                duplicateCode,
                duplicateName,
                duplicateMobile
            });
        }
        public JsonResult OnPostSupplierDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Validate sort column
                var validColumns = new[] {"SupplierName", "City", "SupplierCode", "GSTNo", "MobileNo", "Email" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "SupplierName";

                dataResponse = _repository.GetSupplierDataTable(dataTableRequest);

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
                logger.LogError(ex, "Error loading Purchase Entry  data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VSupplier>()
                });
            }
        }

    }
}