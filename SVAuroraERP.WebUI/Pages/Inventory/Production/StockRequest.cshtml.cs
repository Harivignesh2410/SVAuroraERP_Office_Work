namespace SVAuroraERP.WebUI.Pages.Inventory.Production
{
    public class StockRequestModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IStockRequestServiceRepository _repository;
        private readonly IProcessTypeServiceRepository _processTypeServiceRepository;
        private readonly ISizeServiceRepository _sizeServiceRepository;
        private readonly IColorServiceRespository _colorServiceRespository;
        private readonly IProductionConfigurationServiceRepository _productionConfigurationServiceRepository;
        private readonly IStockRequestTransServiceRepository _stockRequestTransServiceRepository;
        private const int PageControlID = (int)Common.Pages.StockRequest;
        private readonly IPermissionServiceRepository _permissionrepository;
        public StockRequestModel(IAntiforgery antiforgery,
                                 IStockRequestServiceRepository stockReportServiceRepository,
                                 IProcessTypeServiceRepository processTypeServiceRepository,
                                 ISizeServiceRepository sizeServiceRepository,
                                 IColorServiceRespository colorServiceRespository,
                                IProductionConfigurationServiceRepository productionConfigurationServiceRepository,
                                IStockRequestTransServiceRepository stockRequestTransServiceRepository,
                                IPermissionServiceRepository permissionrepository)
        {
            _antiforgery = antiforgery;
            _repository = stockReportServiceRepository;
            _processTypeServiceRepository = processTypeServiceRepository;
            _sizeServiceRepository = sizeServiceRepository;
            _colorServiceRespository = colorServiceRespository;
            _productionConfigurationServiceRepository = productionConfigurationServiceRepository;
            _stockRequestTransServiceRepository = stockRequestTransServiceRepository;
            _permissionrepository = permissionrepository;
        }

        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> ProcessTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ColorList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SizeList { get; set; } = new List<SelectListItem>();
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadProcessTypeList();
            LoadColorList();
            LoadSizeList();

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadProcessTypeList()
        {
            ProcessTypeList.Clear();
            ProcessTypeList = _processTypeServiceRepository.GetProcessTypeList()
                .Select(s => new SelectListItem
                {
                    Value = s.ProcessTypeID.ToString(),
                    Text = s.ProcessTypeName
                }).ToList();

            ProcessTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Process Type--" });
        }
        public void LoadColorList()
        {
            DataResponse dataResponse = null;
            ColorList.Clear();
            dataResponse = _colorServiceRespository.GetColor();
            ColorList = ((List<VColor>)dataResponse.Value).
                            OrderBy(o => o.ColorName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.ColorID.ToString(),
                                 Text = s.ColorName
                             }).ToList();

            ColorList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Color--" });
        }
        public void LoadSizeList()
        {
            DataResponse dataResponse = new DataResponse();
            SizeList.Clear();
            dataResponse = _sizeServiceRepository.GetSize();
            SizeList = ((List<VSize>)dataResponse.Value)
                .OrderBy(o => o.SizeName)
                .Select(s => new SelectListItem
                {
                    Value = s.SizeID.ToString(),
                    Text = s.SizeName
                }).ToList();

            SizeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Size--" });
        }
        public JsonResult OnGetComponentTypeList(int ProcessTypeID)
        {
            var pageList = _productionConfigurationServiceRepository.GetProductionConfigurationByProcessTypeID(ProcessTypeID)
                             .Select(d => new SelectListItem
                             {
                                 Value = d.ComponentTypeID.ToString(),
                                 Text = d.ComponentTypeName
                             }).ToList();

            pageList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Component Type--" });

            return new JsonResult(pageList);
        }
        public JsonResult OnPostBatchStockByFilter([FromBody] BatchStockFilter BatchStockFilter)
        {
            var resultdata = _repository.GetBatchStockByFilter(BatchStockFilter);
            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnGetStockRequestList(int draw, int start, int length)
        {
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            var resultdata = (_repository.GetStockRequest()).OrderByDescending(w=>w.RequestNo).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.RequestNo.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.RequestedByName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VStockRequest, object> orderByFunc = orderCol switch
            {
                1 => d => d.RequestNo,
                2 => d => d.RequestDate,
                3 => d => d.ProcessTypeName,
                4 => d => d.RequestedByName,
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

        public JsonResult OnPostSaveUpdateData([FromBody] StockRequest stockRequest)
        {
            string message = string.Empty;
            Tuple<bool, string> resultSavedata = null;
            Tuple<bool, bool> resultUpdatedata = null;
            try
            {
                stockRequest.RequestDate = (DateTime)ConvertDate(stockRequest.sRequestDate);
                stockRequest.LastUpdatedBy = LoggedUser.UserID;

                if (stockRequest.StockRequestID == 0)
                {
                    stockRequest.RequestedBy = LoggedUser.UserID;
                    resultSavedata = _repository.Save(stockRequest);

                    return new JsonResult(new { success = resultSavedata.Item1, SRRequestNo = resultSavedata.Item2 });
                }
                else if (stockRequest.StockRequestID > 0)
                {
                    resultUpdatedata = _repository.Update(stockRequest);

                    return new JsonResult(new { success = resultUpdatedata.Item1, isExists = resultUpdatedata.Item2 });
                }

                return new JsonResult(new { success = false });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
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
        public JsonResult OnGetStockRequestByID(int ID)
        {
            var resultdata = _repository.GetByID(ID);

            return new JsonResult(new { success = true, data = resultdata });
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

        //Added on 2025.03.14
        public JsonResult OnPostDeleteStockRequestransByID([FromBody] int ID)
        {
            string message = string.Empty;
            int Result = 0;

            try
            {
                Result = _stockRequestTransServiceRepository.DeleteStockRequestTrans(ID);
                return new JsonResult(new { success = Result > 0 ? true : false });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}