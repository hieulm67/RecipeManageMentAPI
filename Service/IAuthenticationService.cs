using RecipeManagementBE.Request.Authentication;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface IAuthenticationService {

        AuthenticationResult Authenticate(LoginDTO dto, bool isRefresh);

        AuthenticationResult RefreshToken(TokenRequest dto);

        bool RevokeToken(string uid);
    }
}