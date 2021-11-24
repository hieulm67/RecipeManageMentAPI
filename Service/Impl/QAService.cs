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
using RecipeManagementBE.Util;

namespace RecipeManagementBE.Service.Impl {
    public class QAService : BaseService, IQAService {
        private readonly IQARepository _qaRepository;

        private readonly IEmployeeRepository _employeeRepository;

        private readonly IRecipeRepository _recipeRepository;

        private readonly INotificationService _notificationService;

        private readonly ILogger<QAService> _logger;

        private readonly IMapper _mapper;

        private const string QA_QA_TIME = "qaTime";

        public QAService(IHttpContextAccessor httpContextAccessor, IQARepository qaRepository,
            ILogger<QAService> logger, IMapper mapper,
            IEmployeeRepository employeeRepository, IRecipeRepository recipeRepository,
            INotificationService notificationService, IAccountRepository accountRepository) : base(httpContextAccessor, accountRepository) {
            _qaRepository = qaRepository;
            _logger = logger;
            _employeeRepository = employeeRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public List<QaDTO> GetAllQAByRecipeId(SearchQADTO dto) {
            var recipeId = dto.RecipeId;
            var qaTime = dto.QATime;

            if (recipeId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing recipe id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"recipeId"});
            }

            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Qa, bool>> filter = qa => !qa.IsDeleted && qa.RecipeId == recipeId && qa.QaParent == null;

            if (brandId != 0) {
                Expression<Func<Qa, bool>> filterBrand = qa => qa.Recipe.Dish.Category.BrandId == brandId && qa.Account.Employee.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            if (qaTime.Year != 1) {
                Expression<Func<Qa, bool>> filterTime = qa =>
                    TimeSpan.Compare(qa.QaTime.TimeOfDay, qaTime.TimeOfDay) == 0;
                filter = filter.And(filterTime);
            }

            var qaEntities = _qaRepository.QueryHelper()
                .Include(qa => qa.Account.Role)
                .Include(qa => qa.QaChild.Account.Role)
                .Include(qa => qa.Recipe)
                .Filter(filter)
                .OrderBy(qas => qas.OrderByDescending(qa => qa.QaTime))
                .GetAll().ToList();

            return _mapper.Map<List<QaDTO>>(qaEntities);
        }

        public ResultSearchQAIncludeMarkedId GetAllQAByRecipeIdIncludeMarkedQA(SearchQANotifiedDTO dto) {
            var recipeId = dto.RecipeId;
            var to = dto.To ?? string.Empty;
            var notifiedId = dto.NotifiedId;

            if (recipeId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing recipe id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"recipeId"});
            }

            var brandId = GetCurrentAccountBrandId();

            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            var qaEntities = _qaRepository.QueryHelper()
                .Include(qa => qa.Account.Role)
                .Include(qa => qa.QaChild.Account.Role)
                .Include(qa => qa.Recipe)
                .Filter(qa =>
                    !qa.IsDeleted && qa.RecipeId == recipeId && qa.QaParent == null &&
                    qa.Recipe.Dish.Category.BrandId == brandId)
                .OrderBy(qas => qas.OrderByDescending(qa => qa.QaTime))
                .GetAll().ToList();

            var qaNotified = _qaRepository.QueryHelper()
                .GetOne(qa => !qa.IsDeleted && qa.RecipeId == recipeId && qa.Recipe.Dish.Category.BrandId == brandId
                              && qa.Account.UID.Equals(to) &&
                              qa.Id == notifiedId);

