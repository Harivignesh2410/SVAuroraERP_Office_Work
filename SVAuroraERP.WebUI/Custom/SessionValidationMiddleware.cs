namespace SVAuroraERP.WebUI.Custom
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SessionValidationMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var sessionService = scope.ServiceProvider.GetRequiredService<SessionService>();
                var loggedUser = sessionService.GetLoggedUser();

                if (loggedUser == null && !context.Request.Path.StartsWithSegments("/SignIn"))
                {
                    // Clear session data
                    context.Session.Clear();
                    context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    context.Response.Redirect("/SignIn");
                    return;
                }

                context.Items["LoggedUser"] = loggedUser;

                if (loggedUser != null && loggedUser.ApplicationID == 2)
                {
                    var loggedHSRPUser = sessionService.GetVHSRPUser();

                    context.Items["HSRPLoggedUser"] = loggedHSRPUser;
                }
            }

            await _next(context);
        }
    }
}