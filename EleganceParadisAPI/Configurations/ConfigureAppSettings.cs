using ApplicationCore.Settings;
using Microsoft.Extensions.Options;

namespace EleganceParadisAPI.Configurations
{
    public static class ConfigureAppSettings
    {
        public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailServerOptions>(configuration.GetSection(MailServerOptions.MailServerSettings));

            services
                .Configure<SendEmailSettings>(configuration.GetSection(SendEmailSettings.SendEmailSettingsKey))
                .AddSingleton(provider => provider.GetRequiredService<IOptions<SendEmailSettings>>().Value);

            services.Configure<LinePaySettings>(configuration.GetRequiredSection(LinePaySettings.LinePaySettingsKey))
                            .AddSingleton(provider => provider.GetRequiredService<IOptions<LinePaySettings>>().Value);

            services.Configure<AdminInfoSettings>(configuration.GetRequiredSection(AdminInfoSettings.AdminInfoSettingsKey))
                            .AddSingleton(provider => provider.GetRequiredService<IOptions<AdminInfoSettings>>().Value);

            return services;
        }
    }
}
