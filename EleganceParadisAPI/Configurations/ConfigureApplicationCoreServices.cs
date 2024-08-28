using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.AdminInterfaces;
using ApplicationCore.Services;
using ApplicationCore.Services.AdminServices;
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
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUnitOfWork, EFUnitOfWork>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddTransient<IProductQueryService, ProductQueryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserManageService, UserManageService>();
            services.AddScoped<IApplicationPasswordHasher, ApplicationPasswordHasherService>();
            services.AddScoped<ISpecService, SpecService>();
            services.AddScoped<IUploadImageService, CloudinaryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IEmailSender, MailKitService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAdminAccountService, AdminAccountService>();
            return services;
        }
    }
}
