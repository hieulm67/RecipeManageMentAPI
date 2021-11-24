using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace RecipeManagementBE.Config {
    public static class RepositoryConfig {
        public static IServiceCollection AddRepositoryConfiguration(this IServiceCollection services) {
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Program))
                .AddClasses(classes => classes.Where(type => type.Name.Contains("Repository")))
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.All))
                .AsImplementedInterfaces()
                // .WithSingletonLifetime()
                .WithScopedLifetime()
            );
            return services;
        }
    }
}