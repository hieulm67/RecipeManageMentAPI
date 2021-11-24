using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using JHipsterNet.Core.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecipeManagementBE.Common.Repository;
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
    public class DishService : BaseService, IDishService {
        private readonly IDishRepository _dishRepository;

        private readonly ICategoryRepository _categoryRepository;

        private readonly IEmployeeRepository _employeeRepository;

        private readonly INotificationService _notificationService;

        private readonly IRecipeService _recipeService;

        private readonly ILogService<Dish> _logService;

        private readonly ILogger<DishService> _logger;

        private readonly IMapper _mapper;

        private const string DISH_PK = "id";

        private const string DISH_NAME = "name";

        public DishService(IDishRepository dishRepository, IHttpContextAccessor httpContextAccessor,
            ILogService<Dish> logService, ILogger<DishService> logger, IMapper mapper,
            ICategoryRepository categoryRepository, IEmployeeRepository employeeRepository,
            INotificationService notificationService, IRecipeService recipeService, IAccountRepository accountRepository) :
            base(httpContextAccessor, accountRepository) {
            _dishRepository = dishRepository;
            _logService = logService;
            _logger = logger;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _recipeService = recipeService;
        }

        /// <summary>
        /// Get all dish in system
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="dto.BrandId">BrandId for search dish</param>
        /// <param name="dto.CategoriesId">Categories for search dish</param>
        /// <param name="dto.Name">Name for search dish</param>
        /// <param name="dto.DishIsShow">Dish status for search dish</param>
        /// <remarks>
        /// 1. Get current account brandId, if brandId == 0 -> current account is admin -> get brandId provided else take current account brandId
        /// 2. Add provided item to filter
        /// </remarks>
        /// <returns>All dish matched</returns>
        public List<DishDTO> GetAllDish(SearchDishDTO dto) {
            var name = dto.Name ?? string.Empty;
            var categoriesId = dto.CategoriesId ?? new List<long>();
            var toolsId = dto.ToolsId ?? new List<long>();
            var ingredientsId = dto.IngredientsId ?? new List<long>();
            var dishIsShow = dto.DishIsShow;
            var brandId = dto.BrandId;

            var currentAccountBrandId = GetCurrentAccountBrandId();
            brandId = currentAccountBrandId != 0 ? currentAccountBrandId : brandId;

            Expression<Func<Dish, bool>> filter = dish =>
                !dish.IsDeleted && dish.Name.ToLower().Contains(name.ToLower());

            if (categoriesId.Count > 0) {
                Expression<Func<Dish, bool>> filterCategories = dish => categoriesId.Contains(dish.CategoryId.Value);
                filter = filter.And(filterCategories);
            }

            if (toolsId.Count > 0) {
                Expression<Func<Dish, bool>> filterTools = dish => dish.Recipes.FirstOrDefault(recipe => recipe.IsUsing)
                    .RecipeTools.FirstOrDefault(rt => toolsId.Contains(rt.ToolId) && !rt.IsDeleted) != null;
                filter = filter.And(filterTools);
            }

            if (ingredientsId.Count > 0) {
                Expression<Func<Dish, bool>> filterIngredients = dish => dish.Recipes.FirstOrDefault(recipe => recipe.IsUsing)
                    .RecipeDetails.FirstOrDefault(rd => ingredientsId.Contains(rd.IngredientId) && !rd.IsDeleted) != null;
                filter = filter.And(filterIngredients);
            }

            // If brandId != 0 => current user is employee, add check brand filter to get exist entity in brand of employee
            if (brandId != 0) {
                Expression<Func<Dish, bool>> filterBrand = dish => dish.Category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            if (dishIsShow.HasValue) {
                Expression<Func<Dish, bool>> filterIsShow = dish => dish.IsShow == dishIsShow.Value;
                filter = filter.And(filterIsShow);
            }

            var query = _dishRepository.QueryHelper();
            IncludeInDish(query);

            var dishEntities = query.Filter(filter)
                .OrderBy(dishes => dishes.OrderByDescending(dish => dish.Id))
                .GetAll().ToList();

            return _mapper.Map<List<DishDTO>>(dishEntities);
        }

        /// <summary>
        /// Get dish by id
        /// </summary>
        /// <param name="id">Id of dish need to get</param>
        /// <remarks>
        /// 1. Get current account brandId, if brandId == 0 -> current account is admin -> get brandId provided else take current account brandId
        /// 2. Add provided item to filter
        /// </remarks>
        /// <returns>Dish matched</returns>
        public DishDTO GetDishById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing dish id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {DISH_PK});
            }

            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Dish, bool>> filter = dish => dish.Id == id && !dish.IsDeleted;

            if (brandId != 0) {
                Expression<Func<Dish, bool>> filterBrand = dish => dish.Category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var query = _dishRepository.QueryHelper();

            IncludeInDish(query);

            var entity = query.GetOne(filter);

            if (entity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed dish entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {DISH_PK});
            }

            return _mapper.Map<DishDTO>(entity);
        }

        /// <summary>
        /// Get page of dishes
        /// </summary>
        /// <param name="pageableModel"></param>
        /// <param name="pageableModel.Name">Name to search category</param>
        /// <param name="pageableModel.BrandId">BrandId to search category</param>
        /// <param name="pageableModel.CategoriesId">CategoriesId to search category</param>
        /// <param name="pageableModel.SortField">Field to sort page</param>
        /// <param name="pageableModel.SortDirection">if direction < 0 -> desc, >= 0 -> asc</param>
        /// <param name="pageableModel.PageNumber">Page number need to get</param>
        /// <param name="pageableModel.PageSize">Size of 1 page</param>
        /// <remarks>
        /// 1. Get current account brandId, if brandId == 0 -> current account is admin -> get brandId provided else take current account brandId
        /// 1. Name is ignore case and contain in entity (like)
        /// 2. Sort direction default is asc
        /// 3. Sort field default is primary key
        /// </remarks>
        /// <returns>Page content list dishes</returns>
        public PageResponse<DishDTO> GetPageDish(PageableModel<SearchDishDTO> pageableModel) {
            var name = pageableModel.SearchModel?.Name ?? string.Empty;
            var brandId = pageableModel.SearchModel?.BrandId;
            var dishIsShow = pageableModel.SearchModel?.DishIsShow;
            var categoryIds = pageableModel.SearchModel?.CategoriesId ?? new List<long>();
            var toolsId =  pageableModel.SearchModel?.ToolsId ?? new List<long>();
            var ingredientsId =  pageableModel.SearchModel?.IngredientsId ?? new List<long>();

            var currentAccountBrandId = GetCurrentAccountBrandId();
            brandId = currentAccountBrandId != 0 ? currentAccountBrandId : brandId;

            var sortField = pageableModel.SortField ?? DISH_PK;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);

            Expression<Func<Dish, bool>> filter = dish =>
                !dish.IsDeleted && dish.Name.ToLower().Contains(name.ToLower());

            if (categoryIds.Count != 0) {
                Expression<Func<Dish, bool>> filterCategories = dish => categoryIds.Contains(dish.CategoryId.Value);
                filter = filter.And(filterCategories);
            }
            
            if (toolsId.Count > 0) {
                Expression<Func<Dish, bool>> filterTools = dish => dish.Recipes.FirstOrDefault(recipe => recipe.IsUsing)
                    .RecipeTools.FirstOrDefault(rt => toolsId.Contains(rt.ToolId) && !rt.IsDeleted) != null;
                filter = filter.And(filterTools);
            }

            if (ingredientsId.Count > 0) {
                Expression<Func<Dish, bool>> filterIngredients = dish => dish.Recipes.FirstOrDefault(recipe => recipe.IsUsing)
                    .RecipeDetails.FirstOrDefault(rd => ingredientsId.Contains(rd.IngredientId) && !rd.IsDeleted) != null;
                filter = filter.And(filterIngredients);
            }

            // If brandId != 0 => current user is employee, add check brand filter to get exist entity in brand of employee
            if (brandId != 0) {
                Expression<Func<Dish, bool>> filterBrand = dish => dish.Category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            if (dishIsShow.HasValue) {
                Expression<Func<Dish, bool>> filterIsShow = dish => dish.IsShow == dishIsShow.Value;
                filter = filter.And(filterIsShow);
            }

            var query = _dishRepository.QueryHelper();
            IncludeInDish(query);

            var dishEntities = query.Filter(filter)
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));

            return _mapper.Map<IPage<Dish>, PageResponse<DishDTO>>(dishEntities).GetTotalPage();
        }

        /// <summary>
        /// Create dish to category
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="dto.Name">Required</param>
        /// <param name="dto.CategoryId">Required</param>
        /// <remarks>
        /// 1. Get current account brandId and uid, if brandId == 0 || uid == empty -> ThrowUnauthorized exception
        /// 2. Get category entity by categoryId and brandId
        /// 3. Get manager entity by current account uid 
        /// 4. Check duplicate name in 1 category
        /// 5. Map info to entity
        /// 6. Add and save to repo
        /// </remarks>
        /// <returns>Updated dish</returns>
        public DishDTO AddNewDish(CreateDishDTO dto) {
            var categoryId = dto.CategoryId;
            var name = dto.Name ?? string.Empty;

            if (categoryId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing category id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"categoryId"});
            }

            if (string.IsNullOrWhiteSpace(name)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing dish name, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {DISH_NAME});
            }

            var brandId = GetCurrentAccountBrandId();
            var currentUserUID = GetCurrentAccountUID();

            // Only manager can use this method if not has brandId -> Unauthorized
            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            if (string.IsNullOrWhiteSpace(currentUserUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            // Get category entity add to existed dish entity after map to prevent update category detail
            var categoryEntity = _categoryRepository.QueryHelper()
                .Include(category => category.Brand)
                .GetOne(category =>
                !category.IsDeleted && category.Id == categoryId && category.BrandId == brandId);

            if (categoryEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed category entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"categoryId"});
            }

            // Get manager entity add to existed dish entity after map to prevent update manager detail
            var managerEntity = _employeeRepository.QueryHelper().GetOne(employee => !employee.IsDeleted &&
                employee.Account.UID.Equals(currentUserUID) &&
                employee.IsManager);

            if (managerEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed employee (manager) entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"uid"});
            }

            // Check duplicate name in 1 category
            if (_dishRepository.Exists(dish =>
                !dish.IsDeleted && dish.Name.Equals(name) && dish.CategoryId == categoryId)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Dish with name in category had already existed while trying create new dish, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {DISH_NAME});
            }

            var newEntity = _mapper.Map<Dish>(dto);
            newEntity.Category = categoryEntity;
            newEntity.Manager = managerEntity;

            newEntity = _dishRepository.Add(newEntity);

            _logService.WriteLogCreate(newEntity);
            SendNotificationToEmployee(brandId, newEntity);
            
            _dishRepository.SaveChanges();

            return _mapper.Map<DishDTO>(newEntity);
        }

        /// <summary>
        /// Update dish by id
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="dto.Id">Required</param>
        /// <param name="dto.Name">Required</param>
        /// <param name="dto.CategoryId">Required</param>
        /// <param name="dto.ManagerId">Required</param>
        /// <remarks>
        /// 1. Get current account brandId, if brandId == 0 -> ThrowUnauthorized exception
        /// 2. Get category entity by categoryId and brandId
        /// 3. Get manager entity by managerId and brandId
        /// 4. Get existed dish from repo
        /// 5. Check duplicate name in 1 category
        /// 6. Map info to entity
        /// 7. Update and save to repo
        /// </remarks>
        /// <returns>Updated dish</returns>
        public DishDTO UpdateDishById(DishDTO dto) {
            var id = dto.Id;
            var name = dto.Name ?? string.Empty;
            var categoryId = dto.CategoryId;

            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing dish id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {DISH_PK});
            }

            if (categoryId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing category id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"categoryId"});
            }

            if (string.IsNullOrWhiteSpace(name)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing dish name, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {DISH_NAME});
            }

            // Get current account brand id 
            var brandId = GetCurrentAccountBrandId();

            // Only manager can use this method if not has brandId -> Unauthorized
            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            // Get category entity add to existed dish entity after map to prevent update category detail
            var categoryEntity = _categoryRepository.QueryHelper().GetOne(category =>
                !category.IsDeleted && category.Id == categoryId && category.BrandId == brandId);

            if (categoryEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed category entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"categoryId"});
            }

            // Check duplicate name in 1 category
            if (_dishRepository.Exists(dish =>
                !dish.IsDeleted && dish.Name.Equals(name) && dish.CategoryId == categoryId && dish.Id != id)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Dish with name in category had already existed while trying update dish, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {DISH_NAME});
            }

            var query = _dishRepository.QueryHelper();
            IncludeInDish(query);
            
            // Get existed dish
            var existedEntity = query.GetOne(dish => !dish.IsDeleted && dish.Id == id && dish.Category.BrandId == brandId);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed dish entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {DISH_PK});
            }

            _mapper.Map(dto, existedEntity);
            existedEntity.Category = categoryEntity;

            existedEntity = _dishRepository.Update(existedEntity);
            _logService.WriteLogUpdate(existedEntity);
            _dishRepository.SaveChanges();

            return _mapper.Map<DishDTO>(existedEntity);
        }

        /// <summary>
        /// Delete dish by id
        /// </summary>
        /// <param name="id">Id of dish need to delete</param>
        /// <remarks>
        /// 1. Check id is provided or not
        /// 2. Get current account brandId, if brandId == 0 -> ThrowUnauthorized exception
        /// 3. Get existed entity from repo
        /// 4. Update flag and save to repo
        /// </remarks>
        /// <returns>Delete status</returns>
        public bool DeleteDishById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing dish id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {DISH_PK});
            }

            var brandId = GetCurrentAccountBrandId();

            // Only manager can use this method if not has brandId -> Unauthorized
            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            var existedEntity = _dishRepository.QueryHelper()
                .Include(dish => dish.Category.Brand)
                .GetOne(dish =>
                    !dish.IsDeleted && dish.Id == id && dish.Category.BrandId == brandId);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed dish entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {DISH_PK});
            }

            existedEntity.IsDeleted = true;
            _dishRepository.Update(existedEntity);
            _logService.WriteLogDelete(existedEntity);
            _recipeService.DeleteRecipesByDishId(existedEntity.Id);
            _dishRepository.SaveChanges();

            return true;
        }
        
        public bool DeleteDishesByCategoryId(long categoryId) {
            if (categoryId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing category id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"categoryId"});
            }

            var existedEntities = _dishRepository.QueryHelper()
                .Include(dish => dish.Category.Brand)
                .Filter(dish => !dish.IsDeleted && dish.CategoryId == categoryId)
                .GetAll().ToList();

            existedEntities.ForEach(dish => {
                dish.IsDeleted = true;
                _logService.WriteLogDelete(dish);
                _recipeService.DeleteRecipesByDishId(dish.Id);
            });

            _dishRepository.UpdateRange(existedEntities.ToArray());

            return true;
        }
        
        public bool UpdateDishesCreateByManagerId(long managerId) {
            if (managerId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing manager id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"managerId"});
            }

            var existedEntities = _dishRepository.QueryHelper()
                .Include(dish => dish.Category.Brand)
                .Filter(dish => !dish.IsDeleted && dish.ManagerId == managerId)
                .GetAll().ToList();

            existedEntities.ForEach(dish => {
                dish.ManagerId = null;
                dish.Manager = null;
                _logService.WriteLogUpdate(dish);
            });

            _dishRepository.UpdateRange(existedEntities.ToArray());

            return true;
        }

        private void IncludeInDish(IFluentRepository<Dish> query) {
            query.Include(dish => dish.Manager.Account.Role)
                .Include(dish => dish.Manager.Brand)
                .Include(dish => dish.Category);
            if (IsStaff()) {
                query.Include(source => source
                    .Include(dish => dish.Recipes.Where(recipe => !recipe.IsDeleted && recipe.IsUsing))
                    .ThenInclude(recipe => recipe.ProcessingSteps.Where(ps => !ps.IsDeleted))
                    .Include(dish => dish.Recipes.Where(recipe => !recipe.IsDeleted && recipe.IsUsing))
                    .ThenInclude(recipe => recipe.Qas.Where(qa => !qa.IsDeleted))
                    .Include(dish => dish.Recipes.Where(recipe => !recipe.IsDeleted && recipe.IsUsing))
                    .ThenInclude(recipe => recipe.RecipeDetails.Where(rd => !rd.IsDeleted))
                    .ThenInclude(rd => rd.Ingredient)
                    .Include(dish => dish.Recipes.Where(recipe => !recipe.IsDeleted && recipe.IsUsing))
                    .ThenInclude(recipe => recipe.RecipeTools.Where(rt => !rt.IsDeleted))
                    .ThenInclude(rt => rt.Tool));
            }
            else {
                query.Include(source => source
                    .Include(dish => dish.Recipes.Where(recipe => !recipe.IsDeleted))
                    .ThenInclude(recipe => recipe.ProcessingSteps.Where(ps => !ps.IsDeleted))
                    .Include(dish => dish.Recipes.Where(recipe => !recipe.IsDeleted))
                    .ThenInclude(recipe => recipe.Qas.Where(qa => !qa.IsDeleted))
                    .Include(dish => dish.Recipes.Where(recipe => !recipe.IsDeleted))
                    .ThenInclude(recipe => recipe.RecipeDetails.Where(rd => !rd.IsDeleted))
                    .ThenInclude(rd => rd.Ingredient)
                    .Include(dish => dish.Recipes.Where(recipe => !recipe.IsDeleted))
                    .ThenInclude(recipe => recipe.RecipeTools.Where(rt => !rt.IsDeleted))
                    .ThenInclude(rt => rt.Tool));
            }
        }

        private void SendNotificationToEmployee(long brandId, Dish dish) {
            var employeeEntities = _employeeRepository.QueryHelper()
                .Include(employee => employee.Account.Role)
                .Include(employee => employee.Brand)
                .Filter(employee => !employee.IsDeleted && employee.BrandId == brandId)
                .GetAll().ToList();
            
            employeeEntities.ForEach(employee => _notificationService.AddNewNotification(
                $"Your brand had added new dish {dish.Name} in category {dish.Category.Name}.", employee.Account, "Dish", dish.Id));
        }
    }
}