using System;
using System.Security.Authentication;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RecipeManagementBE.Response.Exception;

namespace RecipeManagementBE.Config {
    public static class ProblemDetailConfig {
        
        public static IServiceCollection AddProblemDetailConfiguration(this IServiceCollection services) {
            services.AddProblemDetails(setup => {
                setup.Map<BusinessException>(exception => new ProblemDetails {
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { ["errors"] = exception.ExceptionParams},
                    Title = exception.ErrorCode
                });
                
                setup.Map<SystemException>(exception => new ProblemDetails {
                    Title = exception.Message,
                    Status = StatusCodes.Status400BadRequest
                });

                setup.Map<ApplicationException>(exception => new ProblemDetails {
                    Title = exception.Message,
                    Status = StatusCodes.Status400BadRequest
                });
                
                setup.Map<Exception>(exception => new ProblemDetails {
                    Title = exception.Message,
                    Status = StatusCodes.Status400BadRequest
                });
                
                setup.MapToStatusCode<AuthenticationException>(StatusCodes.Status401Unauthorized);
                setup.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

                setup.IncludeExceptionDetails = (ctx, ex) => false;
            });

            return services;
        }
    }
}