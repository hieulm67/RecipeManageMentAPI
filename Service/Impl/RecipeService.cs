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
using RecipeManagementBE.Response;
using RecipeManagementBE.Util;

namespace RecipeManagementBE.Service.Impl {
    public class RecipeService : BaseService, IRecipeService {
        private readonly IRecipeRepository _recipeRepository;

        private readonly IDishRepository _dishRepository;

        private readonly ILogService<Recipe> _logService;

        private readonly ILogger<RecipeService> _logger;

        private readonly IMapper _mapper;

        private const string RECIPE_PK = "id";

        public RecipeService(IRecipeRepository recipeRepository, ILogService<Recipe> logService,
            ILogger<RecipeService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IDishRepository dishRepository, IAccountRepository accountRepository) : base(httpContextAccessor, accountRepository) {
            _recipeRepository = recipeRepository;
            _logService = logService;
            _logger = logger;
            _mapper = mapper;
            _dishRepository = dishRepository;
        }


        public List<RecipeDTO> GetAllRecipeByDishId(long dishId) {
            if (dishId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing dish id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"dishId"});
            }

            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Recipe, bool>> filter = recipe =>
                !recipe.IsDeleted && recipe.DishId == dishId;

            if (brandId != 0) {
                Expression<Func<Recipe, bool>> filterBrand = recipe => recipe.Dish.Category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var query = _recipeRepository.QueryHelper();
            IncludeInRecipe(query);

            var recipeEntities = query.Filter(filter)
                .OrderBy(recipes => recipes.OrderByDescending(recipe => recipe.Id))
                .GetAll().ToList();

            return _mapper.Map<List<RecipeDTO>>(recipeEntities);
        }

