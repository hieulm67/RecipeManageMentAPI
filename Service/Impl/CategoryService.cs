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
using RecipeManagementBE.Response.Exception;
using RecipeManagementBE.Util;

namespace RecipeManagementBE.Service.Impl {
    public class CategoryService : BaseService, ICategoryService {
        private readonly ICategoryRepository _categoryRepository;

        private readonly IBrandRepository _brandRepository;

        private readonly IDishService _dishService;

        private readonly ILogService<Category> _logService;

        private readonly ILogger<CategoryService> _logger;

        private readonly IMapper _mapper;

        private const string CATEGORY_PK = "id";

        private const string CATEGORY_NAME = "name";

        public CategoryService(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor,
            ILogService<Category> logService, ILogger<CategoryService> logger, IMapper mapper,
            IBrandRepository brandRepository, IDishService dishService, IAccountRepository accountRepository) : base(httpContextAccessor, accountRepository) {
            _categoryRepository = categoryRepository;
            _logService = logService;
            _logger = logger;
            _mapper = mapper;
            _brandRepository = brandRepository;
            _dishService = dishService;
        }

        /// <summary>
        /// Get all category available in system
        /// </summary>
        /// <param name="brandId">Brand for admin to search categories</param>
        /// <param name="name">Name for search categories</param>
        /// <remarks>
        /// 1. Set name eq empty if null
        /// 2. Get current account brandId, if brandId == 0 -> current account is admin -> ignore brand filter or else add brand filter
        /// </remarks>
        /// <returns>List of categories match brandId and name</returns>
        public List<CategoryDTO> GetAllCategory(long brandId, string name) {
            name ??= string.Empty;

            var currentAccountBrandId = GetCurrentAccountBrandId();
            brandId = currentAccountBrandId != 0 ? currentAccountBrandId : brandId;

            Expression<Func<Category, bool>> filter = category =>
                !category.IsDeleted && category.Name.ToLower().Contains(name.ToLower());

            // If brandId != 0 => current user is employee, add check brand filter to get exist entity in brand of employee
            if (brandId != 0) {
                Expression<Func<Category, bool>> filterBrand = category => category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var categoryEntities = _categoryRepository.QueryHelper()
                .Filter(filter).Include(category => category.Brand)
                .OrderBy(categories => categories.OrderByDescending(category => category.Id))
                .GetAll().ToList();

            return _mapper.Map<List<CategoryDTO>>(categoryEntities);
        }

        /// <summary>
        /// Get all category available include dish (Similar to the above)
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="name"></param>
        /// <param name="dishIsShow"></param>
        /// <returns></returns>
        public List<CategoryDTO> GetAllCategoryIncludeDish(long brandId, string name, bool? dishIsShow) {
            name ??= string.Empty;

            var currentAccountBrandId = GetCurrentAccountBrandId();
            brandId = currentAccountBrandId != 0 ? currentAccountBrandId : brandId;

            Expression<Func<Category, bool>> filter = category =>
                !category.IsDeleted && category.Name.ToLower().Contains(name.ToLower());

            // If brandId != 0 -> current user is employee, add check brand filter to get exist entity in brand of employee
            if (brandId != 0) {
                Expression<Func<Category, bool>> filterBrand = category => category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var query = _categoryRepository.QueryHelper()
                .Filter(filter);

            if (IsStaff()) {
                IncludeInCategory(query, true);
            }
            else {
                IncludeInCategory(query, dishIsShow);
            }

            var categoryEntities = query.Include(category => category.Brand)
                .OrderBy(categories => categories.OrderByDescending(category => category.Id))
                .GetAll().ToList();

            return _mapper.Map<List<CategoryDTO>>(categoryEntities);
        }

        /// <summary>
        /// Get category by id
        /// </summary>
        /// <param name="categoryId">Id of category need to get</param>
        /// <param name="dishIsShow">Filter for dish at status is show</param>
        /// <remarks>
        /// 1. Check category id is provided -> if not throw missing field
        /// 2. Get current account brandId, if brandId == 0 -> current account is admin -> ignore brand filter or else add brand filter
        /// 3. If is staff query include dish at status is show only else include base on dishIsShow param
        /// </remarks>
        /// <returns>Category matched</returns>
        public CategoryDTO GetCategoryById(long categoryId, bool? dishIsShow) {
            if (categoryId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing category id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {CATEGORY_PK});
            }

            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Category, bool>> filter = category =>
                !category.IsDeleted && category.Id == categoryId;

            // If brandId != 0 => current user is employee, add check brand filter to get exist entity in brand of employee
            if (brandId != 0) {
                Expression<Func<Category, bool>> filterBrand = category => category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var query = _categoryRepository.QueryHelper();

            if (IsStaff()) {
                IncludeInCategory(query, true);
            }
            else {
                IncludeInCategory(query, dishIsShow);
            }

            var entity = query.Include(category => category.Brand)
                .GetOne(filter);

            if (entity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed category entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {CATEGORY_PK});
            }

            return _mapper.Map<CategoryDTO>(entity);
        }

        /// <summary>
        /// Add new category to brand 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="dto.BrandId">Brand the category will be add to</param>
        /// <param name="dto.Name">Name of category will be add to (required)</param>
        /// <remarks>
        /// 1. Get current account brandId, if brandId == 0 -> ThrowUnauthorized exception 
        /// 2. Check if admin not provided brandId throw missing field
        /// 3. Check name category existed in brand
        /// 4. Map info to new entity
        /// 5. Add and save change to repo
        /// </remarks>
        /// <returns>Category added</returns>
        public CategoryDTO AddNewCategoryToBrand(CreateCategoryDTO dto) {
            var brandId = GetCurrentAccountBrandId();

            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing category id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            if (_categoryRepository.Exists(category =>
                !category.IsDeleted && category.Name == dto.Name && category.BrandId == brandId)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Category with name in brand had already existed while trying create new category, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {CATEGORY_NAME});
            }

            var brandEntity = GetBrandEntityById(brandId);

            var newEntity = _mapper.Map<Category>(dto);
            newEntity.Brand = brandEntity;

            newEntity = _categoryRepository.Add(newEntity);
            _logService.WriteLogCreate(newEntity);
            _categoryRepository.SaveChanges();

            return _mapper.Map<CategoryDTO>(newEntity);
        }

