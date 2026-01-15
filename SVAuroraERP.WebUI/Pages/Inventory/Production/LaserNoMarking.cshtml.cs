namespace SVAuroraERP.WebUI.Pages.Inventory.Production
{
    public class LaserNoMarkingModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly ILaserNoMarkingServiceRepository _repository;
        private readonly IEmployeeServiceRepository _emprepo;
        private readonly IMachineServiceRepository _machinerepo;
        private readonly IHologramPunchingServiceRepository _hologramPunching;
        private readonly IWareHouseServiceRepository _warehouse;
        private readonly IItemServiceRepository _itemServiceRepository;
        private readonly IComponentServiceRepository _componentServiceRepository;
        private readonly ISizeServiceRepository _sizeServiceRepository;
        private readonly IColorServiceRespository _colorServiceRespository;
        private readonly IRackLocationServiceRepository _rackLocationServiceRepository;
        private const int PageControlID = (int)Common.Pages.Supplier;
        private readonly IPermissionServiceRepository _permissionrepository;
        public LaserNoMarkingModel(IAntiforgery antiforgery,
                                     ILaserNoMarkingServiceRepository repository,
                                     IEmployeeServiceRepository emprepo,
                                     IMachineServiceRepository machinerepo,
                                     IWareHouseServiceRepository warehouse,
                                     IHologramPunchingServiceRepository hologramPunching,
                                     IItemServiceRepository itemServiceRepository,
                                     ISizeServiceRepository sizeServiceRepository,
                                     IColorServiceRespository colorServiceRespository,
                                     IComponentServiceRepository componentServiceRepository,
                                     IRackLocationServiceRepository rackLocationServiceRepository,
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
            _hologramPunching = hologramPunching;
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
 

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadMachineTypeList();
            LoadEmployeeList();
            LoadComponentTypeList();
            LoadColorList();
            LoadSizeList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public JsonResult OnGetLaserNoMarkingList()
        {
            var resultdata = _hologramPunching.GetHologramPunchingCompleted();

            return new JsonResult(new { success = true, data = resultdata });
        }
        public void LoadMachineTypeList()
        {
            DataResponse dataResponse = null;
            MachineTypeList.Clear();
            dataResponse = _machinerepo.GetMachineList();
            MachineTypeList = ((List<VMachine>)dataResponse.Value).Where(w => w.MachineTypeID == 2)
                .Select(s => new SelectListItem
                {
                    Value = s.MachineTypeID.ToString(),
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
        public JsonResult OnGetHologramPuchingDetailsByBatchID(int ID)
        {
            var resultSet = _repository.GetLaserNoAsync(ID);

            return new JsonResult(new
            {
                success = true,
                data = resultSet
            });
        }
        public JsonResult OnPostSaveLaserNoMarking([FromBody] LaserNoMarking laserNoMarking)
        {
            try
            {
                laserNoMarking.ProductionDate = (DateOnly)ConvertDateonly(laserNoMarking.sProductionDate);
                laserNoMarking.StartTime = (TimeOnly)ConvertTime(laserNoMarking.sStartTime);
                laserNoMarking.EndTime = (TimeOnly)ConvertTime(laserNoMarking.sEndTime);
                laserNoMarking.LastUpdatedBy = LoggedUser.UserID;

                UpdateResult resultdata;
                if (laserNoMarking.LaserNoMarkingID == 0)
                {
                    resultdata = _repository.SaveLaserNoMarking(laserNoMarking);
                }
                else
                {
                    resultdata = _repository.UpdateLaserNoMarking(laserNoMarking);
                }

                return new JsonResult(new
                {
                    success = resultdata.IsSuccess,
                    isExists = resultdata.IsError,
                    message = resultdata.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    isExists = true,
                    message = "Exception: " + ex.Message
                });
            }
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _repository.DeleteLaserNoMarking(ID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnPostCompleteLaserNoMarking([FromBody] int ID)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                resultdata = _repository.CompleteLaserNoMarking(ID);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnGetLaserNoMarkingCompleted()
        {
            var resultdata = _repository.GetLaserNoMarkingCompleted();
            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnGetLaserNoMarkingNxtNo()
        {
            var resultdata = _repository.GetLaserNoMarkingNxtNo();
            return new JsonResult(new { success = true, data = resultdata });
        }

    }
}
