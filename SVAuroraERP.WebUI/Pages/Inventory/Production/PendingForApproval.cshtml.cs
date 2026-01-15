namespace SVAuroraERP.WebUI.Pages.Inventory.Production
{
    public class PendingForApprovalModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IStockRequestServiceRepository _stockReportServiceRepository;
        private readonly IProcessTypeServiceRepository _processTypeServiceRepository;
        private readonly ISizeServiceRepository _sizeServiceRepository;
        private readonly IColorServiceRespository _colorServiceRespository;
        private readonly IProductionConfigurationServiceRepository _productionConfigurationServiceRepository;
        private readonly IPendingApprovalFilterServiceRepository _pendingapprovalfilter;
        private const int PageControlID = (int)Common.Pages.PendingForApproval;
        private readonly IPermissionServiceRepository _permissionrepository;


        public PendingForApprovalModel(IAntiforgery antiforgery,
                                 IStockRequestServiceRepository stockReportServiceRepository,
                                 IProcessTypeServiceRepository processTypeServiceRepository,
                                 ISizeServiceRepository sizeServiceRepository,
                                 IColorServiceRespository colorServiceRespository,
                                IProductionConfigurationServiceRepository productionConfigurationServiceRepository,
                                IPendingApprovalFilterServiceRepository pendingapprovalfilter,
                                IPermissionServiceRepository permissionrepository)
        {
            _antiforgery = antiforgery;
            _stockReportServiceRepository = stockReportServiceRepository;
            _processTypeServiceRepository = processTypeServiceRepository;
            _sizeServiceRepository = sizeServiceRepository;
            _colorServiceRespository = colorServiceRespository;
            _productionConfigurationServiceRepository = productionConfigurationServiceRepository;
            _pendingapprovalfilter= pendingapprovalfilter;
            _permissionrepository = permissionrepository;
        }

        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> ProcessTypeList { get; set; } = new List<SelectListItem>();
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
        public JsonResult OnPostPendingForApprovalByFilter([FromBody] PendingApprovalFilter PendingApprovalFilter)
        {
            if (!string.IsNullOrEmpty(PendingApprovalFilter.sStartDate)) PendingApprovalFilter.StartDate = (DateTime)ConvertDate(PendingApprovalFilter.sStartDate);
            if (!string.IsNullOrEmpty(PendingApprovalFilter.sEndDate)) PendingApprovalFilter.EndDate = (DateTime)ConvertDate(PendingApprovalFilter.sEndDate);
            var resultdata = _pendingapprovalfilter.GetPendingApprovalByFilter(PendingApprovalFilter);
            return new JsonResult(new { success = true, data = resultdata });
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
        public JsonResult OnPostSaveApproval([FromBody] ApprovalRequest request)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                request.LastUpdatedBy = LoggedUser.UserID;

                resultdata = _pendingapprovalfilter.ApproveorRejectStockRequest(request);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
