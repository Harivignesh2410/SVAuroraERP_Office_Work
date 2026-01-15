namespace SVAuroraERP.WebUI.Pages.Inventory.Production
{
    public class ProductionCalculationModel : BasePageModel
    {
        private readonly IProductionCalculationServiceRepository _repository;
        private readonly IComponentServiceRepository _componentRepository;
        private readonly ISizeServiceRepository _sizeServiceRepository;
        private readonly IUnitServiceRespository _unitRepository;
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger<ProductionCalculationModel> _logger;
        private const int PageControlID = (int)Common.Pages.ProductionCalculation; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;

        public ProductionCalculationModel(IProductionCalculationServiceRepository ProductionCalculationServiceRepository,
                        ISizeServiceRepository sizeServiceRepository,
                        IAntiforgery antiforgery,
                         IPermissionServiceRepository permissionService,
                          ILogger<ProductionCalculationModel> logger,
                          IComponentServiceRepository componentRepository,
                          IUnitServiceRespository unitRepository
                         )
        {
            _repository = ProductionCalculationServiceRepository;
            _sizeServiceRepository = sizeServiceRepository;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
            _logger = logger;
            _unitRepository = unitRepository;
            _componentRepository = componentRepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> SizeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ComponentTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> UnitList { get; set; } = new List<SelectListItem>();

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadSizeList();
            LoadComponentTypeList();
            LoadUnitList();

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadSizeList()
        {
            DataResponse dataResponse = null;
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
        public void LoadComponentTypeList()
        {
            DataResponse dataResponse = null;
            ComponentTypeList.Clear();
            dataResponse = _componentRepository.GetComponentList();
            ComponentTypeList = ((List<VComponentType>)dataResponse.Value)
                .OrderBy(o => o.ComponentTypeName)
                .Select(s => new SelectListItem
                {
                    Value = s.ComponentTypeID.ToString(),
                    Text = s.ComponentTypeName
                }).ToList();

            ComponentTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Component Type--" });
        }
        public void LoadUnitList()
        {
            DataResponse dataResponse = null;
            UnitList.Clear();
            dataResponse = _unitRepository.GetUnit();
            UnitList = ((List<VUnit>)dataResponse.Value)
                .OrderBy(o => o.UnitName)
                .Select(s => new SelectListItem
                {
                    Value = s.UnitID.ToString(),
                    Text = s.UnitName
                }).ToList();

            UnitList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Unit --" });
        }

        public JsonResult OnPostSaveUpdateData([FromBody] ProductionCalculation ProductionCalculationData)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                ProductionCalculationData.LastUpdatedBy = LoggedUser.UserID;

                if (ProductionCalculationData.ProductionCalculationID == 0)
                    resultdata = _repository.Save(ProductionCalculationData);
                else if (ProductionCalculationData.ProductionCalculationID > 0)
                    resultdata = _repository.Update(ProductionCalculationData);

                return new JsonResult(new { result = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = resultdata });
            }
        }
        public JsonResult OnGetProductionCalculationByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();

            dataResponse = _repository.GetByID(ID);

            return new JsonResult(dataResponse.Value);
        }

        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

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
        public JsonResult OnPostProductionCalculationtoDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Validate sort column
                var validColumns = new[] { "ComponentTypeName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "ComponentTypeName";

                dataResponse = _repository.GetProductionCalculationtoDataTable(dataTableRequest);

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = dataResponse.recordsTotal,
                    recordsFiltered = dataResponse.recordsFiltered,
                    //data = (List<VJournalEntry>)dataResponse.Value
                    data = dataResponse.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Production Calculation data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VProductionCalculation>()
                });
            }
        }
    }
}
