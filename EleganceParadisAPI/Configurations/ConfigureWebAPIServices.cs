using EleganceParadisAPI.Services;

namespace EleganceParadisAPI.Configurations
{
    public static class ConfigureWebAPIServices
    {
        public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
        {
            //services.AddScoped<CategoryService, CategoryService>();
            services.AddScoped<CategoryService>();
            return services;
        }
    }
}
