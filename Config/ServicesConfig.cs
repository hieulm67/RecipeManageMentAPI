using Microsoft.Extensions.DependencyInjection;
using RecipeManagementBE.Service;
using RecipeManagementBE.Service.Impl;
using Scrutor;

namespace RecipeManagementBE.Config {
    public static class ServicesConfig {
        public static IServiceCollection AddServicesConfiguration(this IServiceCollection services) {
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Program))
                .AddClasses(classes => classes.Where(type => type.Name.Contains("Service")))
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.All))
                .AsImplementedInterfaces()
                // .WithSingletonLifetime()
                .WithScopedLifetime()
            );

            services.AddSingleton<IFirebaseService, FirebaseService>();
            return services;
        }
    }
}