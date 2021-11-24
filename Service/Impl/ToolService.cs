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
    public class ToolService : BaseService, IToolService {
        private readonly IToolRepository _toolRepository;

        private readonly IRecipeToolRepository _recipeToolRepository;

        private readonly ILogService<Tool> _logService;

        private readonly ILogger<ToolService> _logger;

        private readonly IMapper _mapper;

        private const string TOOL_PK = "id";

        private const string TOOL_NAME = "name";

        public ToolService(IHttpContextAccessor httpContextAccessor, IToolRepository toolRepository,
            ILogService<Tool> logService, ILogger<ToolService> logger, IMapper mapper,
            IRecipeToolRepository recipeToolRepository, IAccountRepository accountRepository) : base(
            httpContextAccessor, accountRepository) {
            _toolRepository = toolRepository;
            _logService = logService;
            _logger = logger;
            _mapper = mapper;
            _recipeToolRepository = recipeToolRepository;
        }

        public List<ToolDTO> GetAllTool(string name) {
            name ??= string.Empty;

            var toolEntities = _toolRepository.QueryHelper()
                .Filter(tool => !tool.IsDeleted && tool.Name.ToLower().Contains(name.ToLower()))
                .OrderBy(tools => tools.OrderByDescending(tool => tool.Id))
                .GetAll().ToList();

            return _mapper.Map<List<ToolDTO>>(toolEntities);
        }

        public ToolDTO GetToolById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing tool id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {TOOL_PK});
            }

            var existedEntity = _toolRepository.QueryHelper()
                .GetOne(tool => tool.Id == id && !tool.IsDeleted);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed tool entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {TOOL_PK});
            }

            return _mapper.Map<ToolDTO>(existedEntity);
        }

        public ToolDTO AddNewTool(ToolDTO dto) {
            dto.Id = 0;

            var name = dto.Name ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing tool name, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {TOOL_NAME});
            }

            if (_toolRepository.Exists(tool => !tool.IsDeleted && tool.Name.Equals(name))) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Tool with name had already existed while trying create new tool, existed entity exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {TOOL_NAME});
            }

            var newEntity = _mapper.Map<Tool>(dto);

            newEntity = _toolRepository.Add(newEntity);
            _logService.WriteLogCreate(newEntity);
            _toolRepository.SaveChanges();

            return _mapper.Map<ToolDTO>(newEntity);
        }

        public bool DeleteToolById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing tool id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {TOOL_PK});
            }

            var existedEntity = _toolRepository.QueryHelper()
                .GetOne(tool => tool.Id == id && !tool.IsDeleted);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed tool entity match filter, entity not found exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {TOOL_PK});
            }

            var detailEntity = _recipeToolRepository.QueryHelper()
                .Include(detail => detail.Recipe.Dish)
                .GetOne(detail => detail.ToolId == id && !detail.IsDeleted);

            if (detailEntity != null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't delete existed tool entity in used, item in use exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowItemInUse(new[] {"id", detailEntity.Recipe.DishId.ToString(), detailEntity.Recipe.Dish.ManagerId.ToString()});
            }

            existedEntity.IsDeleted = true;

            _toolRepository.Update(existedEntity);
            _logService.WriteLogDelete(existedEntity);
            _toolRepository.SaveChanges();

            return true;
        }

        public PageResponse<ToolDTO> GetPageTool(PageableModel<string> pageableModel) {
            var name = pageableModel.SearchModel ?? string.Empty;

            var sortField = pageableModel.SortField ?? TOOL_PK;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);

            var toolEntities = _toolRepository.QueryHelper()
                .Filter(tool => !tool.IsDeleted &&
                                tool.Name.ToLower().Contains(name.ToLower()))
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));

            return _mapper.Map<IPage<Tool>, PageResponse<ToolDTO>>(toolEntities).GetTotalPage();
        }
    }
}