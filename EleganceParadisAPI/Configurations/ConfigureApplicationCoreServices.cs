using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using EleganceParadisAPI.Services;
using Infrastructure.Data;
using Infrastructure.Data.Services;
using Infrastructure.Services;

namespace EleganceParadisAPI.Configurations
{
    public static class ConfigureApplicationCoreServices
    {
        public static IServiceCollection AddApplicationCoreServices(this IServiceCollection services) 
        {
            services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddTransient<IProductQueryService, ProductQueryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserManageService, UserManageService>();
            services.AddScoped<IApplicationPasswordHasher, ApplicationPasswordHasherService>();
            services.AddScoped<ISpecService, SpecService>();
            services.AddScoped<IUploadImageService, CloudinaryService>();
            return services;
        }
    }
}
