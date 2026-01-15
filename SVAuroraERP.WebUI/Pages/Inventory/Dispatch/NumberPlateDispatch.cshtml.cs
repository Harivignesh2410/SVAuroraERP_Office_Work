namespace SVAuroraERP.WebUI.Pages.Inventory.Dispatch
{
    public class NumberPlateDispatchModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger<NumberPlateDispatchModel> logger = null;
        private const int PageControlID = (int)Common.Pages.NumberPlateDispatch; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;
        private readonly ICourierServiceRepository _courierServiceRepository;
        private readonly IPackingServiceRepository _packingServiceRepository;
        private readonly INumberPlateDispatchServiceRepository _numberPlateDispatchServiceRepository;
        private readonly IHSRPUserServiceRepository _hsrpartServiceRepository;
        public NumberPlateDispatchModel(IAntiforgery antiforgery,
                            ICourierServiceRepository courierServiceRepository,
                            IPackingServiceRepository packingServiceRepository,
                            INumberPlateDispatchServiceRepository numberPlateDispatchServiceRepository,
                            IHSRPUserServiceRepository hsrpartServiceRepository,
                            IPermissionServiceRepository permissionService,
                            ILogger<NumberPlateDispatchModel> _logger)
        {
            _antiforgery = antiforgery;
            _courierServiceRepository = courierServiceRepository;
            _packingServiceRepository = packingServiceRepository;
            _numberPlateDispatchServiceRepository = numberPlateDispatchServiceRepository;
            _hsrpartServiceRepository = hsrpartServiceRepository;
            _permissionrepository = permissionService;
            logger = _logger;
        }

        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> CourierList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AllotedToList { get; set; } = new List<SelectListItem>();
    
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadCourierList();
            LoadAllotedToList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadCourierList()
        {
            DataResponse dataResponse = null;

            CourierList.Clear();
            dataResponse = _courierServiceRepository.GetCourier();
            CourierList = ((List<VCourier>)dataResponse.Value)
                .OrderBy(o => o.CourierName)
                .Select(s => new SelectListItem
                {
                    Value = s.CourierID.ToString(),
                    Text = s.CourierName
                }).ToList();

            CourierList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Courier--" });
        }
        public void LoadAllotedToList()
        {
            DataResponse dataResponse = null;

            AllotedToList.Clear();
            dataResponse = _hsrpartServiceRepository.GetEmbossingStation();
            AllotedToList = ((List<VHSRPUser>)dataResponse.Value)
                .OrderBy(o => o.CompanyName)
                .Select(s => new SelectListItem
                {
                    Value = s.HSRPUserID.ToString(),
                    Text = s.CompanyName + " (" + s.DistrictName + ") - " + s.HSRPUserCode
                }).ToList();

            AllotedToList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Dispatch To--" });
        }

        public JsonResult OnGetPackingListByIDData(int ID)
        {
            var resultdata = _packingServiceRepository.GetPackingListByStatus(ID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] NumberPlateDispatch request)
        {
            Tuple<bool, string> resultdata = null;
            request.DispatchDate = (DateTime)ConvertDate(request.sDispatchDate);
            request.DocketBookingDate = (DateTime)ConvertDate(request.sDocketBookingDate);
            request.LastUpdatedDate = DateTime.Now;
            request.LastUpdatedBy = LoggedUser.UserID;
            resultdata = _numberPlateDispatchServiceRepository.Save(request);

            return new JsonResult(new { success = true, data = resultdata });
        }
        private DateTime? ConvertDate(string sdate)
        {

            DateTime? dtConvertedDate = null;
            if (DateTime.TryParseExact(sdate, Domain.Constants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDt))
            {
                dtConvertedDate = parsedDt;
            }

            return dtConvertedDate;
        }
        public JsonResult OnGetListData(int draw, int start, int length)
        {
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            var resultdata = (_numberPlateDispatchServiceRepository.GetNumberPlateDispatchList()).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.DispatchNo.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();


            // Handle sorting based on the column index and direction
            Func<VNumberPlateDispatch, object> orderByFunc = orderCol switch
            {
                1 => d => d.DispatchNo,
                2 => d => d.DispatchDate,
                3 => d => d.ModeofTransportName,
                4 => d => d.TransportDetails,
                5 => d => d.DocketNo,
                6 => d => d.DocketBookingDate,
                7 => d => d.EmbossingStationName,
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
            DataResponse resultdata = null;

            try
            {
                resultdata = _numberPlateDispatchServiceRepository.Delete(ID,LoggedUser.UserID);

                return new JsonResult(new { resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata });
            }
        }
        public JsonResult OnPostAcknowledgeInnerBox([FromBody] int packingID)
        {
            try
            {
                if (packingID <= 0)
                {
                    return new JsonResult(new { success = false, message = "Invalid Packing ID" });
                }

                var LastupdatedBy = LoggedUser.UserID;

                // Directly pass the integer to the repository (assuming the repository method supports int input)
                var resultdata = _numberPlateDispatchServiceRepository.InsertHSRPLaserStockTransID(packingID, LastupdatedBy);

                return new JsonResult(new { success = true, data = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnGetNumberPlateDispatchByID(int ID)
        {
            var resultdata = _numberPlateDispatchServiceRepository.GetNumberPlateDispatchByID(ID);

            return new JsonResult(new { success = true, data = resultdata });
        }

    }
}
