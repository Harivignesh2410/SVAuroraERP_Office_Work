namespace SVAuroraERP.WebUI.Pages.Online.Master
{
    public class OEMPricingModel : BasePageModel
    {
        private readonly IOEMPricingServiceRepository _repository = null;
        private readonly ILogger<OEMPricingServiceRepository> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IHSRPPartNumberServiceRepository _hsrpPartNumberServiceRepository = null;
        private readonly IVehiclePlateSizeServiceRepository _vehiclePlateSizeServiceRepository = null;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.OEMPricing; // ID for this specific page

        public OEMPricingModel(IOEMPricingServiceRepository respository,
                           ILogger<OEMPricingServiceRepository> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IHSRPPartNumberServiceRepository hsrpPartNumberServiceRepository,
                           IVehiclePlateSizeServiceRepository vehiclePlateSizeServiceRepository,
                                    IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _hsrpPartNumberServiceRepository = hsrpPartNumberServiceRepository;
            _vehiclePlateSizeServiceRepository = vehiclePlateSizeServiceRepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> PartNumberList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VehiclePlateSizeList { get; set; } = new List<SelectListItem>();
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadPartNumberList();
            LoadvehiclePlateSizeList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadPartNumberList()
        {
            DataResponse dataResponse = new DataResponse();
            PartNumberList.Clear();
            dataResponse = _hsrpPartNumberServiceRepository.GetHSRPPartNumber();
            PartNumberList = ((List<VHSRPPartNumber>)dataResponse.Value)
                .OrderBy(o => o.PartNumber)
                .Select(s => new SelectListItem
                {
                    Value = s.HSRPPartNumberID.ToString(),
                    Text = s.PartNumber
                }).ToList();

            PartNumberList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Hsrp Part Number --" });
        }
        public void LoadvehiclePlateSizeList()
        {
            DataResponse dataResponse = new DataResponse();
            VehiclePlateSizeList.Clear();
            dataResponse = _vehiclePlateSizeServiceRepository.GetVehiclePlateSize();
            VehiclePlateSizeList = ((List<VVehiclePlateSize>)dataResponse.Value)
                .OrderBy(o => o.VehiclePlateSizeCode)
                .Select(s => new SelectListItem
                {
                    Value = s.VehiclePlateSizeID.ToString(),
                    Text = s.VehiclePlateSizeName
                }).ToList();

            VehiclePlateSizeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Vehicle Plate Size --" });
        }
        public JsonResult OnGetOEMPricingList(int draw, int start, int length)
        {
            DataResponse dataResponse = null;
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = (_repository.GetOEMPricing());
            var resultdata = ((List<VOEMPricing>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.PartNumber.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.VehiclePlateSizeCodeFront ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                      (d.VehiclePlateSizeCodeRear ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VOEMPricing, object> orderByFunc = orderCol switch
            {
                1 => d => d.PartNumber,
                2 => d => d.Rivets,
                3 => d => d.SnapLock,
                4 => d => d.Rate,
                5 => d => d.CourierCharges,
                6 => d => d.TotalAmount,
                7 => d => d.VehiclePlateSizeNameFront,
                8 => d => d.VehiclePlateSizeNameRear,
                9 => d => d.OEMPricingID,
                10 => d => d.OEMName,

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
        public JsonResult OnPostSaveUpdateData([FromBody] OEMPricing OEMPricing)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                OEMPricing.LastUpdatedBy = LoggedUser.UserID;

                if (OEMPricing.OEMPricingID == 0)
                    resultdata = _repository.Save(OEMPricing);
                else if (OEMPricing.OEMPricingID > 0)
                    resultdata = _repository.Update(OEMPricing);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetOEMPricingByID(int ID)
        {
            DataResponse resultdata = _repository.GetOEMPricingByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();

            try
            {
                dataResponse = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new {resultdata = dataResponse});
            }
        }
        public JsonResult OnPostOEMPricingList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "PartNumber" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "PartNumber";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetOEMPricingDataTableList(dataTableRequest);

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
                _logger.LogError(ex, "Error loading OEM Pricing data");

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
