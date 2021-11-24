using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Config;
using RecipeManagementBE.Repository;
using RecipeManagementBE.Security;

namespace RecipeManagementBE {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<StaffMateContext>(option => {
                        option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                        // option.UseSqlServer(Configuration.GetConnectionString("AzureConnection"));
                        option.EnableSensitiveDataLogging();
                    },
                    // contextLifetime: ServiceLifetime.Singleton,
                    // optionsLifetime: ServiceLifetime.Singleton)
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Scoped)
                // .AddSingleton<DbContext>(provider => provider.GetService<StaffMateContext>())
                .AddScoped<DbContext>(provider => provider.GetService<StaffMateContext>())
                .AddAppSettingsConfiguration(Configuration)
                .AddSwaggerConfiguration()
                .AddServicesConfiguration()
                .AddRepositoryConfiguration()
                // .AddSingleton<IUnitOfWork, UnitOfWork>()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddAutoMapper(typeof(Startup))
                .AddProblemDetailConfiguration()
                .AddSecurityConfiguration()
                .AddRouting(options => options.LowercaseUrls = true)
                .AddHttpContextAccessor();

            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<SecuritySettings> securitySettingsOptions) {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecipeManagementBE v1"));

            app.UseProblemDetails();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseApplicationSecurity(securitySettingsOptions.Value);
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}