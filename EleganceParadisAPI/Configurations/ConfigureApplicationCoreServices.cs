using ApplicationCore.Interfaces;
using Infrastructure.Data;

namespace EleganceParadisAPI.Configurations
{
    public static class ConfigureApplicationCoreServices
    {
        public static IServiceCollection AddApplicationCoreServices(this IServiceCollection services) 
        {
            services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

            return services;
        }
    }
}
