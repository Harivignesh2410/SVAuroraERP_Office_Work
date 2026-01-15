namespace SVAuroraERP.WebUI.Pages.Inventory.Dispatch
{
    public class PackingModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger<PackingModel> logger = null;
        private readonly IBoxServiceRepository _boxServiceRepository;
        private readonly IHSRPUserServiceRepository _hsrpartServiceRepository;
        private readonly IColorServiceRespository _colorrepository;
        private readonly IPackingServiceRepository _packingServiceRepository;
        private const int PageControlID = (int)Common.Pages.Packing; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;
        public PackingModel(IAntiforgery antiforgery, IBoxServiceRepository boxServiceRepository, 
                               IColorServiceRespository colorrepository,
                               IPackingServiceRepository packingServiceRepository, 
                               IHSRPUserServiceRepository hsrpartServiceRepository,
                               IPermissionServiceRepository permissionService,
                               ILogger<PackingModel> _logger)
        {
            _antiforgery = antiforgery;
            _boxServiceRepository = boxServiceRepository;
            _colorrepository = colorrepository;
            _packingServiceRepository = packingServiceRepository;
            _hsrpartServiceRepository = hsrpartServiceRepository;
            _permissionrepository = permissionService;
            logger = _logger;
        }

        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> ColorList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AllotedToList { get; set; } = new List<SelectListItem>();
 
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadColorList();
            LoadAllotedToList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public JsonResult OnGetBoxList()
        {
            DataResponse dataResponse = null;
            dataResponse = _boxServiceRepository.GetBox();
            var Itemlist = ((List<VBox>)dataResponse.Value)
            .OrderBy(o => o.BoxName).ToList();
            return new JsonResult(Itemlist);
        }
        public void LoadColorList()
        {
            DataResponse dataResponse = null;

            ColorList.Clear();
            dataResponse = _colorrepository.GetColor();
            ColorList = ((List<VColor>)dataResponse.Value)
                .OrderBy(o => o.ColorName)
                .Select(s => new SelectListItem
                {
                    Value = s.ColorID.ToString(),
                    Text = s.ColorName
                }).ToList();

            ColorList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Color--" });
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

            AllotedToList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Alloted To--" });
        }

        public JsonResult OnPostPackingStockByFilter([FromBody] PackingFilter PackingFilter)
        {
           var resultdata = _packingServiceRepository.GetAvailableLaserNos(PackingFilter);
            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] Packing request)
        {
            Tuple<bool, int> resultdata = null;
            request.PackingDate = (DateTime)ConvertDate(request.sPackingDate);
            request.LastUpdatedDate = DateTime.Now;
            request.LastUpdatedBy = LoggedUser.UserID;
            resultdata = _packingServiceRepository.Save(request);

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

            var resultdata = (_packingServiceRepository.GetPackingList()).OrderByDescending(w => w.PackingNo).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.PackingNo.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();


            // Handle sorting based on the column index and direction
            Func<VPacking, object> orderByFunc = orderCol switch
            {
                1 => d => d.PackingNo,
                2 => d => d.PackingDate,
                3 => d => d.BoxName,
                4 => d => d.ColorName,
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
        public JsonResult OnGetPackingByID(int ID)
        {
            var resultdata = _packingServiceRepository.GetByID(ID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            Tuple<bool,bool> resultdata = null;

            try
            {
                resultdata = _packingServiceRepository.Delete(ID,LoggedUser.UserID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
