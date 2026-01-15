namespace SVAuroraERP.WebUI.Pages.Online.User
{
    public class EmbossingStationModel : BasePageModel
    {
        private readonly IHSRPUserServiceRepository _repository = null;
        private readonly IStateServiceRepository _staterepository = null;
        private readonly ILogger<IHSRPUserServiceRepository> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IHomeFitmentPincodeServiceRepository _homerepository = null;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.EmbossingStation; // ID for this specific page
        public EmbossingStationModel(IHSRPUserServiceRepository respository,
            IStateServiceRepository stateRepository,
                           ILogger<IHSRPUserServiceRepository> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService, IHomeFitmentPincodeServiceRepository homerepository,
                           IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _staterepository = stateRepository;
            _logger = logger;
            _antiforgery = antiforgery;
            _homerepository = homerepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> StateList { get; set; } = new List<SelectListItem>();

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadStateList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadStateList()
        {
            DataResponse dataResponse = new DataResponse();
            StateList.Clear();
            dataResponse = _staterepository.GetState();
            StateList = ((List<VState>)dataResponse.Value)
                .OrderBy(o => o.StateName)
                .Select(s => new SelectListItem
                {
                    Value = s.StateID.ToString(),
                    Text = s.StateName
                }).ToList();

            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State --" });
        }
        public JsonResult OnGetDistrictByStateID(int StateID)
        {
            var response = _homerepository.GetDistrictByStateID(StateID);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetEmbossingStationByID(int ID)
        {
            DataResponse resultdata = _repository.GetHSRPUserByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostSaveUpdateData([FromBody] HSRPUser HSRPUser)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                HSRPUser.LastUpdatedBy = LoggedUser.UserID;

                if (HSRPUser.HSRPUserID == 0)
                    resultdata = _repository.Save(HSRPUser);
                else if (HSRPUser.HSRPUserID > 0)
                    resultdata = _repository.Update(HSRPUser);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetEmbossingStationList(int draw, int start, int length)
        {
            DataResponse dataResponse = null;
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = (_repository.GetEmbossingStation());
            var resultdata = ((List<VHSRPUser>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.CompanyName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.Pincode ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                      (d.DistrictName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VHSRPUser, object> orderByFunc = orderCol switch
            {
                1 => d => d.CompanyName,
                2 => d => d.Pincode,
                3 => d => d.DistrictName,
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
            DataResponse dataResponse = null;

            try
            {
                dataResponse = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata = dataResponse });
            }
        }
        public JsonResult OnPostEmbossingStationList([FromForm] HSRPUserRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "CompanyName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "CompanyName";
                dataTableRequest.UserTypeID = (byte)HSRPUserTypeEnum.EmbossingStation;
                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetHSRPUserDataTableList(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Embossing Station data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VHSRPUser>()
                });
            }
        }
    }
}
