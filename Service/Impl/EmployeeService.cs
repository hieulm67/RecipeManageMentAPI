using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class EmployeeService : BaseService, IEmployeeService {
        private readonly IEmployeeRepository _employeeRepository;

        private readonly IAccountRepository _accountRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly IBrandRepository _brandRepository;

        private readonly IMailService _mailService;

        private readonly IAuthenticationService _authenticationService;

        private readonly IQAService _qaService;

        private readonly IDishService _dishService;

        private readonly IFirebaseService _firebaseService;

        private readonly ILogService<Employee> _logService;

        private readonly ILogger<EmployeeService> _logger;

        private readonly IMapper _mapper;

        private const string EMPLOYEE_PK = "id";

        public EmployeeService(IEmployeeRepository employeeRepository, IAccountRepository accountRepository,
            IRoleRepository roleRepository, IBrandRepository brandRepository,
            IMapper mapper, ILogService<Employee> logService,
            IHttpContextAccessor httpContextAccessor, ILogger<EmployeeService> logger,
            IMailService mailService, IQAService qaService, IDishService dishService,
            IFirebaseService firebaseService, IAuthenticationService authenticationService) : base(httpContextAccessor, accountRepository) {
            _employeeRepository = employeeRepository;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _brandRepository = brandRepository;
            _mapper = mapper;
            _logService = logService;
            _logger = logger;
            _mailService = mailService;
            _qaService = qaService;
            _dishService = dishService;
            _firebaseService = firebaseService;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Get all employee available in system
        /// </summary>
        /// <remarks>
        /// 1. Get current account brandId, if brandId == 0 -> current account is admin -> ignore brand filter or else add brand filter
        /// 2. Add brand filter to prevent employee of this brand can view employee of another brand
        /// </remarks>
        /// <returns>List of employee</returns>
        public List<EmployeeDTO> GetAllEmployee() {
            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Employee, bool>> filter = employee => !employee.IsDeleted;

            if (brandId != 0) {
                Expression<Func<Employee, bool>> filterBrand = employee => employee.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var employeeEntities = _employeeRepository.QueryHelper()
                .Filter(filter)
                .Include(employee => employee.Brand)
                .Include(employee => employee.Account)
                .Include(employee => employee.Account.Role)
                .OrderBy(employees => employees.OrderByDescending(employee => employee.Id))
                .GetAll().ToList();

            return _mapper.Map<List<EmployeeDTO>>(employeeEntities);
        }

        /// <summary>
        /// Get employee information by id
        /// </summary>
        /// <param name="id">Id of employee need to get</param>
        /// <remarks>
        /// 1. Get current account brandId, if brandId == 0 -> current account is admin -> ignore brand filter or else add brand filter
        /// 2. Add brand filter to prevent employee of this brand can view employee of another brand
        /// 3. If employee of this brand search employee of another brand -> throw not found exception
        /// </remarks>
        /// <returns>Employee information matched with id</returns>
        public EmployeeDTO GetEmployeeById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing employee id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {EMPLOYEE_PK});
            }

            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Employee, bool>> filter = employee => employee.Id == id && !employee.IsDeleted;

            if (brandId != 0) {
                Expression<Func<Employee, bool>> filterBrand = employee => employee.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var existedEntity = _employeeRepository.QueryHelper()
                .Include(employee => employee.Brand)
                .Include(employee => employee.Account)
                .Include(employee => employee.Account.Role)
                .GetOne(filter);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed employee entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {EMPLOYEE_PK});
            }

            return _mapper.Map<EmployeeDTO>(existedEntity);
        }

        /// <summary>
        /// Add new employee to system
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="dto.BrandId">Brand Id employee will be added to (admin)</param>
        /// <param name="dto.Email">Required</param>
        /// <param name="dto.Password">Required</param>
        /// <param name="dto.FullName">Required for logging</param>
        /// <remarks>
        /// 1. Check uid, email, password is empty or not -> if empty throw missing field
        /// 2. Get current account brandId, if brandId == 0 -> current account is admin -> ignore brand filter or else add brand filter
        /// 3. Replace brandId with current account brandId if != 0
        /// 4. If current account brandId == 0 and brandId == 0 -> throw missing brandId
        /// 5. Check existed account with uid
        /// 6. Check duplicate employee email in 1 brand
        /// 7. Get brand entity by brandId from repo and hash password
        /// 8. Map info provided to new employee entity
        /// 9. Get role entity base on isManager param
        /// 10. Add new employee to repo and save change 
        /// </remarks>
        /// <returns></returns>
        public EmployeeDTO AddNewEmployee(RegisterDTO dto) {
            var uid = dto.UID ?? string.Empty;
            var brandId = dto.BrandId;
            var email = dto.Email ?? string.Empty;
            var password = dto.Password ?? string.Empty;
            var fullName = dto.FullName ?? string.Empty;

            if (string.IsNullOrWhiteSpace(uid)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing employee uid, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"uid"});
            }

            if (string.IsNullOrWhiteSpace(email)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing employee email, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"email"});
            }

            if (string.IsNullOrWhiteSpace(password)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing employee password, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"password"});
            }

            if (string.IsNullOrWhiteSpace(fullName)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing employee fullname, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"fullName"});
            }

            var currentAccountBrandId = GetCurrentAccountBrandId();
            brandId = currentAccountBrandId != 0 ? currentAccountBrandId : brandId;

            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing employee brandId, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"brandId"});
            }

            if (_accountRepository.Exists(account => account.UID.Equals(uid) && !account.IsDeleted)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Employee with uid had already existed while trying create new employee, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {"uid"});
            }
            
            if (_accountRepository.Exists(account => account.Email.Equals(email) && !account.IsDeleted)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Employee with email had already existed while trying create new employee, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {"email"});
            }

            var brandEntity = GetBrandEntityById(brandId);

            dto.Password = BCryptPasswordHasher.HashPassword(password);

            if (IsManager()) {
                dto.IsManager = false;
            }

            var newEntity = new Employee {
                Account = _mapper.Map<Account>(dto),
                IsManager = dto.IsManager,
                Brand = brandEntity
            };
            GetRoleEmployee(dto.IsManager, newEntity);

            using (var transaction = _accountRepository.BeginTransaction()) {
                newEntity = _employeeRepository.Add(newEntity);
                _logService.WriteLogCreateAccount(newEntity.Account);
                _employeeRepository.SaveChanges();

                _mailService.SendMail(newEntity.Account.Email,
                    "Welcome to StaffMate, you have an account created",
                    "Your account info:\n" +
                    "FullName: " + newEntity.Account.FullName + "\n" +
                    "Password: " + password + "\n" +
                    "Phone: " + newEntity.Account?.Phone + "\n" +
                    "Role: " + newEntity.Account.Role.Name + "\n" +
                    "Brand: " + newEntity.Brand.Name);
                
                transaction.Commit();
            }

            return _mapper.Map<EmployeeDTO>(newEntity);
        }

        /// <summary>
        /// Update employee role
        /// </summary>
        /// <param name="id">Employee id need to update</param>
        /// <param name="isManager">Flag to know employee role will be manager or staff</param>
        /// <returns>Updated employee info</returns>
        public EmployeeDTO UpdateEmployeeRoleById(long id, bool isManager) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing employee id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {EMPLOYEE_PK});
            }

            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Employee, bool>> filter = employee => employee.Id == id && !employee.IsDeleted;

            if (brandId != 0) {
                Expression<Func<Employee, bool>> filterBrand = employee => employee.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var existedEntity = _employeeRepository.QueryHelper()
                .Include(employee => employee.Account.Role)
                .Include(employee => employee.Brand)
                .GetOne(filter);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed employee entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {EMPLOYEE_PK});
            }

            GetRoleEmployee(isManager, existedEntity);
            existedEntity.IsManager = isManager;

            existedEntity = _employeeRepository.Update(existedEntity);
            _logService.WriteLogUpdateAccount(existedEntity.Account);
            
            _mailService.SendMail(existedEntity.Account.Email,
                "StaffMate please inform you that your account role has been updated", 
                "Your account info updated>\n" +
                "FullName: " + existedEntity.Account.FullName + "\n" +
                "Password: " + existedEntity.Account.Password + "\n" +
                "Phone> " + existedEntity.Account?.Phone + "\n" +
                "Role: " + existedEntity.Account.Role.Name + "\n" + 
                "Brand: " + existedEntity.Brand.Name);
            
            _employeeRepository.SaveChanges();

            return _mapper.Map<EmployeeDTO>(existedEntity);
        }

        /// <summary>
        /// Delete employee by id (update isDeleted flag)
        /// </summary>
        /// <param name="id">Id of employee need to delete</param>
        /// <remarks>
        /// 1. Check id
        /// 2. Get current account brandId, if brandId == 0 -> current account is admin -> ignore brand filter or else add brand filter
        /// 3. Get current account uid to prevent delete current account
        /// 4. Update flag and save to repo
        /// </remarks>>
        /// <returns>Delete status</returns>
        public bool DeleteEmployeeById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing employee id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {EMPLOYEE_PK});
            }

            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Employee, bool>> filter = employee => employee.Id == id && !employee.IsDeleted;

            // If branId is 0 -> current account is admin so ignore brand filter
            if (brandId != 0) {
                Expression<Func<Employee, bool>> filterBrand = employee => employee.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var existedEntity = _employeeRepository.QueryHelper()
                .Include(employee => employee.Account.Role)
                .Include(employee => employee.Brand)
                .GetOne(filter);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed employee entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {EMPLOYEE_PK});
            }

            var currentUserUID = GetCurrentAccountUID();

            if (string.IsNullOrWhiteSpace(currentUserUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            // Prevent delete current account
            if (currentUserUID.Equals(existedEntity?.UID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Try to delete current account, method not allow exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMethodNotAllow(new[] {"uid"});
            }

            existedEntity.IsDeleted = true;
            existedEntity.Account.IsDeleted = true;

            using (var transaction = _employeeRepository.BeginTransaction()) {
                _employeeRepository.Update(existedEntity);
                _logService.WriteLogDelete(existedEntity);
                
                _qaService.DeleteQAsByAccountUID(existedEntity.UID);
                _dishService.UpdateDishesCreateByManagerId(existedEntity.Id);
                _authenticationService.RevokeToken(existedEntity.UID);
                _firebaseService.DeleteAccountFirebase(existedEntity.UID);
                
                _employeeRepository.SaveChanges();
                transaction.Commit();
            }

            return true;
        }
        
        public bool DeleteEmployeesByBrandId(long brandId) {
            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing brand id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {EMPLOYEE_PK});
            }

            var existedEntities = _employeeRepository.QueryHelper()
                .Filter(employee => !employee.IsDeleted && employee.BrandId == brandId)
                .GetAll().ToList();

            existedEntities.ForEach(employee => {
                employee.IsDeleted = true;
                _logService.WriteLogDelete(employee);
                _qaService.DeleteQAsByAccountUID(employee.UID);
                _authenticationService.RevokeToken(employee.UID);
                _dishService.UpdateDishesCreateByManagerId(employee.Id);
            });

            var uids = existedEntities.Select(employee => employee.UID).ToList();

            _firebaseService.DeleteMultipleAccountFirebase(uids);
            _employeeRepository.UpdateRange(existedEntities.ToArray());

            return true;
        }

        private void GetRoleEmployee(bool isManager, Employee entity) {
            if (isManager) {
                var roleEntity = GetRoleEntityById(RoleConstants.MANAGER_ID);
                entity.Account.Role = roleEntity;
                entity.Account.RoleId = RoleConstants.MANAGER_ID;
            }
            else {
                var roleEntity = GetRoleEntityById(RoleConstants.STAFF_ID);
                entity.Account.Role = roleEntity;
                entity.Account.RoleId = RoleConstants.STAFF_ID;
            }
        }

        private Role GetRoleEntityById(long roleId) {
            var roleEntity = _roleRepository.QueryHelper()
                .GetOne(role => role.Id == roleId && !role.IsDeleted);

            return roleEntity;
        }

        private Brand GetBrandEntityById(long brandId) {
            var brandEntity = _brandRepository.QueryHelper()
                .GetOne(brand => brand.Id == brandId && !brand.IsDeleted);

            if (brandEntity == null) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing brandId, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"brandId"});
            }

            return brandEntity;
        }

        /// <summary>
        /// Get page of employee
        /// </summary>
        /// <param name="pageableModel"></param>
        /// <param name="pageableModel.FullName">FullName to search employee</param>
        /// <param name="pageableModel.Email">Email to search employee</param>
        /// <param name="pageableModel.BrandId">BrandId to search employee</param>
        /// <param name="pageableModel.RoleId">RoleId to search employee</param>
        /// <param name="pageableModel.SortField">Field to sort page</param>
        /// <param name="pageableModel.SortDirection">if direction < 0 -> desc, >= 0 -> asc</param>
        /// <param name="pageableModel.PageNumber">Page number need to get</param>
        /// <param name="pageableModel.PageSize">Size of 1 page</param>
        /// <remarks>
        /// 1. FullName and Email is ignore case and contain in entity (like)
        /// 2. Sort direction default is asc
        /// 3. Sort field default is primary key
        /// </remarks>
        /// <returns>Page content list employee</returns>
        public PageResponse<EmployeeDTO> GetPageEmployee(PageableModel<SearchAccountDTO> pageableModel) {
            var fullName = pageableModel.SearchModel?.FullName ?? string.Empty;
            var email = pageableModel.SearchModel?.Email ?? string.Empty;
            var brandId = pageableModel.SearchModel?.BrandId;
            var roleId = pageableModel.SearchModel?.RoleId;

            var currentAccountBrandId = GetCurrentAccountBrandId();
            brandId = currentAccountBrandId != 0 ? currentAccountBrandId : brandId;

            Expression<Func<Employee, bool>> filter = employee => !employee.IsDeleted &&
                                                                  employee.Account.FullName.ToLower()
                                                                      .Contains(fullName.ToLower()) &&
                                                                  employee.Account.Email.ToLower()
                                                                      .Contains(email.ToLower());

            if (brandId != 0) {
                Expression<Func<Employee, bool>> filterBrand = employee => employee.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            if (roleId != 0) {
                Expression<Func<Employee, bool>> filterRole = employee => employee.Account.RoleId == roleId;
                filter = filter.And(filterRole);
            }

            var sortField = pageableModel.SortField ?? EMPLOYEE_PK;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);

            var employeeEntities = _employeeRepository.QueryHelper()
                .Filter(filter)
                .Include(employee => employee.Account)
                .Include(employee => employee.Brand)
                .Include(employee => employee.Account.Role)
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));

            return _mapper.Map<IPage<Employee>, PageResponse<EmployeeDTO>>(employeeEntities).GetTotalPage();
        }
    }
}