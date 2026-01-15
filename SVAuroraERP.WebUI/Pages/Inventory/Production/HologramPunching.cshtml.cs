namespace SVAuroraERP.WebUI.Pages.Inventory.Production
{
    public class HologramPunchingModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHologramPunchingServiceRepository _repository;
        private readonly IEmployeeServiceRepository _emprepo;
        private readonly IMachineServiceRepository _machinerepo;
        private readonly IWareHouseServiceRepository _warehouse;
        private readonly IItemServiceRepository _itemServiceRepository;
        private readonly IComponentServiceRepository _componentServiceRepository;
        private readonly ISizeServiceRepository _sizeServiceRepository;
        private readonly IColorServiceRespository _colorServiceRespository;
        private readonly IRackLocationServiceRepository _rackLocationServiceRepository;
        private readonly IHydrolicPressureServiceRepository _hydrolicPressureServiceRepository;
        private const int PageControlID = (int)Common.Pages.HologramPunching;
        private readonly IPermissionServiceRepository _permissionrepository;

        public HologramPunchingModel(IAntiforgery antiforgery,
                                     IHologramPunchingServiceRepository repository,
                                     IEmployeeServiceRepository emprepo,
                                     IMachineServiceRepository machinerepo,
                                     IWareHouseServiceRepository warehouse,
                                      IItemServiceRepository itemServiceRepository,
                                       ISizeServiceRepository sizeServiceRepository,
                              IColorServiceRespository colorServiceRespository,
                              IComponentServiceRepository componentServiceRepository,
                              IRackLocationServiceRepository rackLocationServiceRepository,
                              IHydrolicPressureServiceRepository hydrolicPressureServiceRepository,
                               IPermissionServiceRepository permissionrepository)
        {
            _antiforgery = antiforgery;
            _repository = repository;
            _emprepo = emprepo;
            _machinerepo = machinerepo;
            _warehouse = warehouse;
            _itemServiceRepository = itemServiceRepository;
            _sizeServiceRepository = sizeServiceRepository;
            _colorServiceRespository = colorServiceRespository;
            _componentServiceRepository = componentServiceRepository;
            _rackLocationServiceRepository = rackLocationServiceRepository;
            _hydrolicPressureServiceRepository = hydrolicPressureServiceRepository;
            _permissionrepository = permissionrepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> MachineTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> EmployeeList { get; set; } = new List<SelectListItem>();
        //public List<SelectListItem> ItemList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ComponentTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SizeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ColorList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        const string DateFormat = "dd MMM, yyyy";
    
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadMachineTypeList();
            LoadEmployeeList();
            //LoadItemList();
            LoadComponentTypeList();
            LoadColorList();
            LoadSizeList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public JsonResult OnGetStockRequestList(int ID)
        {
            var resultdata = _repository.GetStockRequestList(ID);
            return new JsonResult(new { success = true, data = resultdata });
        }
        public void LoadMachineTypeList()
        {
            DataResponse dataResponse = null;
            MachineTypeList.Clear();
            dataResponse = _machinerepo.GetMachineList();
            MachineTypeList = ((List<VMachine>)dataResponse.Value).Where(w => w.MachineTypeID == 1)
                .Select(s => new SelectListItem
                {
                    Value = s.MachineID.ToString(),
                    Text = s.MachineName,
                }).ToList();

            MachineTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Machine Type --" });
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
        //public void LoadItemList()
        //{
        //    DataResponse dataResponse = null;
        //    ItemList.Clear();
        //    dataResponse = _itemServiceRepository.GetItem();
        //    ItemList = ((List<VItem>)dataResponse.Value)
        //        .Select(s => new SelectListItem
        //        {
        //            Value = s.ItemID.ToString(),
        //            Text = s.ItemName,
        //        }).ToList();

        //    ItemList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Item --" });
        //}
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
        public JsonResult OnGetWarehouseTabList()
        {
            var resultdata = _repository.GetWarehouseList();
            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnGetHologramPunchingByWarehouseID(int ID, int ComponentTypeID)
        {
            var resultdata = _repository.GetHologramPunchingByWarehouseID(ID, ComponentTypeID);
            return new JsonResult(new { success = true, data = resultdata });
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
        public JsonResult OnPostUpdateHologramPunching([FromBody] HologramPunching HologramPunching)
        {
            string message = string.Empty;
            Tuple<bool, bool, bool> resultdata = null;
            try
            {
                HologramPunching.ProductionDate = (DateOnly)ConvertDateonly(HologramPunching.sProductionDate);
                HologramPunching.StartTime = (TimeOnly)ConvertTime(HologramPunching.sStartTime);
                HologramPunching.EndTime = (TimeOnly)ConvertTime(HologramPunching.sEndTime);
                HologramPunching.LastUpdatedBy = LoggedUser.UserID;

                if (HologramPunching.HologramPunchingID == 0)
                    resultdata = _repository.SaveHologramPunching(HologramPunching);
                else
                    resultdata = _repository.UpdateHologramPunching(HologramPunching);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2, ProductionCompleted = resultdata.Item3 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnGetDataListStockRequestID(int ID, int StockRequestID)
        {
            var resultSet = _repository.GetHologramDetailsAsync(ID, StockRequestID);

            return new JsonResult(new
            {
                success = true,
                data = resultSet
            });
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _repository.DeleteHologramPunching(ID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnPostCompleteHologramPunching([FromBody] int ID)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _repository.CompleteHologramPunching(ID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnGetHydrolicPressureByID(int ID)
        {
            var resultSet = _hydrolicPressureServiceRepository.GetHydrolicPressureByID(ID);

            return new JsonResult(new
            {
                success = true,
                data = resultSet
            });
        }
        public JsonResult OnGetHologramPunchingCompleted()
        {
            var resultdata = _repository.GetHologramPunchingCompleted();
            return new JsonResult(new { success = true, data = resultdata });
        }
    }
}
