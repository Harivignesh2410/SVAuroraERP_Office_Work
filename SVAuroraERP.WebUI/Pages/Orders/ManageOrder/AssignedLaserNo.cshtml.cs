namespace SVAuroraERP.WebUI.Pages.Orders.ManageOrder
{
    public class AssignedLaserNoModel : HSRPBasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly INumberPlateDispatchServiceRepository _numberPlateDispatchServiceRepository;
        private readonly ILogger<AssignedLaserNoModel> logger = null;
        private const int PageControlID = (int)Common.Pages.AssignedLaserNo; 
        private readonly IPermissionServiceRepository _permissionrepository;
        private readonly IHSRPLaserNoStockServiceRepository _hSRPLaserNoStockServiceRepository;

        public AssignedLaserNoModel(IAntiforgery antiforgery,
                            INumberPlateDispatchServiceRepository numberPlateDispatchServiceRepository,
                         IPermissionServiceRepository permissionService,
                          ILogger<AssignedLaserNoModel> _logger,
                          IHSRPLaserNoStockServiceRepository hSRPLaserNoStockServiceRepository)
        {
            _antiforgery = antiforgery;
            _numberPlateDispatchServiceRepository = numberPlateDispatchServiceRepository;
            _permissionrepository = permissionService;
            logger = _logger;
            _hSRPLaserNoStockServiceRepository = hSRPLaserNoStockServiceRepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");
            if (HSRPLoggedUser.HSRPUserTypeID != (byte)Common.HSRPUserType.EmbossingStation && HSRPLoggedUser.HSRPUserTypeID != (byte)Common.HSRPUserType.Admin)
            {
                return RedirectToPage("/Orders/ViewOrders/ViewAssignedLaserNo");
            }
            return Page();
        }
        public JsonResult OnPostAssignedLaserNoList([FromForm] HSRPLaserNoStockFilterData dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "DispatchNo", "EmbossingStationName", "Dimension", "StockInsertedDate" , "SerialNo" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "StockInsertedDate";

                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.EmbossingStation)
                {
                    dataTableRequest.EmbossingStationID = HSRPLoggedUser.HSRPUserID;
                }

                DataResponse dataResponse = new DataResponse();
                dataResponse = _hSRPLaserNoStockServiceRepository.GetLaserNoStockDataTable(dataTableRequest);

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
                return new JsonResult(new { Success = false, Message = ex.Message });
            }
        }
        public JsonResult OnPostLaserNoStockSummary([FromBody] HSRPLaserNoStockFilterData request)
        {
            try
            {
                DataResponse dataResponse = new DataResponse();
                request.EmbossingStationID = HSRPLoggedUser.HSRPUserID;
                dataResponse = _hSRPLaserNoStockServiceRepository.GetLaserStockSummary(request);

                return new JsonResult(new
                {
                    Success = true,
                    data = dataResponse.Value
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Success = false, Message = ex.Message });
            }
        }
        public JsonResult OnGetLaserNoStockLogByID(int ID)
        {
            DataResponse resultdata = _hSRPLaserNoStockServiceRepository.GetHSRPLaserNoStockLogByID(ID);

            return new JsonResult(resultdata);
        }
    }
}