using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecipeManagementBE.Constant;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Repository;
using RecipeManagementBE.Security;

namespace RecipeManagementBE.Service.Impl {
    public class AccountService : BaseService, IAccountService {

        private readonly IFirebaseService _firebaseService;
        
        private readonly IRoleRepository _roleRepository;

        private readonly IAccountRepository _accountRepository;

        private readonly IAdminRepository _adminRepository;

        private readonly IEmployeeRepository _employeeRepository;

        private readonly IMailService _mailService;

        private readonly ILogService<Account> _logService;

        private readonly ILogger<AccountService> _logger;

        private readonly IMapper _mapper;

        public AccountService(IRoleRepository roleRepository,
            IAccountRepository accountRepository, IMapper mapper,
            ILogger<AccountService> logger, IAdminRepository adminRepository,
            IEmployeeRepository employeeRepository, ILogService<Account> logService,
            IHttpContextAccessor httpContextAccessor, IMailService mailService,
            IFirebaseService firebaseService) : base(httpContextAccessor, accountRepository) {
            _roleRepository = roleRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _logger = logger;
            _adminRepository = adminRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
            _logService = logService;
            _mailService = mailService;
            _firebaseService = firebaseService;
        }

        /// <summary>
        /// Get all role available in system
        /// </summary>
        /// <returns>List of role</returns>
        public List<RoleDTO> GetAllRole() {
            var roleEntities = _roleRepository.QueryHelper()
                .Filter(role => !role.IsDeleted)
                .GetAll().ToList();

            return _mapper.Map<List<RoleDTO>>(roleEntities);
        }

        /// <summary>
        /// Get current account information 
        /// </summary>
        /// <remarks>
        /// 1. Get current account UID in jwt token
        /// 2. Get role of current account in jwt token
        /// 3. Check empty of uid and role if empty -> unauthorized and throw exception
        /// 4. Base role to get entity from repo then return
        /// </remarks>
        /// <returns>Current account information</returns>
        public object GetCurrentAccountInfo() {
            var jwtToken = GetToken();

            var uid = GetCurrentAccountUID();
            var role = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(JwtRegisteredClaimNames.Typ))?.Value ??
                       string.Empty;

            if (string.IsNullOrWhiteSpace(uid)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            if (string.IsNullOrWhiteSpace(role)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account role, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"role"});
            }

