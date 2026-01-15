using SVAuroraERP.Application.Interfaces;

namespace SVAuroraERP.WebUI.Custom
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAppUserContext, AppUserContext>();

            return services;
        }
    }
}