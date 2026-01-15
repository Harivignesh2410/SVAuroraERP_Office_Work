using SVAuroraERP.Domain.Inventory.Master;

namespace SVAuroraERP.WebUI.Pages
{
    public class AppLauncherModel : BasePageModel
    {
        private readonly IAppLauncherServiceRepository _repository = null;
        private readonly IRoleConfigurationServiceRepository _roleconfig = null;
        private readonly ILogger<Color> logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.AppLauncher;
        private readonly IPermissionServiceRepository _permissionrepository;
        //private readonly IHubContext<NotificationHub> _hubContext;

        public AppLauncherModel(IAppLauncherServiceRepository respository,
                          ILogger<Color> _logger,
                          IAntiforgery antiforgery,
                          SessionService sessionService,
                          IRoleConfigurationServiceRepository roleconfig,
                          IPermissionServiceRepository permissionrepository
                          )
        //IHubContext<NotificationHub> hubContext
        {
            _repository = respository;
            logger = _logger;
            _antiforgery = antiforgery;
            _roleconfig = roleconfig;
            _permissionrepository = permissionrepository;
            //_hubContext = hubContext;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public async Task OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            CurrentPageControlID = PageControlID;
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);

            //await _hubContext.Clients.All.SendAsync("ReceiveNotification", LoggedUser.UserName + " Logged In Successfully");
        }
        public JsonResult OnGetAppLauncherByUserID()
        {
            var resultdata = _repository.GetByUserID(LoggedUser.UserID);

            return new JsonResult(new { success = true, data = resultdata });
        }
    }
}