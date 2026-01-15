namespace SVAuroraERP.WebUI.Custom
{  
    [Authorize]
    public class HSRPBasePageModel : PageModel
    {
        protected VHSRPUser HSRPLoggedUser => HttpContext.Items["HSRPLoggedUser"] as VHSRPUser;

        public HSRPBasePageModel()
        {
        }
    }
}