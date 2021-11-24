using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RecipeManagementBE.Security.Authorization {
    internal class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement) {
            if (context.User.HasClaim(c => c.Type.Equals(JwtRegisteredClaimNames.NameId)))
                if (requirement.Role.Contains(context.User.FindFirst(c => c.Type.Equals(JwtRegisteredClaimNames.Typ))?.Value))
                    context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    internal class RoleRequirement : IAuthorizationRequirement {
        public RoleRequirement(List<string> role)
        {
            Role = role;
        }

        public readonly List<string> Role;
    }
}