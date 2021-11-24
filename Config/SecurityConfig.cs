using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeManagementBE.Constant;
using RecipeManagementBE.Security;
using RecipeManagementBE.Security.Authorization;

namespace RecipeManagementBE.Config {
    public static class SecurityConfig {
        public static IServiceCollection AddSecurityConfiguration(this IServiceCollection services) {
            var opt = services.BuildServiceProvider().GetRequiredService<IOptions<SecuritySettings>>();
            var securitySettings = opt.Value;
            byte[] keyBytes;
            var secret = securitySettings.Authentication.Jwt.Secret;

            if (!string.IsNullOrWhiteSpace(secret)) {
                keyBytes = Encoding.ASCII.GetBytes(secret);
            }
            else {
                keyBytes = Convert.FromBase64String(securitySettings.Authentication.Jwt.Base64Secret);
            }

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddCors(options => {
                options.AddDefaultPolicy(CorsPolicyBuilder(securitySettings.Cors));
            });
            
            var tokenValidationParameters = new TokenValidationParameters {
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                RequireExpirationTime = false,
                ClockSkew = TimeSpan.Zero,
            };
            services.AddSingleton(tokenValidationParameters);
            
            services
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg => {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = tokenValidationParameters;
                });

            // services.AddSingleton<ITokenProvider, TokenProvider>();
            services.AddScoped<ITokenProvider, TokenProvider>();
            
            services.AddAuthorization(options => {
                options.AddPolicy(RoleConstants.ADMIN_CODE,builder => 
                    builder.Requirements.Add(new RoleRequirement(new List<string>(new[]{RoleConstants.ADMIN_CODE}))));
                
                options.AddPolicy(RoleConstants.ADMIN_MANAGER_CODE,builder => 
                    builder.Requirements.Add(new RoleRequirement(new List<string>(new[]{RoleConstants.ADMIN_CODE, RoleConstants.MANAGER_CODE}))));
                
                options.AddPolicy(RoleConstants.MANAGER_CODE,builder => 
                    builder.Requirements.Add(new RoleRequirement(new List<string>(new[]{RoleConstants.MANAGER_CODE}))));
                
                options.AddPolicy(RoleConstants.STAFF_CODE,builder => 
                    builder.Requirements.Add(new RoleRequirement(new List<string>(new[]{RoleConstants.STAFF_CODE}))));
                
                options.AddPolicy(RoleConstants.EMPLOYEE_CODE,builder => 
                    builder.Requirements.Add(new RoleRequirement(new List<string>(new[]{RoleConstants.MANAGER_CODE, RoleConstants.STAFF_CODE}))));
                
                options.AddPolicy(RoleConstants.ADMIN_SYSTEM,builder => 
                    builder.Requirements.Add(new AdminSystemRequirement(Constants.ADMIN_SYSTEM_UID)));
            });
            // services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, AdminSystemAuthorizationHandler>();
            return services;
        }

        public static IApplicationBuilder UseApplicationSecurity(this IApplicationBuilder app,
            SecuritySettings securitySettings) {
            app.UseCors(CorsPolicyBuilder(securitySettings.Cors));
            app.UseAuthentication();
            return app;
        }

        private static Action<CorsPolicyBuilder> CorsPolicyBuilder(Cors config) {
            return builder => {
                if (!config.AllowedOrigins.Equals("*")) {
                    if (config.AllowCredentials) {
                        builder.AllowCredentials();
                    }
                    else {
                        builder.DisallowCredentials();
                    }
                }

                builder.WithOrigins(config.AllowedOrigins)
                    .WithMethods(config.AllowedMethods)
                    .WithHeaders(config.AllowedHeaders)
                    .WithExposedHeaders(config.ExposedHeaders)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(config.MaxAge));
            };
        }
    }
}