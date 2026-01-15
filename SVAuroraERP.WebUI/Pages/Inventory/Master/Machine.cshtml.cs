namespace SVAuroraERP.WebUI.Pages.Inventory.Master
{
    public class MachineModel : BasePageModel
    {
        private readonly IMachineServiceRepository _repository = null;
        private readonly ILogger<Machine> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.Machine; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;
        public MachineModel(IMachineServiceRepository respository,
                           ILogger<Machine> logger,
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
        public List<SelectListItem> MachineTypeList { get; set; } = new List<SelectListItem>();
   
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadMachineList();

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadMachineList()
        {
            DataResponse dataResponse = new DataResponse();
            MachineTypeList.Clear();
            dataResponse= _repository.GetMachineTypeList();
            MachineTypeList=((List<MachineType>)dataResponse.Value)
                .OrderBy(o => o.MachineTypeName)
                .Select(s => new SelectListItem
                {
                    Value = s.MachineTypeID.ToString(),
                    Text = s.MachineTypeName
                }).ToList();

            MachineTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Machine Type--" });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] Machine MachineData)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse(); ;

            try
            {
                MachineData.LastUpdatedBy = LoggedUser.UserID;
                MachineData.LastUpdatedDate = DateTime.UtcNow;
               // MachineData.LoginAuditID = LoggedUser.LoginAuditID;
                MachineData.LastUpdatedBy = LoggedUser.UserID;

                if (MachineData.MachineID == 0)
                    resultdata = _repository.Save(MachineData);
                else if (MachineData.MachineID > 0)
                    resultdata = _repository.Update(MachineData);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetMachineList(int draw, int start, int length)
        {
            DataResponse dataResponse = new DataResponse(); ;
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = _repository.GetMachineList();
            var resultdata =((List<VMachine>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.MachineCode.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.MachineName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VMachine, object> orderByFunc = orderCol switch
            {
                1 => d => d.MachineCode,
                2 => d => d.MachineName,
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

        public JsonResult OnGetMachineByID(int ID)
        {
            DataResponse resultdata = new DataResponse(); ;
             resultdata = _repository.GetMachineByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse(); ;

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
        public JsonResult OnPostMachineDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Validate sort column
                var validColumns = new[] { "MachineName","MachineCode", "MachineTypeName"};
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "MachineName";

                dataResponse = _repository.GetMachineDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Machine data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VMachine>()
                });
            }
        }
    }
}
