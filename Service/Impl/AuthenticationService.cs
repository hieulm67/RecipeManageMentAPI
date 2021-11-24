using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RecipeManagementBE.Constant;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Repository;
using RecipeManagementBE.Request.Authentication;
using RecipeManagementBE.Response;
using RecipeManagementBE.Response.Exception;
using RecipeManagementBE.Security;

namespace RecipeManagementBE.Service.Impl {
    public class AuthenticationService : IAuthenticationService {

        private readonly IFirebaseService _firebaseService;
        
        private readonly IAccountRepository _accountRepository;
        
        private readonly ITokenProvider _tokenProvider;

        private readonly ILogger<AuthenticationService> _logger;
        
        private readonly IEmployeeRepository _employeeRepository;

        private readonly IRefreshTokenRepository _refreshTokenRepository;

        private readonly TokenValidationParameters _tokenValidationParameters;

        private const int _refreshTokenExpireDuration = 7;
        
        private const int _durationAllowRefreshBeforeExpire = 2;
        
        public AuthenticationService(IAccountRepository accountRepository, ITokenProvider tokenProvider, IEmployeeRepository employeeRepository,
            ILogger<AuthenticationService> logger, IRefreshTokenRepository refreshTokenRepository, TokenValidationParameters tokenValidationParameters,
            IFirebaseService firebaseService) {
            _accountRepository = accountRepository;
            _tokenProvider = tokenProvider;
            _employeeRepository = employeeRepository;
            _logger = logger;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenValidationParameters = tokenValidationParameters.Clone();
            _tokenValidationParameters.ValidateLifetime = false;
            _firebaseService = firebaseService;
        }

        /// <summary>
        /// Authenticate account login
        /// </summary>
        /// <param name="dto">Login info</param>
        /// <param name="isRefresh">Flag for refresh method</param>
        /// <remarks>
        /// 1. Get account with uid provided
        /// 2. Check password
        /// 3. Check email
        /// 4. Check role to include brandId and brandName to token
        /// 5. Create principle
        /// 6. Create security token
        /// 7. New refresh token and save to repo
        /// </remarks>
        /// <returns>Authentication result</returns>
        /// <exception cref="BusinessException"></exception>
        public AuthenticationResult Authenticate(LoginDTO dto, bool isRefresh) {
            var uid = dto.UID ?? string.Empty;
            var email = dto.Email ?? string.Empty;
            var password = dto.Password ?? string.Empty;
            
            var userLogin = _accountRepository
                .QueryHelper()
                .Include(account => account.Role)
                .GetOne(account => account.UID.Equals(uid) && !account.IsDeleted && account.Email.Equals(email));
            
            if (userLogin == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed account entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_EXIST, new ExceptionParams{Params = new[]{"uid", "email"}});
            }

