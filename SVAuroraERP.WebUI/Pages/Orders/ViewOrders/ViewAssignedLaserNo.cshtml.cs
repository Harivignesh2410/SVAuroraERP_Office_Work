namespace SVAuroraERP.WebUI.Pages.Orders.ViewOrders
{
    public class ViewAssignedLaserNoModel : HSRPBasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly INumberPlateDispatchServiceRepository _numberPlateDispatchServiceRepository;
        private readonly ILogger<ViewAssignedLaserNoModel> logger = null;
        private const int PageControlID = (int)Common.Pages.ViewAssignedLaserNo;
        private readonly IPermissionServiceRepository _permissionrepository;
        private readonly IHSRPLaserNoStockServiceRepository _hSRPLaserNoStockServiceRepository;

        public ViewAssignedLaserNoModel(IAntiforgery antiforgery,
                            INumberPlateDispatchServiceRepository numberPlateDispatchServiceRepository,
                         IPermissionServiceRepository permissionService,
                          ILogger<ViewAssignedLaserNoModel> _logger,
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

            // Normal logic here
            return Page();
        }
        public JsonResult OnPostAssignedLaserNoList([FromForm] HSRPLaserNoStockFilterData dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "DispatchNo", "EmbossingStationName", "Dimension", "StockInsertedDate" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "StockInsertedDate";

                DataResponse dataResponse = new DataResponse();
                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.EmbossingStation)
                {
                    dataTableRequest.EmbossingStationID = HSRPLoggedUser.HSRPUserID;
                }
          
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