        /// <summary>
        /// Update category 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="dto.Name">Name of category will be add to (required)</param>
        /// <remarks>
        /// 1. Check id and name category need to update -> if not provided throw missing field
        /// 2. Get current account brandId, if brandId == 0 -> ThrowUnauthorized exception 
        /// 3. Get existed entity -> get existed entity brandId -> update brandId
        /// 4. Check duplicate name in brandId
        /// 5. Map info to existed entity (mapper profile ignore brandId and dishes)
        /// 5. Update and save change to repo
        /// </remarks>
        /// <returns>Category updated</returns>
        public CategoryDTO UpdateCategoryById(CategoryDTO dto) {
            var id = dto.Id;
            var name = dto.Name ?? string.Empty;

            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing category id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {CATEGORY_PK});
            }

            if (string.IsNullOrWhiteSpace(name)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing category name, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {CATEGORY_NAME});
            }

            // Get current account brand id 
            var brandId = GetCurrentAccountBrandId();

            // If brandId != 0 => current user is employee, add check brand filter to check exist entity 
            // Prevent update category not brand of employee
            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            var existedEntity = _categoryRepository.QueryHelper()
                .Include(category => category.Brand)
                .GetOne(category =>
                    !category.IsDeleted && category.Id == id && category.BrandId == brandId);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed category entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {CATEGORY_PK});
            }

            brandId = existedEntity.BrandId;

            if (_categoryRepository.Exists(category =>
                !category.IsDeleted && category.Name.Equals(name) && category.BrandId == brandId &&
                category.Id != id)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Category with name in brand had already existed while trying update category, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {CATEGORY_NAME});
            }

            _mapper.Map(dto, existedEntity);

            existedEntity = _categoryRepository.Update(existedEntity);
            _logService.WriteLogUpdate(existedEntity);
            _categoryRepository.SaveChanges();

            return _mapper.Map<CategoryDTO>(existedEntity);
        }

        /// <summary>
        /// Delete category by id 
        /// </summary>
        /// <param name="id">Id of category need to delete</param>
        /// <remarks>
        /// 1. Check id category need to delete -> if not provided throw missing field
        /// 2. Get current account brandId, if brandId == 0 -> ThrowUnauthorized exception
        /// 3. Get existed entity 
        /// 4. Update flag and save change to repo
        /// </remarks>
        /// <returns>Delete status</returns>
        public bool DeleteCategoryById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing category id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {CATEGORY_PK});
            }

            var brandId = GetCurrentAccountBrandId();

            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            var existedEntity = _categoryRepository.QueryHelper()
                .Include(category => category.Brand)
                .GetOne(category => !category.IsDeleted && category.Id == id && category.BrandId == brandId);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed category entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {CATEGORY_PK});
            }

            existedEntity.IsDeleted = true;
            _categoryRepository.Update(existedEntity);
            _logService.WriteLogDelete(existedEntity);
            _dishService.DeleteDishesByCategoryId(existedEntity.Id);
            _categoryRepository.SaveChanges();

            return true;
        }

        /// <summary>
        /// Delete category by brandId (call in DeleteBrandById) 
        /// </summary>
        /// <param name="brandId">BrandId of category need to delete</param>
        /// <returns>Delete status</returns>
        public bool DeleteCategoriesByBrandId(long brandId) {
            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing brand id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"brandId"});
            }

            var existedEntities = _categoryRepository.QueryHelper()
                .Include(category => category.Brand)
                .Filter(category => !category.IsDeleted && category.BrandId == brandId)
                .GetAll().ToList();

            existedEntities.ForEach(category => {
                category.IsDeleted = true;
                _logService.WriteLogDelete(category);
                _dishService.DeleteDishesByCategoryId(category.Id);
            });

            _categoryRepository.UpdateRange(existedEntities.ToArray());

            return true;
        }

        /// <summary>
        /// Get page of categories
        /// </summary>
        /// <param name="pageableModel"></param>
        /// <param name="pageableModel.Name">Name to search category</param>
        /// <param name="pageableModel.BrandId">BrandId to search category</param>
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
        /// <returns>Page content list employee</returns>
        public PageResponse<CategoryDTO> GetPageCategory(PageableModel<SearchCategoryDTO> pageableModel) {
            var name = pageableModel.SearchModel?.Name ?? "";
            var brandId = pageableModel.SearchModel?.BrandId;

            var currentAccountBrandId = GetCurrentAccountBrandId();
            brandId = currentAccountBrandId != 0 ? currentAccountBrandId : brandId;

            var sortField = pageableModel.SortField ?? CATEGORY_PK;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);

            Expression<Func<Category, bool>> filter = category =>
                !category.IsDeleted && category.Name.ToLower().Contains(name.ToLower());

            // If brandId != 0 => current user is employee, add check brand filter to get exist entity in brand of employee
            if (brandId != 0) {
                Expression<Func<Category, bool>> filterBrand = category => category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var query = _categoryRepository.QueryHelper()
                .Filter(filter);

            var categoryEntities = query.Include(category => category.Brand)
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));

            return _mapper.Map<IPage<Category>, PageResponse<CategoryDTO>>(categoryEntities).GetTotalPage();
        }

        /// <summary>
        /// Get page of categories include dishes
        /// </summary>
        /// <param name="pageableModel"></param>
        /// <param name="pageableModel.Name">Name to search category</param>
        /// <param name="pageableModel.BrandId">BrandId to search category</param>
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
        /// <returns>Page content list employee</returns>
        public PageResponse<CategoryDTO> GetPageCategoryIncludeDish(PageableModel<SearchCategoryDTO> pageableModel) {
            var name = pageableModel.SearchModel?.Name ?? "";
            var brandId = pageableModel.SearchModel?.BrandId;
            var dishIsShow = pageableModel.SearchModel?.DishIsShow;

            var currentAccountBrandId = GetCurrentAccountBrandId();
            brandId = currentAccountBrandId != 0 ? currentAccountBrandId : brandId;

            var sortField = pageableModel.SortField ?? CATEGORY_PK;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);

            Expression<Func<Category, bool>> filter = category =>
                !category.IsDeleted && category.Name.ToLower().Contains(name.ToLower());

            // If brandId != 0 => current user is employee, add check brand filter to get exist entity in brand of employee
            if (brandId != 0) {
                Expression<Func<Category, bool>> filterBrand = category => category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var query = _categoryRepository.QueryHelper()
                .Filter(filter);

            if (IsStaff()) {
                IncludeInCategory(query, true);
            }
            else {
                IncludeInCategory(query, dishIsShow);
            }

            var categoryEntities = query.Include(category => category.Brand)
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));

            return _mapper.Map<IPage<Category>, PageResponse<CategoryDTO>>(categoryEntities).GetTotalPage();
        }

        private Brand GetBrandEntityById(long brandId) {
            var brandEntity = _brandRepository.QueryHelper()
                .GetOne(brand => brand.Id == brandId && !brand.IsDeleted);

            if (brandEntity == null) {
                throw new BusinessException(ExceptionCodeMapping.ITEM_ALREADY_EXISTED,
                    new ExceptionParams {Params = new[] {"brandId"}});
            }

            return brandEntity;
        }

        private static void IncludeInCategory(IFluentRepository<Category> query, bool? isShow) {
            if (isShow.HasValue) {
                if (isShow.Value) {
                    query.Include(source => source
                        .Include(category => category.Dishes.Where(dish => dish.IsShow))
                        .ThenInclude(dish => dish.Manager)
                        .Include(category => category.Dishes.Where(dish => dish.IsShow))
                        .ThenInclude(dish => dish.Recipes.Where(recipe => recipe.IsUsing && !recipe.IsDeleted))
                    );
                }
                else {
                    query.Include(source => source
                        .Include(category => category.Dishes.Where(dish => dish.IsShow == false))
                        .ThenInclude(dish => dish.Manager)
                        .Include(category => category.Dishes.Where(dish => dish.IsShow == false))
                        .ThenInclude(dish => dish.Recipes.Where(recipe => recipe.IsUsing && !recipe.IsDeleted))
                    );
                }
            }
            else {
                query.Include(source => source
                    .Include(category => category.Dishes)
                    .ThenInclude(dish => dish.Manager)
                    .Include(category => category.Dishes)
                    .ThenInclude(dish => dish.Recipes.Where(recipe => recipe.IsUsing && !recipe.IsDeleted))
                );
            }
        }
    }
}