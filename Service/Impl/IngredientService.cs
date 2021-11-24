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
using RecipeManagementBE.Response;
using RecipeManagementBE.Util;

namespace RecipeManagementBE.Service.Impl {
    public class IngredientService : BaseService, IIngredientService {
        private readonly IIngredientRepository _ingredientRepository;

        private readonly IRecipeDetailRepository _recipeDetailRepository;

        private readonly ILogService<Ingredient> _logService;

        private readonly ILogger<IngredientService> _logger;

        private readonly IMapper _mapper;
        
        private const string INGREDIENT_PK = "id";

        private const string INGREDIENT_NAME = "name";

        public IngredientService(IIngredientRepository ingredientRepository, IHttpContextAccessor httpContextAccessor,
            ILogService<Ingredient> logService, ILogger<IngredientService> logger, IMapper mapper,
            IRecipeDetailRepository recipeDetailRepository, IAccountRepository accountRepository) :
            base(httpContextAccessor, accountRepository) {
            _ingredientRepository = ingredientRepository;
            _logService = logService;
            _logger = logger;
            _mapper = mapper;
            _recipeDetailRepository = recipeDetailRepository;
        }

        public List<IngredientDTO> GetAllIngredient(string name) {
            name ??= string.Empty;
            
            var toolEntities = _ingredientRepository.QueryHelper()
                .Filter(ingredient => !ingredient.IsDeleted && ingredient.Name.ToLower().Contains(name.ToLower()))
                .OrderBy(ingredients => ingredients.OrderByDescending(ingredient => ingredient.Id))
                .GetAll().ToList();

            return _mapper.Map<List<IngredientDTO>>(toolEntities);
        }

        public IngredientDTO GetIngredientById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing ingredient id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {INGREDIENT_PK});
            }

            var existedEntity = _ingredientRepository.QueryHelper()
                .GetOne(ingredient => ingredient.Id == id && !ingredient.IsDeleted);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed ingredient entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {INGREDIENT_PK});
            }

            return _mapper.Map<IngredientDTO>(existedEntity);
        }

        public IngredientDTO AddNewIngredient(IngredientDTO dto) {
            dto.Id = 0;

            var name = dto.Name ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing ingredient name, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {INGREDIENT_NAME});
            }

            if (_ingredientRepository.Exists(ingredient => !ingredient.IsDeleted && ingredient.Name.Equals(name))) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Ingredient with name had already existed while trying create new ingredient, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {INGREDIENT_NAME});
            }

            var newEntity = _mapper.Map<Ingredient>(dto);

            newEntity = _ingredientRepository.Add(newEntity);
            _logService.WriteLogCreate(newEntity);
            _ingredientRepository.SaveChanges();

            return _mapper.Map<IngredientDTO>(newEntity);
        }

        public bool DeleteIngredientById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing ingredient id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {INGREDIENT_PK});
            }

            var existedEntity = _ingredientRepository.QueryHelper()
                .GetOne(ingredient => ingredient.Id == id && !ingredient.IsDeleted);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed ingredient entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {INGREDIENT_PK});
            }

            var detailEntity = _recipeDetailRepository.QueryHelper()
                .Include(detail => detail.Recipe.Dish)
                .GetOne(detail => detail.IngredientId == id && !detail.IsDeleted);
            
            if (detailEntity != null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't delete existed ingredient entity in used, item in use exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowItemInUse(new []{"id", detailEntity.Recipe.DishId.ToString(), detailEntity.Recipe.Dish.ManagerId.ToString()});
            }

            existedEntity.IsDeleted = true;

            _ingredientRepository.Update(existedEntity);
            _logService.WriteLogDelete(existedEntity);
            _ingredientRepository.SaveChanges();

            return true;
        }

        public PageResponse<IngredientDTO> GetPageIngredient(PageableModel<string> pageableModel) {
            var name = pageableModel.SearchModel ?? string.Empty;

            var sortField = pageableModel.SortField ?? INGREDIENT_PK;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);

            var ingredientEntities = _ingredientRepository.QueryHelper()
                .Filter(ingredient => !ingredient.IsDeleted &&
                                      ingredient.Name.ToLower().Contains(name.ToLower()))
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));

            return _mapper.Map<IPage<Ingredient>, PageResponse<IngredientDTO>>(ingredientEntities).GetTotalPage();
        }
    }
}