            if (!BCryptPasswordHasher.VerifyHashedPassword(userLogin.Password, password) && !isRefresh) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Wrong password, authorize fail exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.AUTHORIZE_FAIL, new ExceptionParams{Params = new[]{"password"}});
            }

            if (!_firebaseService.AuthenticationFirebase(userLogin.Email, password, userLogin.UID)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't not authenticate firebase, authorize fail exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.AUTHORIZE_FAIL, new ExceptionParams{Params = new[]{"email", "password"}});
            }

            Brand brand = null;
            if (userLogin.RoleId != RoleConstants.ADMIN_ID) {
                brand = _employeeRepository.QueryHelper()
                    .Include(employee => employee.Brand)
                    .GetOne(employee => !employee.IsDeleted && employee.UID.Equals(userLogin.UID))?.Brand;

                if (brand == null) {
                    _logger.LogError(
                        "[{Time}] [{ApplicationName}]: Can't found existed brand entity match filter, entity not found exception throw",
                        DateTime.Now ,Constants.APPLICATION_NAME);
                    throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_EXIST, new ExceptionParams{Params = new[]{"employee", "uid"}});
                }
            }
            
            var user = CreatePrincipal(userLogin, brand);
            var token = _tokenProvider.CreateToken(user, dto.RememberMe);

            var refreshToken = new RefreshToken {
                Token = RandomString(35) + Guid.NewGuid(),
                IsUsed = false,
                IsRevoked = false,
                UserUID = userLogin.UID,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(_refreshTokenExpireDuration),
                JwtId = token.Id
            };

            _refreshTokenRepository.Add(refreshToken);
            _refreshTokenRepository.SaveChanges();

            return new AuthenticationResult {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresTo = token.ValidTo.ToLocalTime(),
                RefreshToken = refreshToken.Token
            };
        }
        
        public AuthenticationResult RefreshToken(TokenRequest dto) {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var tokenInVerification = jwtSecurityTokenHandler.ValidateToken(dto.Token, _tokenValidationParameters,
                        out var validatedToken);
            
            if (validatedToken is JwtSecurityToken jwtSecurityToken) {
                if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                    _logger.LogError(
                        "[{Time}] [{ApplicationName}]: Invalid token alg",
                        DateTime.Now ,Constants.APPLICATION_NAME);
                    throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_CORRECT_FORMAT, new ExceptionParams{Params = new[]{"token"}});
                }
            }
            
            var utcExpiryDate =
                long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type.Equals(JwtRegisteredClaimNames.Exp))?.Value ?? "0");

            if (utcExpiryDate == 0) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Invalid token, expiry date not found",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_CORRECT_FORMAT, new ExceptionParams{Params = new[]{"token"}});
            }

            var expiryDateTime = UnixTimestampToDateTime(utcExpiryDate);

            // Allow refresh token before expire 
            if (expiryDateTime > DateTime.UtcNow.AddHours(_durationAllowRefreshBeforeExpire)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Token has not yet expired",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_CORRECT_FORMAT, new ExceptionParams{Params = new[]{"expDate"}});
            }

            var storedToken = _refreshTokenRepository.QueryHelper()
                .GetOne(token => token.Token.Equals(dto.RefreshToken));

            if (storedToken == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Refresh token not found in repo",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_EXIST, new ExceptionParams{Params = new[]{"refreshToken"}});
            }

            if (storedToken.IsUsed) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Refresh token is already used",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_EXIST, new ExceptionParams{Params = new[]{"refreshToken"}});
            }
            
            if (storedToken.IsRevoked) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Refresh token is already revoked",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_EXIST, new ExceptionParams{Params = new[]{"refreshToken"}});
            }

            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type.Equals(JwtRegisteredClaimNames.Jti))?.Value;

            if (storedToken.JwtId != jti) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Jti not matched",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_CORRECT_FORMAT, new ExceptionParams{Params = new[]{"expToken"}});
            }

            storedToken.IsUsed = true;
            _refreshTokenRepository.Update(storedToken);
            _refreshTokenRepository.SaveChanges();

            var accountEntity = _accountRepository.QueryHelper()
                .GetOne(account => account.UID.Equals(storedToken.UserUID) && !account.IsDeleted);

            if (accountEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Account entity not found",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_EXIST, new ExceptionParams{Params = new[]{"refreshToken"}});
            }

            return Authenticate(new LoginDTO {
                UID = accountEntity.UID,
                Email = accountEntity.Email,
                Password = accountEntity.Password,
                RememberMe = false
            }, true);
        }

        private IPrincipal CreatePrincipal(Account account, Brand brand)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, account.UID),
                new Claim(ClaimTypes.Email, account.Email)
            };
            var role = account.Role;
            claims.Add(new Claim(ClaimTypes.Role, role.Code));

            if (brand != null) {
                claims.Add(new Claim("brandId", brand.Id.ToString()));
                claims.Add(new Claim("brandName", brand.Name));
            }
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }

        // Generate refresh token
        private string RandomString(int length) {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(x.Length)]).ToArray());
        }

        // Parse unixTimeStamp get from exp access token to datetime (utc)
        private DateTime UnixTimestampToDateTime(long timeStamp) {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(timeStamp);
            return dateTime;
        }

        public bool RevokeToken(string uid) {
            if (string.IsNullOrWhiteSpace(uid)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing admin uid, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                throw new BusinessException(ExceptionCodeMapping.ITEM_FIELD_REQUIRED_MISSING, new ExceptionParams{Params = new[]{"uid"}});
            }
            
            var storedTokens = _refreshTokenRepository.QueryHelper()
                .Filter(token => token.UserUID.Equals(uid))
                .GetAll().ToList();
            
            storedTokens.ForEach(token => token.IsRevoked = true);

            _refreshTokenRepository.UpdateRange(storedTokens.ToArray());

            return true;
        }
    }
}