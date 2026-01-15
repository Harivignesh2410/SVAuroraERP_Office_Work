namespace SVAuroraERP.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<MenuService>();

            return services;
        }
    }
}