using Core.Logging.Models;
using SVAuroraERP.Application.Interfaces;

namespace SVAuroraERP.WebUI.Pages
{
    public class SignInModel(IAuthenticaionServiceRepository authenticaionServiceRepository,
                             ILogger<SignInModel> logger,
                             AppVersionService appVersionService,
                             IHSRPUserServiceRepository hSRPUserServiceRepository, // Added on 2025.9.24
                             IAntiforgery antiforgery) : PageModel
    {
        private readonly IAuthenticaionServiceRepository _authenticaionServiceRepository = authenticaionServiceRepository;
        private readonly IHSRPUserServiceRepository _hSRPUserServiceRepository = hSRPUserServiceRepository; // Added on 2025.9.24
        private readonly ILogger<SignInModel> _logger = logger;
        private readonly AppVersionService _appVersionService = appVersionService;
        private readonly IAntiforgery _antiforgery = antiforgery;

        public string AppVersion { get; private set; } = string.Empty;
        public string AppEdition { get; private set; } = string.Empty; //Added on 2025.02.26
        public List<SelectListItem> ApplicationList { get; set; } = new List<SelectListItem>(); // Added on 2025.9.24
        [BindProperty] public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public class InputModel
        {
            [Required] public string Username { get; set; } = string.Empty;
            [Required][DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        }
        public string? AntiforgeryToken { get; private set; }

        public void OnGet(string returnUrl = null)
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view

            AppVersion = _appVersionService.GetAppVersion();
            AppEdition = _appVersionService.GetAppEdition();

            // Capture the ReturnUrl
            ReturnUrl = returnUrl;
            //LoadApplicationList();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public JsonResult OnPostUserSignIn([FromBody] LoginAudit logindata)
        {
            //if (!ModelState.IsValid) { return Page(); }

            // Retrieve the IP address
            logindata.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            Console.WriteLine("Device Type: " + logindata.DeviceType);
            Console.WriteLine("OS Version: " + logindata.OSName);

            string? devicetype = logindata.DeviceType;

            var userData = _authenticaionServiceRepository.SignIn(logindata).Result;

            if (userData != null)
            {
                // Cache the user's ID and role information
                var cacheKey = $"UserPermissions_User_{userData.UserID}";

                // Store the serialized object in session
                string jsonData = JsonConvert.SerializeObject(userData);
                HttpContext.Session.SetObjectAsJson("UserLoggedData", userData);

                //Added on 2025.11.06
                if (userData.ApplicationID == (byte)Common.Application.HSRPPortal)
                {
                    if (userData.HSRPUser == null)
                    {
                        _logger.LogWarning("HSRP User data is null for UserID: {UserID}", userData.UserID);
                        return new JsonResult(new { success = false, message = "HSRP User data not found. Please contact support." });
                    }

                    VHSRPUser localhsrpUser =(VHSRPUser)userData.HSRPUser;
                    HttpContext.Session.SetObjectAsJson("VHSRPUserData", localhsrpUser);
                }

                // Create the identity and sign in
                var claims = new[] {
                    new Claim(ClaimTypes.Name, userData.UserName),
                    new Claim(ClaimTypes.Role, userData.RoleID.ToString()),
                    new Claim("UserId", userData.UserID.ToString()),
                    new Claim("RoleId", userData.RoleID.ToString()),
                    new Claim("LoginAuditID", userData.LoginAuditID.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userData.UserID.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect to the ReturnUrl if it's a local URL
                if (!string.IsNullOrEmpty(logindata.ReturnURL) && logindata.ReturnURL != "/" && Url.IsLocalUrl(logindata.ReturnURL))
                {
                    return new JsonResult(new { success = true, redirectpage = logindata.ReturnURL });
                }
                else
                {
                    return new JsonResult(new { success = true, redirectpage = userData.PageURL });
                }
            }
            else
                return new JsonResult(new { success = false, message = "Invalid username or password. Please try again." });
        }
        // Added on 2025.9.24
        //public void LoadApplicationList()
        //{
        //    DataResponse dataResponse = new DataResponse();
        //    ApplicationList.Clear();
        //    dataResponse = _hSRPUserServiceRepository.GetApplication();
        //    ApplicationList = ((List<LkupApplication>)dataResponse.Value)
        //        .OrderBy(o => o.ApplicationName)
        //        .Select(s => new SelectListItem
        //        {
        //            Value = s.ApplicationID.ToString(),
        //            Text = s.ApplicationName
        //        }).ToList();

        //    ApplicationList.Insert(0, new SelectListItem { Value = "0", Text = "--Select User Type--" });
        //}
    }
}