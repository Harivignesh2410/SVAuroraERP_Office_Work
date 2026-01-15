namespace SVAuroraERP.WebUI.Custom
{
    [Authorize]
    public class BasePageModel : PageModel
    {
        protected UserLoginData LoggedUser => HttpContext.Items["LoggedUser"] as UserLoginData;
        protected VHSRPUser HSRPLoggedUser => HttpContext.Items["HSRPLoggedUser"] as VHSRPUser;

        public BasePageModel()
        {
        }
    }
}