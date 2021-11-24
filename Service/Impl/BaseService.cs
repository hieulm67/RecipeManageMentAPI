using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using RecipeManagementBE.Constant;
using RecipeManagementBE.Repository;
using RecipeManagementBE.Response.Exception;

namespace RecipeManagementBE.Service.Impl {
    public class BaseService {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAccountRepository _accountRepository;

        public BaseService(IHttpContextAccessor httpContextAccessor, IAccountRepository accountRepository) {
            _httpContextAccessor = httpContextAccessor;
            _accountRepository = accountRepository;
        }

        public static void ThrowMissingField(string[] missingFields) {
            throw new BusinessException(ExceptionCodeMapping.ITEM_FIELD_REQUIRED_MISSING,
                new ExceptionParams {Params = missingFields});
        }

        public static void ThrowEntityNotFound(string[] param) {
            throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_EXIST,
                new ExceptionParams {Params = param});
        }

        public static void ThrowEntityExisted(string[] param) {
            throw new BusinessException(ExceptionCodeMapping.ITEM_ALREADY_EXISTED,
                new ExceptionParams {Params = param});
        }

        public static void ThrowUnauthorizedException(string[] param) {
            throw new BusinessException(ExceptionCodeMapping.UNAUTHORIZED,
                new ExceptionParams {Params = param});
        }
        
        public static void ThrowAuthorizeFailException(string[] param) {
            throw new BusinessException(ExceptionCodeMapping.AUTHORIZE_FAIL,
                new ExceptionParams {Params = param});
        }

        public static void ThrowMethodNotAllow(string[] param) {
            throw new BusinessException(ExceptionCodeMapping.METHOD_NOT_ALLOW,
                new ExceptionParams {Params = param});
        }
        
        public static void ThrowItemInUse(string[] param) {
            throw new BusinessException(ExceptionCodeMapping.ITEM_IN_USE,
                new ExceptionParams {Params = param});
        }
        
        public static void ThrowItemNotCorrectFormat(string[] param) {
            throw new BusinessException(ExceptionCodeMapping.ITEM_NOT_CORRECT_FORMAT,
                new ExceptionParams {Params = param});
        }

        public JwtSecurityToken GetToken() {
            var token = _httpContextAccessor.HttpContext?.GetTokenAsync("Bearer", "access_token").Result;
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            return jwtSecurityTokenHandler.ReadJwtToken(token);
        }

        public void CheckValidAccount(long brandId) {
            var brandIdInToken = GetToken().Claims.FirstOrDefault(claim => claim.Type.Equals("brandId"))?.Value ??
                                 string.Empty;
            
            if (string.IsNullOrWhiteSpace(brandIdInToken)) {
                if (!IsAdmin()) {
                    ThrowUnauthorizedException(new[] {"brandId"});
                }
            }
            else {
                if (Convert.ToInt64(brandIdInToken) != brandId) {
                    ThrowMethodNotAllow(new []{"brandId"});
                }
            }
        }

        public bool IsAdmin() {
            var role = GetToken().Claims.FirstOrDefault(claim => claim.Type.Equals(JwtRegisteredClaimNames.Typ))?.Value ??
                       string.Empty;

            if (string.IsNullOrWhiteSpace(role)) {
                ThrowUnauthorizedException(new[] {"role"});
            }

            return role.Equals(RoleConstants.ADMIN_CODE);
        }
        
        public bool IsManager() {
            var role = GetToken().Claims.FirstOrDefault(claim => claim.Type.Equals(JwtRegisteredClaimNames.Typ))?.Value ??
                       string.Empty;

            if (string.IsNullOrWhiteSpace(role)) {
                ThrowUnauthorizedException(new[] {"role"});
            }

            return role.Equals(RoleConstants.MANAGER_CODE);
        }
        
        public bool IsStaff() {
            var role = GetToken().Claims.FirstOrDefault(claim => claim.Type.Equals(JwtRegisteredClaimNames.Typ))?.Value ??
                       string.Empty;

            if (string.IsNullOrWhiteSpace(role)) {
                ThrowUnauthorizedException(new[] {"role"});
            }

            return role.Equals(RoleConstants.STAFF_CODE);
        }
        
        public long GetCurrentAccountBrandId() {
            GetCurrentAccountUID();

            var brandIdInToken = GetToken().Claims.FirstOrDefault(claim => claim.Type.Equals("brandId"))?.Value ??
                                 string.Empty;
            var role =
                GetToken().Claims.FirstOrDefault(claim => claim.Type.Equals(JwtRegisteredClaimNames.Typ))?.Value ??
                string.Empty;

            if (string.IsNullOrWhiteSpace(brandIdInToken)) {
                if (!role.Equals(RoleConstants.ADMIN_CODE)) {
                    ThrowUnauthorizedException(new[] {"brandId"});
                }
            }
            else {
                return Convert.ToInt64(brandIdInToken);
            }

            return 0;
        }

        public string GetCurrentAccountUID() {
            var uid = GetToken().Claims.FirstOrDefault(claim => claim.Type.Equals(JwtRegisteredClaimNames.NameId))?.Value ??
                   string.Empty;
            
            if (string.IsNullOrWhiteSpace(uid)) {
                ThrowUnauthorizedException(new[] {"uid"});
            }
            
            if (!_accountRepository.Exists(account => !account.IsDeleted && account.UID.Equals(uid))) {
                ThrowUnauthorizedException(new[] {"uid"});
            }

            return uid;
        }
    }
}