using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using JHipsterNet.Core.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecipeManagementBE.Constant;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Repository;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;
using RecipeManagementBE.Security;
using RecipeManagementBE.Util;

namespace RecipeManagementBE.Service.Impl {
    public class AdminService : BaseService, IAdminService {
        private readonly IAdminRepository _adminRepository;

        private readonly IAccountRepository _accountRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly IFirebaseService _firebaseService;
            
        private readonly IMailService _mailService;

        private readonly IAuthenticationService _authenticationService;
        
        private readonly ILogService<Admin> _logService;

        private readonly ILogger<AdminService> _logger;

        private readonly IMapper _mapper;

        private const string ADMIN_PK = "id";

        public AdminService(IAdminRepository adminRepository, IAccountRepository accountRepository,
            IRoleRepository roleRepository, IMapper mapper, ILogService<Admin> logService,
            IHttpContextAccessor httpContextAccessor, ILogger<AdminService> logger, IMailService mailService,
            IFirebaseService firebaseService, IAuthenticationService authenticationService) : base(httpContextAccessor, accountRepository) {
            _adminRepository = adminRepository;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logService = logService;
            _logger = logger;
            _mailService = mailService;
            _firebaseService = firebaseService;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Get all admin available in system
        /// </summary>
        /// <returns>List of admin available</returns>
        public List<AdminDTO> GetAllAdmin() {
            var adminEntities = _adminRepository.QueryHelper()
                .Filter(admin => !admin.IsDeleted)
                .Include(admin => admin.Account)
                .Include(admin => admin.Account.Role)
                .OrderBy(admins => admins.OrderByDescending(admin => admin.Id))
                .GetAll().ToList();

            return _mapper.Map<List<AdminDTO>>(adminEntities);
        }

        /// <summary>
        /// Get admin information by id
        /// </summary>
        /// <param name="id">Id of admin want to get</param>
        /// <remarks>
        /// 1. Check id is provided or not -> if didn't provide throw exception
        /// </remarks>
        /// <returns>Admin information matched with provided id</returns>
        public AdminDTO GetAdminById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing admin id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {ADMIN_PK});
            }

