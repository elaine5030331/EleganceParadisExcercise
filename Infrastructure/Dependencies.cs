using CloudinaryDotNet;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;


namespace Infrastructure
{
    public static class Dependencies
    {
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<EleganceParadisContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("EleganceParadisDB")));
            services.AddTransient<IDbConnection>(sp => new SqlConnection(configuration.GetConnectionString("EleganceParadisDB")));
            //services.AddIdentityCore<Account>(); → 包含所有IdentityCore的實作
            services.AddScoped<IPasswordHasher<ApplicationCore.Entities.Account>, PasswordHasher<ApplicationCore.Entities.Account>>();

        }

        public static void ConfigureCloudinaryService(IConfiguration configuration, IServiceCollection services)
        {
            var cloudName = configuration["CloudinarySettings:CloudName"]!;
            var apiKey = configuration["CloudinarySettings:ApiKey"]!;
            var apiSecret = configuration["CloudinarySettings:ApiSecret"]!;

            if (new List<string> { cloudName, apiKey, apiSecret }.Any(x => string.IsNullOrEmpty(x)))
            {
                throw new ArgumentException("Cloudinary parameter is wrong");
            }

            services.AddSingleton(new Cloudinary(new CloudinaryDotNet.Account
            {
                Cloud = cloudName,
                ApiKey = apiKey,
                ApiSecret = apiSecret
            }));

        }

    }
}
