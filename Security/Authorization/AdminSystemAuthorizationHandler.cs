using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RecipeManagementBE.Security.Authorization {
    internal class AdminSystemAuthorizationHandler : AuthorizationHandler<AdminSystemRequirement> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminSystemRequirement requirement) {
            if (context.User.HasClaim(c => c.Type.Equals(JwtRegisteredClaimNames.NameId)))
                if (requirement.UID.Equals(context.User.FindFirst(c => c.Type.Equals(JwtRegisteredClaimNames.NameId))?.Value))
                    context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    internal class AdminSystemRequirement : IAuthorizationRequirement {
        public AdminSystemRequirement(string uid)
        {
            UID = uid;
        }

        public readonly string UID;
    }
}