            var existedEntity = _adminRepository.QueryHelper()
                .Include(admin => admin.Account)
                .Include(admin => admin.Account.Role)
                .GetOne(admin => admin.Id == id && !admin.IsDeleted);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed admin entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {ADMIN_PK});
            }

            return _mapper.Map<AdminDTO>(existedEntity);
        }

        /// <summary>
        /// Add new admin to system with provided information
        /// </summary>
        /// <param name="dto">Information to add new admin</param>
        /// <param name="dto.UID">Required primary key of account</param>
        /// <param name="dto.Password">Required</param>
        /// <param name="dto.Email">Required for authentication</param>
        /// <param name="dto.FullName">Required for logging</param>
        /// <remarks>
        /// 1. Check uid is provided or not -> if not throw exception
        /// 2. Check existed account in repo base on uid
        /// 3. Check existed account in repo base on email
        /// 4. Get role admin entity from repo
        /// 5. Hash password
        /// 6. Add to repo and write log
        /// </remarks>
        /// <returns>Admin information added</returns>
        public AdminDTO AddNewAdmin(RegisterDTO dto) {
            var uid = dto.UID;
            var password = dto.Password ?? string.Empty;
            var email = dto.Email ?? string.Empty;
            var fullName = dto.FullName ?? string.Empty;

            if (string.IsNullOrWhiteSpace(uid)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing admin uid, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"uid"});
            }

            if (string.IsNullOrWhiteSpace(password)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing admin password, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"password"});
            }

            if (string.IsNullOrWhiteSpace(email)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing admin email, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"email"});
            }

            if (string.IsNullOrWhiteSpace(fullName)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing admin fullname, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"fullName"});
            }

            if (_accountRepository.Exists(account => account.UID.Equals(uid) && !account.IsDeleted)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Admin with uid had already existed while trying create new admin, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {"uid"});
            }
            
            if (_accountRepository.Exists(account => account.Email.Equals(email) && !account.IsDeleted)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Admin with email in system had already existed while trying create new admin, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {"email"});
            }

            var roleEntity = _roleRepository.QueryHelper()
                .GetOne(role => role.Id == RoleConstants.ADMIN_ID && !role.IsDeleted);

            dto.Password = BCryptPasswordHasher.HashPassword(password);

            var newEntity = new Admin {
                Account = _mapper.Map<Account>(dto),
                UID = dto.UID
            };
            newEntity.Account.Role = roleEntity;

            using (var transaction = _accountRepository.BeginTransaction()) {
                newEntity = _adminRepository.Add(newEntity);
                _logService.WriteLogCreateAccount(newEntity.Account);
                _adminRepository.SaveChanges();
                
                _mailService.SendMail(newEntity.Account.Email,
                    "Welcome to StaffMate, you have an account created",
                    "Your account info:\n" +
                    "FullName: " + newEntity.Account.FullName + "\n" +
                    "Password: " + password + "\n" +
                    "Phone: " + newEntity.Account?.Phone + "\n" +
                    "Role: " + newEntity.Account.Role.Name);
                
                transaction.Commit();
            }

            return _mapper.Map<AdminDTO>(newEntity);
        }

        /// <summary>
        /// Delete admin by id (update isDeleted flag)
        /// </summary>
        /// <remarks>
        /// 1. Check id is provided or not -> if not throw exception
        /// 2. Get existed entity by id
        /// 3. Get current account UID in jwt token
        /// 4. Check existed entity UID equal with current account UID -> if equal throw method not allow exception
        /// 5. Update flag, save to repo, write log
        /// </remarks>
        /// <param name="id">Id of admin want to delete</param>
        /// <returns>Delete status</returns>
        public bool DeleteAdminById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing admin id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {ADMIN_PK});
            }

            var existedEntity = _adminRepository.QueryHelper()
                .Include(admin => admin.Account.Role)
                .GetOne(admin => admin.Id == id && !admin.IsDeleted);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed admin entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {ADMIN_PK});
            }

            var currentUserUID = GetCurrentAccountUID();

            if (string.IsNullOrWhiteSpace(currentUserUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            // Prevent delete current user
            if (currentUserUID.Equals(existedEntity?.UID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Try to delete current account,method not allow exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMethodNotAllow(new[] {"uid"});
            }

            existedEntity.IsDeleted = true;
            existedEntity.Account.IsDeleted = true;

            using (var transaction = _adminRepository.BeginTransaction()) {
                _adminRepository.Update(existedEntity);
                _logService.WriteLogDelete(existedEntity);

                _authenticationService.RevokeToken(existedEntity.UID);
                _firebaseService.DeleteAccountFirebase(existedEntity.UID);
                
                _adminRepository.SaveChanges();
                transaction.Commit();
            }

            return true;
        }

        /// <summary>
        /// Get page of admin
        /// </summary>
        /// <param name="pageableModel"></param>
        /// <param name="pageableModel.FullName">FullName to search admin</param>
        /// <param name="pageableModel.Email">Email to search admin</param>
        /// <param name="pageableModel.SortField">Field to sort page</param>
        /// <param name="pageableModel.SortDirection">if direction < 0 -> desc, >= 0 -> asc</param>
        /// <param name="pageableModel.PageNumber">Page number need to get</param>
        /// <param name="pageableModel.PageSize">Size of 1 page</param>
        /// <remarks>
        /// 1. FullName and Email is ignore case and contain in entity (like)
        /// 2. Sort direction default is asc
        /// 3. Sort field default is primary key
        /// </remarks>
        /// <returns>Page content list admin</returns>
        public PageResponse<AdminDTO> GetPageAdmin(PageableModel<SearchAccountDTO> pageableModel) {
            var fullName = pageableModel.SearchModel?.FullName ?? "";
            var email = pageableModel.SearchModel?.Email ?? "";

            var sortField = pageableModel.SortField ?? ADMIN_PK;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);

            var adminEntities = _adminRepository.QueryHelper()
                .Filter(admin => !admin.IsDeleted &&
                                 admin.Account.FullName.ToLower().Contains(fullName.ToLower()) &&
                                 admin.Account.Email.ToLower().Contains(email.ToLower()))
                .Include(admin => admin.Account)
                .Include(admin => admin.Account.Role)
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));

            return _mapper.Map<IPage<Admin>, PageResponse<AdminDTO>>(adminEntities).GetTotalPage();
        }
    }
}