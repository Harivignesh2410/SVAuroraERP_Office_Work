namespace SVAuroraERP.WebUI.Pages.Inventory.Master
{
    [Authorize]
    public class CompanyModel : BasePageModel
    {
        private readonly ICompanyServiceRepository _repository = null;
        private readonly ILogger<CompanyModel> _logger = null;
        private const int PageControlID = (int)Common.Pages.Company; // ID for this specific page
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        public CompanyModel(ICompanyServiceRepository respository,
                           ILogger<CompanyModel> logger,
                           IAntiforgery antiforgery, IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
        }
        public async Task<JsonResult> OnPostSaveUpdateData([FromBody] Company CompanyData)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                CompanyData.LastUpdatedBy = LoggedUser.UserID;
                if (CompanyData.CompanyID >= 0)
                    resultdata = _repository.Save(CompanyData);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }

        public JsonResult OnGetCompany()
        {
            DataResponse resultdata = null;
            resultdata = _repository.GetCompany();

            return new JsonResult(resultdata.Value);
        }
    }
}
