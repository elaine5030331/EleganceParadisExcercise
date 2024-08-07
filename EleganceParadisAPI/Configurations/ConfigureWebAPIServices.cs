using ApplicationCore.Services;
using EleganceParadisAPI.Services;
using Infrastructure.Data.Services;
using Infrastructure.Schedules;

namespace EleganceParadisAPI.Configurations
{
    public static class ConfigureWebAPIServices
    {
        public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
        {
            services.AddTransient<OrderInvalidJob>();
            return services;
        }
    }
}
