using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeManagementBE.Firebase;
using RecipeManagementBE.Mail;
using RecipeManagementBE.Security;

namespace RecipeManagementBE.Config {
    public static class AppSettingConfig {
        
        public static IServiceCollection AddAppSettingsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Use this to load settings from appSettings file
            services.Configure<SecuritySettings>(options => configuration.GetSection("security").Bind(options));
            services.Configure<MailMetadata>(options => configuration.GetSection("emailMetadata").Bind(options));
            services.Configure<FirebaseMetadata>(options => configuration.GetSection("firebase").Bind(options));

            return services;
        }
    }
}