        public RecipeDTO GetRecipeById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing recipe id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"id"});
            }

            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Recipe, bool>> filter = recipe =>
                !recipe.IsDeleted && recipe.Id == id;

            if (brandId != 0) {
                Expression<Func<Recipe, bool>> filterBrand = recipe => recipe.Dish.Category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var query = _recipeRepository.QueryHelper();
            IncludeInRecipe(query);

            var entity = query.GetOne(filter);

            if (entity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed recipe entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {RECIPE_PK});
            }

            return _mapper.Map<RecipeDTO>(entity);
        }

        public PageResponse<RecipeDTO> GetPageRecipe(PageableModel<long> pageableModel) {
            var dishId = pageableModel.SearchModel;

            if (dishId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing dish id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"dishId"});
            }

            var brandId = GetCurrentAccountBrandId();

            Expression<Func<Recipe, bool>> filter = recipe =>
                !recipe.IsDeleted && recipe.DishId == dishId;

            if (brandId != 0) {
                Expression<Func<Recipe, bool>> filterBrand = recipe => recipe.Dish.Category.BrandId == brandId;
                filter = filter.And(filterBrand);
            }

            var sortField = pageableModel.SortField ?? RECIPE_PK;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);

            var query = _recipeRepository.QueryHelper();
            IncludeInRecipe(query);

            var recipeEntities = query.Filter(filter)
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));

            return _mapper.Map<IPage<Recipe>, PageResponse<RecipeDTO>>(recipeEntities).GetTotalPage();
        }

        public RecipeDTO AddNewRecipe(CreateRecipeDTO dto) {
            var dishId = dto.DishId;
            var image = dto.ImageDescription ?? string.Empty;
            var listIngredients = dto.RecipeDetails ?? new HashSet<RecipeDetailDTO>();
            var listTools = dto.RecipeTools ?? new HashSet<RecipeToolDTO>();
            var listSteps = dto.ProcessingSteps ?? new HashSet<ProcessingStepDTO>();

            if (dishId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing dish id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"dishId"});
            }
            
            if (string.IsNullOrWhiteSpace(image)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing recipe image, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"recipeImage"});
            }

            if (CheckInvalidListIngredient(listIngredients)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: List ingredient contains invalid obj, item not correct format exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowItemNotCorrectFormat(new[] {"ingredientId"});
            }

            if (CheckInvalidListStep(listSteps)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: List step contains invalid obj, item not correct format exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowItemNotCorrectFormat(new[] {"stepNumber"});
            }

            if (CheckInvalidListTool(listTools)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: List tool contains invalid obj, item not correct format exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowItemNotCorrectFormat(new[] {"toolId"});
            }

            var brandId = GetCurrentAccountBrandId();

            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            var dishEntity = _dishRepository.QueryHelper()
                .GetOne(dish => !dish.IsDeleted && dish.Id == dishId && dish.Category.BrandId == brandId);

            if (dishEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed dish entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"dishId"});
            }

            var newEntity = _mapper.Map<Recipe>(dto);
            newEntity.Dish = dishEntity;
            newEntity.IsUsing = false;

            using (var transaction = _recipeRepository.BeginTransaction()) {
                newEntity = _recipeRepository.Add(newEntity);
                _recipeRepository.SaveChanges();
                
                _logService.WriteLogCreate(newEntity);
                _recipeRepository.SaveChanges();
                transaction.Commit();
            }

            return _mapper.Map<RecipeDTO>(newEntity);
        }

        public RecipeDTO UpdateRecipeById(RecipeDTO dto) {
            var id = dto.Id;
            var listIngredients = dto.RecipeDetails ?? new HashSet<RecipeDetailDTO>();
            var listTools = dto.RecipeTools ?? new HashSet<RecipeToolDTO>();
            var listSteps = dto.ProcessingSteps ?? new HashSet<ProcessingStepDTO>();

            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing recipe id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"id"});
            }

            if (CheckInvalidListIngredient(listIngredients)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: List ingredient contains invalid obj, item not correct format exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowItemNotCorrectFormat(new[] {"ingredientId"});
            }

            if (CheckInvalidListStep(listSteps)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: List step contains invalid obj, item not correct format exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowItemNotCorrectFormat(new[] {"stepNumber"});
            }

            if (CheckInvalidListTool(listTools)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: List tool contains invalid obj, item not correct format exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowItemNotCorrectFormat(new[] {"toolId"});
            }

            var brandId = GetCurrentAccountBrandId();

            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            var query = _recipeRepository.QueryHelper();
            IncludeInRecipe(query);

            var existedEntity = query.GetOne(recipe => !recipe.IsDeleted && recipe.Id == id && recipe.Dish.Category.BrandId == brandId);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed recipe entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"id"});
            }

            _mapper.Map(dto, existedEntity);

            UpdateListTool(existedEntity.RecipeTools, dto.RecipeTools);
            UpdateListIngredient(existedEntity.RecipeDetails, dto.RecipeDetails);
            UpdateListStep(existedEntity.ProcessingSteps, dto.ProcessingSteps);

            existedEntity = _recipeRepository.Update(existedEntity);
            _logService.WriteLogUpdate(existedEntity);
            _recipeRepository.SaveChanges();

            return _mapper.Map<RecipeDTO>(existedEntity);
        }

        public RecipeDTO UpdateRecipeIsUsingById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing recipe id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"id"});
            }
            
            var brandId = GetCurrentAccountBrandId();

            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            var query = _recipeRepository.QueryHelper();
            IncludeInRecipe(query);    
            
            var existedEntity = query.GetOne(recipe =>
                !recipe.IsDeleted && recipe.Id == id && recipe.Dish.Category.BrandId == brandId);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed recipe entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"id"});
            }
            
            query = _recipeRepository.QueryHelper();
            IncludeInRecipe(query);
            
            var currentUsingRecipe = query.Filter(recipe =>
                !recipe.IsDeleted && recipe.IsUsing && recipe.Dish.Category.BrandId == brandId && recipe.DishId == existedEntity.DishId && recipe.Id != id)
                .GetAll().ToList();

            if (currentUsingRecipe.Count > 0) {
                currentUsingRecipe.ForEach(recipe => recipe.IsUsing = false);

                _recipeRepository.UpdateRange(currentUsingRecipe.ToArray());
                currentUsingRecipe.ForEach(recipe => _logService.WriteLogUpdate(recipe));
            }

            existedEntity.IsUsing = true;
            existedEntity = _recipeRepository.Update(existedEntity);
            _logService.WriteLogUpdate(existedEntity);
            _recipeRepository.SaveChanges();
            
            return _mapper.Map<RecipeDTO>(existedEntity);
        }

        public bool DeleteRecipeById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing recipe id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"id"});
            }

            var brandId = GetCurrentAccountBrandId();

            if (brandId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account brand id, unauthorized exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"brandId"});
            }

            var query = _recipeRepository.QueryHelper();
            IncludeInRecipe(query);

            var existedEntity = query.GetOne(recipe =>
                !recipe.IsDeleted && recipe.Id == id && recipe.Dish.Category.BrandId == brandId);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed recipe entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {"id"});
            }

            existedEntity.IsDeleted = true;
            DeleteRecipeChild(existedEntity);

            _recipeRepository.Update(existedEntity);
            _logService.WriteLogDelete(existedEntity);
            _recipeRepository.SaveChanges();

            return true;
        }
        
        public bool DeleteRecipesByDishId(long dishId) {
            if (dishId == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing dish id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"dishId"});
            }

            var query = _recipeRepository.QueryHelper();
            IncludeInRecipe(query);    
                
            var existedEntities = query.Filter(recipe => !recipe.IsDeleted && recipe.DishId == dishId)
                .GetAll().ToList();

            existedEntities.ForEach(recipe => {
                recipe.IsDeleted = true;
                DeleteRecipeChild(recipe);
                _logService.WriteLogDelete(recipe);
            });

            _recipeRepository.UpdateRange(existedEntities.ToArray());

            return true;
        }

        private void DeleteRecipeChild(Recipe recipe) {
            foreach (var rd in recipe.RecipeDetails) {
                rd.IsDeleted = true;
            }

            foreach (var rt in recipe.RecipeTools) {
                rt.IsDeleted = true;
            }

            foreach (var ps in recipe.ProcessingSteps) {
                ps.IsDeleted = true;
            }

            foreach (var qa in recipe.Qas) {
                qa.IsDeleted = true;
                qa.QaChild.IsDeleted = true;
            }
        }

        private void UpdateListIngredient(ICollection<RecipeDetail> source, HashSet<RecipeDetailDTO> updateList) {
            var updateListEntity = _mapper.Map<HashSet<RecipeDetail>>(updateList);
            
            foreach (var item in source) {
                if (updateListEntity.Contains(item)) {
                    _mapper.Map(updateList.FirstOrDefault(element => element.IngredientId == item.IngredientId), item);
                } else {
                    item.IsDeleted = true;
                }
            }
            
            // foreach (var item in source.Where(item => !updateListEntity.Contains(item))) {
            //     item.IsDeleted = true;
            // }
            //
            // foreach (var item in source.Where(updateListEntity.Contains)) {
            //     _mapper.Map(updateList.FirstOrDefault(element => element.IngredientId == item.IngredientId), item);
            // }

            foreach (var item in updateListEntity) {
                source.Add(item);
            }
        }

        private void UpdateListTool(ICollection<RecipeTool> source, HashSet<RecipeToolDTO> updateList) {
            var updateListEntity = _mapper.Map<HashSet<RecipeTool>>(updateList);
            
            foreach (var item in source) {
                if (updateListEntity.Contains(item)) {
                    _mapper.Map(updateList.FirstOrDefault(element => element.ToolId == item.ToolId), item);
                } else {
                    item.IsDeleted = true;
                }
            }
            
            // foreach (var item in source.Where(item => !updateListEntity.Contains(item))) {
            //     item.IsDeleted = true;
            // }
            //
            // foreach (var item in source.Where(updateListEntity.Contains)) {
            //     _mapper.Map(updateList.FirstOrDefault(element => element.ToolId == item.ToolId), item);
            // }

            foreach (var item in updateListEntity) {
                source.Add(item);
            }
        }

        private void UpdateListStep(ICollection<ProcessingStep> source, HashSet<ProcessingStepDTO> updateList) {
            var updateListEntity = _mapper.Map<HashSet<ProcessingStep>>(updateList);
            
            foreach (var item in source) {
                if (updateListEntity.Contains(item)) {
                    _mapper.Map(updateList.FirstOrDefault(element => element.StepNumber == item.StepNumber), item);
                } else {
                    item.IsDeleted = true;
                }
            }
            
            // foreach (var item in source.Where(item => !updateListEntity.Contains(item))) {
            //     item.IsDeleted = true;
            // }
            //
            // foreach (var item in source.Where(updateListEntity.Contains)) {
            //     _mapper.Map(updateList.FirstOrDefault(element => element.StepNumber == item.StepNumber), item);
            // }

            foreach (var item in updateListEntity) {
                source.Add(item);
            }
        }

        private bool CheckInvalidListIngredient(HashSet<RecipeDetailDTO> list) {
            return list.FirstOrDefault(rd => rd.IngredientId == 0) != null;
        }

        private bool CheckInvalidListTool(HashSet<RecipeToolDTO> list) {
            return list.FirstOrDefault(rt => rt.ToolId == 0) != null;
        }

        private bool CheckInvalidListStep(HashSet<ProcessingStepDTO> list) {
            return list.FirstOrDefault(ps => ps.StepNumber == 0) != null;
        }

        private void IncludeInRecipe(IFluentRepository<Recipe> query) {
            query.Include(recipe => recipe.Dish)
                .Include(source => source
                .Include(recipe => recipe.Qas.Where(qa => !qa.IsDeleted))
                .ThenInclude(qa => qa.QaChild)
                .Include(recipe => recipe.RecipeTools.Where(rt => !rt.IsDeleted))
                .ThenInclude(rt => rt.Tool)
                .Include(recipe => recipe.RecipeDetails.Where(rd => !rd.IsDeleted))
                .ThenInclude(rd => rd.Ingredient)
                .Include(recipe => recipe.ProcessingSteps.Where(ps => !ps.IsDeleted)));
        }
    }
}