            switch (role) {
                case RoleConstants.ADMIN_CODE:
                    var adminEntity = _adminRepository.QueryHelper()
                        .Include(admin => admin.Account.Role)
                        .GetOne(admin => !admin.IsDeleted && admin.UID.Equals(uid));

                    return _mapper.Map<AdminDTO>(adminEntity);
                default:
                    var employeeEntity = _employeeRepository.QueryHelper()
                        .Include(employee => employee.Account.Role)
                        .Include(employee => employee.Brand)
                        .GetOne(employee => !employee.IsDeleted && employee.UID.Equals(uid));

                    return _mapper.Map<EmployeeDTO>(employeeEntity);
            }
        }

        /// <summary>
        /// Update account information
        /// </summary>
        /// <param name="dto">Information to update account</param>
        /// <param name="dto.UID">UID for get account need to update (required)</param>
        /// <param name="dto.Password">Required</param>
        /// <param name="dto.Email">Required for authentication</param>
        /// <param name="dto.FullName">Required for logging</param>
        /// <remarks>
        /// 1. Check uid, pass, email, fullname provided or not -> if not throw exception
        /// 2. Get existed account in repo base on uid and include account role 
        /// 3. Compare provided password with stored password if not equal need to hash provided password and replace stored
        /// 4. Map info from dto to existed entity (account map profile ignore role and roleId)
        /// 5. Update and save to repo
        /// </remarks>
        /// <returns>Updated account information</returns>
        public AccountDTO UpdateAccountInfo(AccountDTO dto) {
            var uid = dto.UID ?? string.Empty;
            var password = dto.Password ?? string.Empty;
            var email = dto.Email ?? string.Empty;
            var fullName = dto.FullName ?? string.Empty;

            if (string.IsNullOrWhiteSpace(uid)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing account uid, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"uid"});
            }
            
            if (string.IsNullOrWhiteSpace(password)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing account password, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"password"});
            }

            if (string.IsNullOrWhiteSpace(email)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing account email, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"email"});
            }

            if (string.IsNullOrWhiteSpace(fullName)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing account fullname, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"fullName"});
            }
            
            var currentAccountUID = GetCurrentAccountUID();
            
            if (string.IsNullOrWhiteSpace(currentAccountUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            if (!uid.Equals(currentAccountUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Try to update not current account, method not allow exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMethodNotAllow(new[] {"uid"});
            }
            
            if (_accountRepository.Exists(account => account.Email.Equals(email) && !account.IsDeleted && !account.UID.Equals(currentAccountUID))) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Account with email in system had already existed while trying update account, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {"email"});
            }
            
            var existedEntity = _accountRepository.QueryHelper()
                .Include(account => account.Role)
                .Include(account => account.Employee.Brand)
                .GetOne(account => account.UID.Equals(uid) && !account.IsDeleted);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed account entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"uid"});
            }
            
            var passwordUpdate = string.Empty;
            if (!string.IsNullOrWhiteSpace(password)) {
                if (!existedEntity.Password.Equals(password)) {
                    if (!BCryptPasswordHasher.VerifyHashedPassword(existedEntity.Password, password)) {
                        passwordUpdate = BCryptPasswordHasher.HashPassword(password);
                    }
                }
            }
            
            _mapper.Map(dto, existedEntity);
            
            if (!string.IsNullOrWhiteSpace(passwordUpdate)) {
                existedEntity.Password = passwordUpdate;
            }
            
            using (var transaction = _accountRepository.BeginTransaction()) {
                existedEntity = _accountRepository.Update(existedEntity);
                _logService.WriteLogUpdateAccount(existedEntity);
                _accountRepository.SaveChanges();
                
                _mailService.SendMail(existedEntity.Email,
                    "StaffMate please inform you that your account has been updated", 
                    "Your account info updated:\n" +
                    "FullName: " + existedEntity.FullName + "\n" +
                    "Password: " + password + "\n" +
                    "Phone: " + existedEntity.Phone);
                
                transaction.Commit();
            }

            return _mapper.Map<AccountDTO>(existedEntity);
        }
        
        /// <summary>
        /// Update account password
        /// </summary>
        /// <param name="dto">Information to update account</param>
        /// <param name="dto.UID">UID for get account need to update (required)</param>
        /// <param name="dto.Password">Required</param>
        /// <param name="dto.Email">Required for authentication</param>
        /// <param name="dto.FullName">Required for logging</param>
        /// <remarks>
        /// 1. Check uid, pass, email, fullname provided or not -> if not throw exception
        /// 2. Get existed account in repo base on uid and email and include account role 
        /// 3. Map info from dto to existed entity (account map profile ignore role and roleId)
        /// 4. Update and save to repo
        /// </remarks>
        /// <returns>Updated account information</returns>
        public AccountDTO UpdateAccountPasswordByUIDAndEmail(AccountDTO dto) {
            var uid = dto.UID ?? string.Empty;
            var password = dto.Password ?? string.Empty;
            var email = dto.Email ?? string.Empty;

            if (string.IsNullOrWhiteSpace(uid)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing account uid, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"uid"});
            }
            
            if (string.IsNullOrWhiteSpace(password)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing account password, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"password"});
            }

            if (string.IsNullOrWhiteSpace(email)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing account email, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"email"});
            }
            
            if (!_firebaseService.AuthenticationFirebase(email, password, uid)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't not authenticate firebase, authorize fail exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowAuthorizeFailException(new[]{"email", "password"});
            }

            var existedEntity = _accountRepository.QueryHelper()
                .Include(account => account.Role)
                .Include(account => account.Employee.Brand)
                .GetOne(account => account.UID.Equals(uid) && !account.IsDeleted && account.Email.Equals(email));

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed account entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"uid"});
            }
            
            if (!string.IsNullOrWhiteSpace(password)) {
                existedEntity.Password = BCryptPasswordHasher.HashPassword(password);
            }
            
            using (var transaction = _accountRepository.BeginTransaction()) {
                existedEntity = _accountRepository.Update(existedEntity);
                _accountRepository.SaveChanges();
                
                _mailService.SendMail(existedEntity.Email,
                    "StaffMate please inform you that your account has been updated", 
                    "Your account info updated:\n" +
                    "FullName: " + existedEntity.FullName + "\n" +
                    "Password: " + password + "\n" +
                    "Phone: " + existedEntity.Phone);
                
                transaction.Commit();
            }

            return _mapper.Map<AccountDTO>(existedEntity);
        }

        public bool CheckPasswordCurrentAccount(string password) {
            var uid = GetCurrentAccountUID();

            if (string.IsNullOrWhiteSpace(uid)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            var currentAccountEntity = _accountRepository.QueryHelper()
                .GetOne(account => account.UID.Equals(uid) && !account.IsDeleted);

            return BCryptPasswordHasher.VerifyHashedPassword(currentAccountEntity.Password, password);
        }
    }
}