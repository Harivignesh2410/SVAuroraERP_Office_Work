namespace SVAuroraERP.WebUI.Pages.Inventory.Production
{
    public class HydrolicPressureModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IStockRequestServiceRepository _stockReportServiceRepository;
        private readonly IProcessTypeServiceRepository _processTypeServiceRepository;
        private readonly ISizeServiceRepository _sizeServiceRepository;
        private readonly IColorServiceRespository _colorServiceRespository;
        private readonly IProductionConfigurationServiceRepository _productionConfigurationServiceRepository;
        private readonly IPendingApprovalFilterServiceRepository _pendingapprovalfilter;
        private readonly IComponentServiceRepository _componentServiceRepository;
        private readonly IItemServiceRepository _itemServiceRepository;
        private readonly IHydrolicPressureServiceRepository _HydrolicPressureServiceRepository;
        private readonly IRackLocationServiceRepository _rackLocationServiceRepository;
        private readonly IHologramPunchingServiceRepository _hologramPunchingServiceRepository;
        private readonly IEmployeeServiceRepository _emprepo;
        private const int PageControlID = (int)Common.Pages.HydrolicPressure;
        private readonly IPermissionServiceRepository _permissionrepository;
        public HydrolicPressureModel(IAntiforgery antiforgery,
                              IStockRequestServiceRepository stockReportServiceRepository,
                              IProcessTypeServiceRepository processTypeServiceRepository,
                              ISizeServiceRepository sizeServiceRepository,
                              IColorServiceRespository colorServiceRespository,
                             IProductionConfigurationServiceRepository productionConfigurationServiceRepository,
                             IPendingApprovalFilterServiceRepository pendingapprovalfilter,
                             IComponentServiceRepository componentServiceRepository,
                             IItemServiceRepository itemServiceRepository,
                             IHydrolicPressureServiceRepository HydrolicPressureServiceRepository,
                             IRackLocationServiceRepository rackLocationServiceRepository,
                             IHologramPunchingServiceRepository hologramPunchingServiceRepository,
                              IEmployeeServiceRepository emprepo,
                               IPermissionServiceRepository permissionrepository)
        {
            _antiforgery = antiforgery;
            _stockReportServiceRepository = stockReportServiceRepository;
            _processTypeServiceRepository = processTypeServiceRepository;
            _sizeServiceRepository = sizeServiceRepository;
            _colorServiceRespository = colorServiceRespository;
            _productionConfigurationServiceRepository = productionConfigurationServiceRepository;
            _pendingapprovalfilter = pendingapprovalfilter;
            _componentServiceRepository = componentServiceRepository;
            _itemServiceRepository = itemServiceRepository;
            _HydrolicPressureServiceRepository = HydrolicPressureServiceRepository;
            _rackLocationServiceRepository = rackLocationServiceRepository;
            _hologramPunchingServiceRepository = hologramPunchingServiceRepository;
            _emprepo = emprepo;
            _permissionrepository = permissionrepository;

        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> ProcessTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ComponentTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SizeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ColorList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> EmployeeList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }

        const string DateFormat = "dd MMM, yyyy";

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadProcessTypeList();
            LoadComponentTypeList();
            LoadSizeList();
            LoadColorList();
            LoadEmployeeList();
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
        public void LoadComponentTypeList()
        {
            DataResponse dataResponse = null;
            ComponentTypeList.Clear();
            dataResponse = _componentServiceRepository.GetComponentList();
            ComponentTypeList = ((List<VComponentType>)dataResponse.Value)
                            .OrderBy(o => o.ComponentTypeName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.ComponentTypeID.ToString(),
                                 Text = s.ComponentTypeName
                             }).ToList();

            ComponentTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
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
        public void LoadEmployeeList()
        {
            DataResponse dataResponse = new DataResponse();
            EmployeeList.Clear();
            dataResponse = _emprepo.GetEmployee();
            EmployeeList = ((List<VEmployee>)dataResponse.Value)
                .Select(s => new SelectListItem
                {
                    Value = s.EmployeeID.ToString(),
                    Text = s.FirstName
                }).ToList();

            EmployeeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Operator--" });
        }
        public JsonResult OnGetStockRequestList(int ID)
        {
            var resultdata = _HydrolicPressureServiceRepository.GetStockRequestList(ID);
            return new JsonResult(new { success = true, data = resultdata });
        }
        private DateOnly? ConvertDateonly(string sdate)
        {
            DateOnly? dtConvertedDate = null;

            const string DateFormat = "dd/MM/yyyy"; // Matches "28/04/2025"

            if (DateOnly.TryParseExact(sdate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsedDt))
            {
                dtConvertedDate = parsedDt;
            }

            return dtConvertedDate;
        }

        private DateTime? ConvertDate(string sdate)
        {

            DateTime? dtConvertedDate = null;

            if (DateTime.TryParseExact(sdate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDt))
            {
                dtConvertedDate = parsedDt;
            }

            return dtConvertedDate;
        }


        private TimeOnly? ConvertTime(string stime)
        {
            TimeOnly? convertedTime = null;
            string[] formats = { "HH:mm", "HH:mm:ss" };

            if (TimeOnly.TryParseExact(stime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly parsedTime))
            {
                convertedTime = parsedTime;
            }

            return convertedTime;
        }

        public JsonResult OnGetDataListStockRequestID(int ID)
        {
            var resultSet = _HydrolicPressureServiceRepository.GetHydraulicDetailsAsync(ID);

            return new JsonResult(new
            {
                success = true,
                data = resultSet
            });
        }

        public JsonResult OnPostUpdateHydrolicPressure([FromBody] HydrolicPressure HydrolicPressure)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;
            try
            {
                HydrolicPressure.ProductionDate = (DateOnly)ConvertDateonly(HydrolicPressure.sProductionDate);
                HydrolicPressure.StartTime = (TimeOnly)ConvertTime(HydrolicPressure.sStartTime);
                HydrolicPressure.EndTime = (TimeOnly)ConvertTime(HydrolicPressure.sEndTime);
                HydrolicPressure.LastUpdatedBy = LoggedUser.UserID;

                if(HydrolicPressure.HydrolicPressureID==0)
                resultdata = _HydrolicPressureServiceRepository.SaveHydrolicPressure(HydrolicPressure);
                else
                 resultdata = _HydrolicPressureServiceRepository.UpdateHydrolicPressure(HydrolicPressure);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnPostItemDropdownByFilte([FromBody] BatchStockFilter batchStockFilter)
        {

            var resultdata = _itemServiceRepository.GetItemByFilter(batchStockFilter);
            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnGetRackLocationDropdownByFilter(int ComponentTypeID)
        {
            var resultdata = _rackLocationServiceRepository.GetRackLocationByComponentID(ComponentTypeID);
            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnGetHologramPunchingByID(int id)
        {
            var resultdata = _hologramPunchingServiceRepository.GetHologramPunchingByID(id);
            return new JsonResult(new { success = true, data = resultdata });
        }
        
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _HydrolicPressureServiceRepository.DeleteHydrolicPressure(ID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnPostCompleteHydrolicPressure([FromBody] int ID)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _HydrolicPressureServiceRepository.CompleteHydrolicPressure(ID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
