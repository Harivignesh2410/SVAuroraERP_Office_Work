namespace SVAuroraERP.WebUI.Pages
{
    [Authorize]
    public class SignoutModel : BasePageModel
    {
        private readonly IAuthenticaionServiceRepository _authenticaionServiceRepository;
        public SignoutModel(IAuthenticaionServiceRepository authenticaionServiceRepository)
        {
            _authenticaionServiceRepository = authenticaionServiceRepository;
        }
        public void OnGet()
        {
            if (LoggedUser != null)
            {
                _authenticaionServiceRepository.SignOut(LoggedUser.LoginAuditID);
            }

            // Clear session data
            HttpContext.Session.Clear();

            // Optionally, clear authentication cookies if using authentication
            Response.Cookies.Delete(".AspNetCore.Session");
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            Response.Cookies.Delete(".AspNetCore.DataProtection");
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}