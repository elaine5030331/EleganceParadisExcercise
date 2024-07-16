using ApplicationCore.Services;
using EleganceParadisAPI.Services;
using Infrastructure.Data.Services;

namespace EleganceParadisAPI.Configurations
{
    public static class ConfigureWebAPIServices
    {
        public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
        {
            services.AddScoped<AccountService>();
            return services;
        }
    }
}
