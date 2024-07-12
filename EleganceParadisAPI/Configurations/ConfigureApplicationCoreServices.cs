using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using EleganceParadisAPI.Services;
using Infrastructure.Data;
using Infrastructure.Data.Services;

namespace EleganceParadisAPI.Configurations
{
    public static class ConfigureApplicationCoreServices
    {
        public static IServiceCollection AddApplicationCoreServices(this IServiceCollection services) 
        {
            services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddTransient<IProductQueryService, ProductQueryService>();
            return services;
        }
    }
}