            if (qaNotified == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found qa entity match filter notification.To, notification.NotifiedId, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"to", "sendingTime"});
            }

            return new ResultSearchQAIncludeMarkedId{
                ListQA = _mapper.Map<List<QaDTO>>(qaEntities),
                IdMarked = qaNotified.Id,
                IsChild = qaNotified.QaParentId.HasValue,
                ParentId = qaNotified.QaParentId ?? 0
            };
        }

        public PageResponse<QaDTO> GetPageQAByRecipeId(PageableModel<SearchQADTO> pageableModel) {
            var recipeId = pageableModel.SearchModel?.RecipeId;
            var qaTime = pageableModel.SearchModel?.QATime;

            if (recipeId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing recipe id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"recipeId"});
            }

            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Qa, bool>> filter = qa =>
                !qa.IsDeleted && qa.RecipeId == recipeId && qa.QaParent == null;

            if (brandId != 0) {
                Expression<Func<Qa, bool>> filterBrand = qa => qa.Recipe.Dish.Category.BrandId == brandId && qa.Account.Employee.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            if (qaTime.HasValue && qaTime.Value.Year != 1) {
                Expression<Func<Qa, bool>> filterTime = qa =>
                    TimeSpan.Compare(qa.QaTime.TimeOfDay, qaTime.Value.TimeOfDay) == 0;
                filter = filter.And(filterTime);
            }

            var sortField = pageableModel.SortField ?? QA_QA_TIME;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);

            var qaEntities = _qaRepository.QueryHelper().Filter(filter)
                .Include(qa => qa.Account.Role)
                .Include(qa => qa.QaChild.Account.Role)
                .Include(qa => qa.Recipe)
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));

            return _mapper.Map<IPage<Qa>, PageResponse<QaDTO>>(qaEntities).GetTotalPage();
        }

        public QaDTO GetQAById(long id) {
            var brandId = GetCurrentAccountBrandId();

            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            var qaNotified = _qaRepository.QueryHelper()
                .Include(qa => qa.Account.Role)
                .Include(qa => qa.QaChild.Account.Role)
                .Include(qa => qa.Recipe)
                .GetOne(qa => qa.Recipe.Dish.Category.BrandId == brandId &&
                              qa.Id == id && !qa.IsDeleted);

            if (qaNotified == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found qa entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"id"});
            }

            return _mapper.Map<QaDTO>(qaNotified);
        }

        public QaDTO PostNewQAInRecipe(CreateQADTO dto) {
            var recipeId = dto.RecipeId;
            var qaParentId = dto.QaParentId;
            var isReply = dto.IsReply;

            if (recipeId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing recipe id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"recipeId"});
            }

            var branId = GetCurrentAccountBrandId();

            if (branId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            var recipeEntity = _recipeRepository.QueryHelper()
                .Include(recipe => recipe.Dish.Manager.Account)
                .GetOne(recipe => !recipe.IsDeleted && recipe.Id == recipeId && recipe.Dish.Category.BrandId == branId);

            if (recipeEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found recipe entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"recipeId"});
            }

            var currentUserUID = GetCurrentAccountUID();

            if (string.IsNullOrWhiteSpace(currentUserUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            var employeeEntity = _employeeRepository.QueryHelper()
                .Include(employee => employee.Account.Role)
                .GetOne(employee => !employee.IsDeleted && employee.UID.Equals(currentUserUID) && employee.BrandId == branId);

            if (employeeEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found employee entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"uid"});
            }

            Qa qaParentEntity = null;
            if (qaParentId != 0 && isReply) {
                if (!employeeEntity.IsManager) {
                    _logger.LogError(
                        "[{Time}] [{ApplicationName}]: Current account is employee then answer a question method is not allow, method not allow throw",
                        DateTime.Now ,Constants.APPLICATION_NAME);
                    ThrowMethodNotAllow(new[] {"role"});
                }
                
                qaParentEntity = _qaRepository.QueryHelper()
                    .Include(qa => qa.Account)
                    .GetOne(qa => !qa.IsDeleted && qa.Id == qaParentId && qa.QaParentId == null && qa.QaChild == null);

                if (qaParentEntity == null) {
                    _logger.LogError(
                        "[{Time}] [{ApplicationName}]: Can't found parent qa entity match filter, entity not found exception throw",
                        DateTime.Now ,Constants.APPLICATION_NAME);
                    ThrowEntityNotFound(new[] {"qaParentId"});
                }
            }
            else if (employeeEntity.IsManager) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Current account is manager then post a question method is not allow, method not allow throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMethodNotAllow(new[] {"role"});
            }

            var newEntity = _mapper.Map<Qa>(dto);
            newEntity.Account = employeeEntity.Account;
            newEntity.Recipe = recipeEntity;

            if (qaParentEntity != null) {
                newEntity.QaParent = qaParentEntity;
                newEntity.IsReply = true;
            }

            using (var transaction = _qaRepository.BeginTransaction()){
                newEntity = _qaRepository.Add(newEntity);
                _qaRepository.SaveChanges();

                if (qaParentEntity != null) {
                    _notificationService.AddNewNotification(
                        $"Account: {employeeEntity.Account.Email} with name {employeeEntity.Account.FullName} " +
                        $"had replied in recipe has id {recipeEntity.Id} of dish {recipeEntity.Dish.Name}",
                        qaParentEntity.Account, "QA", newEntity.Id);
                }
                else {
                    _notificationService.AddNewNotification(
                        $"Account: {employeeEntity.Account.Email} with name {employeeEntity.Account.FullName} " +
                        $"had posted a question in recipe has id {recipeEntity.Id} of dish {recipeEntity.Dish.Name}",
                        recipeEntity.Dish.Manager.Account, "QA", newEntity.Id);
                }

                _qaRepository.SaveChanges();
                transaction.Commit();
            }

            return _mapper.Map<QaDTO>(newEntity);
        }

        public QaDTO UpdateQAById(QaDTO dto) {
            var id = dto.Id;

            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing qa id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"id"});
            }

            var currentAccountUID = GetCurrentAccountUID();

            if (string.IsNullOrWhiteSpace(currentAccountUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            var existedEntity = _qaRepository.QueryHelper()
                .Include(qa => qa.Account.Role)
                .Include(qa => qa.QaChild.Account.Role)
                .Include(qa => qa.Recipe)
                .GetOne(qa => !qa.IsDeleted && qa.Id == id && qa.UID.Equals(currentAccountUID));

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed qa entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"id"});
            }

            existedEntity.Content = dto.Content;

            existedEntity = _qaRepository.Update(existedEntity);
            _qaRepository.SaveChanges();

            return _mapper.Map<QaDTO>(existedEntity);
        }

        public bool DeleteQAById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing qa id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"id"});
            }

            var currentAccountUID = GetCurrentAccountUID();

            if (string.IsNullOrWhiteSpace(currentAccountUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            var existedEntity = _qaRepository.QueryHelper()
                .Include(qa => qa.QaChild)
                .Include(qa => qa.QaParent)
                .Include(qa => qa.Recipe)
                .GetOne(qa => !qa.IsDeleted && qa.Id == id && qa.UID.Equals(currentAccountUID));

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed qa entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"id"});
            }

            existedEntity.IsDeleted = true;
            if (existedEntity.QaChild != null) {
                existedEntity.QaChild.IsDeleted = true;
            }

            if (existedEntity.QaParent != null) {
                existedEntity.QaParent = null;
                existedEntity.QaParentId = null;
            }

            _qaRepository.Update(existedEntity);
            _qaRepository.SaveChanges();

            return true;
        }
        
        public bool DeleteQAsByAccountUID(string accountUID) {
            if (string.IsNullOrWhiteSpace(accountUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing account uid, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"employeeUID"});
            }
            
            var existedEntities = _qaRepository.QueryHelper()
                            .Filter(qa => !qa.IsDeleted && qa.UID == accountUID)
                            .GetAll().ToList();

            existedEntities.ForEach(qa => {
                qa.IsDeleted = true;
                if (qa.QaChild != null) {
                    qa.QaChild.IsDeleted = true;
                }
            });

            _qaRepository.UpdateRange(existedEntities.ToArray());

            return true;
        }
    }
}