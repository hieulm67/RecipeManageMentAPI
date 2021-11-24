using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace RecipeManagementBE.Security
{
    public interface ITokenProvider
    {
        SecurityToken CreateToken(IPrincipal principal, bool rememberMe);
    }

    public class TokenProvider : ITokenProvider
    {
        private readonly SecuritySettings _securitySettings;

        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        private SigningCredentials _key;

        private long _tokenValidityInSeconds;

        private long _tokenValidityInSecondsForRememberMe;


        public TokenProvider(IOptions<SecuritySettings> securitySettings)
        {
            _securitySettings = securitySettings.Value;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            Init();
        }

        public SecurityToken CreateToken(IPrincipal principal, bool rememberMe)
        {
            var subject = CreateSubject(principal);
            var validity =
                DateTime.UtcNow.AddSeconds(rememberMe
                    ? _tokenValidityInSecondsForRememberMe
                    : _tokenValidityInSeconds);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = validity,
                SigningCredentials = _key
            };

            var token = _jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            return token;
        }

        private void Init()
        {
            byte[] keyBytes;
            var secret = _securitySettings.Authentication.Jwt.Secret;

            if (!string.IsNullOrWhiteSpace(secret))
            {
                keyBytes = Encoding.ASCII.GetBytes(secret);
            }
            else
            {
                keyBytes = Convert.FromBase64String(_securitySettings.Authentication.Jwt.Base64Secret);
            }

            _key = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature);
            
            _tokenValidityInSeconds = _securitySettings.Authentication.Jwt.TokenValidityInSeconds;
            
            _tokenValidityInSecondsForRememberMe = _securitySettings.Authentication.Jwt.TokenValidityInSecondsForRememberMe;
        }

        private static ClaimsIdentity CreateSubject(IPrincipal principal)
        {
            var username = GetNameIdentifier(principal);
            var role = GetRoles(principal);
            var email = GetEmail(principal);
            var brandId = GetBrandIdentifier(principal);
            var brandName = GetBrandName(principal);
            var identity = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.NameId, username.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email.Value),
                new Claim(JwtRegisteredClaimNames.Typ, role.Value),
            });
            if (brandId != null) {
                identity.AddClaim(new Claim("brandId", brandId.Value));
            }
            
            if (brandName != null) {
                identity.AddClaim(new Claim("brandName", brandName.Value));
            }
            
            return identity;
        }

        private static Claim GetRoles(IPrincipal principal) {
            return principal is ClaimsPrincipal user
                ? user.FindFirst(claim => claim.Type.Equals(ClaimTypes.Role)) : null;
        }
        
        private static Claim GetEmail(IPrincipal principal) {
            return principal is ClaimsPrincipal user
                ? user.FindFirst(claim => claim.Type.Equals(ClaimTypes.Email)) : null;
        }
        
        private static Claim GetNameIdentifier(IPrincipal principal) {
            return principal is ClaimsPrincipal user
                ? user.FindFirst(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)) : null;
        }
        
        private static Claim GetBrandIdentifier(IPrincipal principal) {
            return principal is ClaimsPrincipal user
                ? user.FindFirst(claim => claim.Type.Equals("brandId")) : null;
        }
        
        private static Claim GetBrandName(IPrincipal principal) {
            return principal is ClaimsPrincipal user
                ? user.FindFirst(claim => claim.Type.Equals("brandName")) : null;
        }
    }
}
