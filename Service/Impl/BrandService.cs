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
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;
using RecipeManagementBE.Util;

namespace RecipeManagementBE.Service.Impl {
    public class BrandService : BaseService, IBrandService {
        private readonly IBrandRepository _brandRepository;

        private readonly ICategoryService _categoryService;

        private readonly IEmployeeService _employeeService;
        
        private readonly ILogService<Brand> _logService;

        private readonly ILogger<BrandService> _logger;

        private readonly IMapper _mapper;

        private const string BRAND_PK = "id";

        private const string BRAND_NAME = "name";

        public BrandService(IBrandRepository brandRepository, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, ILogger<BrandService> logger,
            ILogService<Brand> logService, ICategoryService categoryService,
            IEmployeeService employeeService, IAccountRepository accountRepository) : base(httpContextAccessor, accountRepository) {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _logger = logger;
            _logService = logService;
            _categoryService = categoryService;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Get all brand available in system
        /// </summary>
        /// <returns>List of brand</returns>
        public List<BrandDTO> GetAllBrand() {
            var brandEntities = _brandRepository.QueryHelper()
                .Filter(brand => !brand.IsDeleted)
                .OrderBy(brands => brands.OrderByDescending(brand => brand.Id))
                .GetAll().ToList();

            return _mapper.Map<List<BrandDTO>>(brandEntities);
        }

        /// <summary>
        /// Get brand information by id
        /// </summary>
        /// <param name="id">Id of brand need to get</param>
        /// <returns>Brand information matched id</returns>
        public BrandDTO GetBrandById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing brand id, missing required field exception throw",
                    DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {BRAND_PK});
            }

            var existedEntity = _brandRepository.QueryHelper()
                .GetOne(brand => brand.Id == id && !brand.IsDeleted);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed brand entity match filter, entity not found exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {BRAND_PK});
            }

            return _mapper.Map<BrandDTO>(existedEntity);
        }

        /// <summary>
        /// Add new brand to system
        /// </summary>
        /// <param name="dto.Name">Name of brand required and unique</param>
        /// <param name="dto.Logo">Image logo of brand (url to image on firebase)</param>
        /// <param name="dto.Phone">Brand phone required</param>
        /// <remarks>
        /// 1. Set id to 0 ignore id come from client
        /// 2. Check name, phone is empty or not -> if empty throw missing field
        /// 3. Check exist with name
        /// 4. Map brand info to new entity
        /// 5. Add and save to repo
        /// </remarks>
        /// <returns>Brand added</returns>
        public BrandDTO AddNewBrand(BrandDTO dto) {
            dto.Id = 0;

            var name = dto.Name ?? string.Empty;
            var phone = dto.Phone ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing brand name, missing required field exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {BRAND_NAME});
            }

            if (string.IsNullOrWhiteSpace(phone)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing brand phone, missing required field exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"phone"});
            }

            if (_brandRepository.Exists(brand => !brand.IsDeleted && brand.Name.Equals(name))) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Brand with name had already existed while trying create new brand, existed entity exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {BRAND_NAME});
            }

            var newEntity = _mapper.Map<Brand>(dto);

            newEntity = _brandRepository.Add(newEntity);
            _logService.WriteLogCreate(newEntity);
            _brandRepository.SaveChanges();

            return _mapper.Map<BrandDTO>(newEntity);
        }

        /// <summary>
        /// Update brand
        /// </summary>
        /// <param name="dto.Name">Name of brand required and unique</param>
        /// <param name="dto.Logo">Image logo of brand (url to image on firebase)</param>
        /// <param name="dto.Phone">Brand phone required</param>
        /// <remarks>
        /// 1. Check id, name, phone is empty or not -> if empty throw missing field
        /// 2. Get existed entity by id
        /// 3. Check existed with name
        /// 4. Map brand info to new entity
        /// 5. Add and save to repo
        /// </remarks>
        /// <returns>Brand updated</returns>
        public BrandDTO UpdateBrandById(BrandDTO dto) {
            var id = dto.Id;
            var name = dto.Name ?? string.Empty;
            var phone = dto.Phone ?? string.Empty;

            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing brand id, missing required field exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {BRAND_PK});
            }

            if (string.IsNullOrWhiteSpace(name)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing brand name, missing required field exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {BRAND_NAME});
            }

            if (string.IsNullOrWhiteSpace(phone)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing brand phone, missing required field exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"phone"});
            }

            var existedEntity = _brandRepository.QueryHelper()
                .GetOne(brand => brand.Id == id && !brand.IsDeleted);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed brand entity match filter, entity not found exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {BRAND_PK});
            }

            if (_brandRepository.Exists(brand => !brand.IsDeleted && brand.Name.Equals(name) && brand.Id != id)) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Brand with name had already existed while trying update brand, existed entity exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowEntityExisted(new[] {BRAND_NAME});
            }

            _mapper.Map(dto, existedEntity);

            existedEntity = _brandRepository.Update(existedEntity);
            _logService.WriteLogUpdate(existedEntity);
            _brandRepository.SaveChanges();

            return _mapper.Map<BrandDTO>(existedEntity);
        }

        /// <summary>
        /// Delete brand by id
        /// </summary>
        /// <param name="id">Brand Id need to delete</param>
        /// <remarks>
        /// 1. Check id is empty or not -> if empty throw missing field
        /// 2. Get existed entity by id
        /// 3. Update flag and save to repo
        /// </remarks>
        /// <returns>Brand updated</returns>
        public bool DeleteBrandById(long id) {
            if (id == 0) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing brand id, missing required field exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {BRAND_PK});
            }

            var existedEntity = _brandRepository.QueryHelper()
                .GetOne(brand => brand.Id == id && !brand.IsDeleted);

            if (existedEntity == null) {
                _logger.LogError(
                    "[{Time}] [{ApplicationName}]: Can't found existed brand entity match filter, entity not found exception throw",
                    DateTime.Now, Constants.APPLICATION_NAME);
                ThrowEntityNotFound(new[] {BRAND_PK});
            }

            existedEntity.IsDeleted = true;

            _brandRepository.Update(existedEntity);
            _logService.WriteLogDelete(existedEntity);
            _categoryService.DeleteCategoriesByBrandId(id);
            _employeeService.DeleteEmployeesByBrandId(id);
            _brandRepository.SaveChanges();

            return true;
        }

        /// <summary>
        /// Get page of brand
        /// </summary>
        /// <param name="pageableModel"></param>
        /// <param name="pageableModel.Name">Name to search brand</param>
        /// <param name="pageableModel.Address">Address to search brand</param>
        /// <param name="pageableModel.SortField">Field to sort page</param>
        /// <param name="pageableModel.SortDirection">if direction < 0 -> desc, >= 0 -> asc</param>
        /// <param name="pageableModel.PageNumber">Page number need to get</param>
        /// <param name="pageableModel.PageSize">Size of 1 page</param>
        /// <remarks>
        /// 1. Name and Address is ignore case and contain in entity (like)
        /// 2. Sort direction default is asc
        /// 3. Sort field default is primary key
        /// </remarks>
        /// <returns>Page content list brand</returns>
        public PageResponse<BrandDTO> GetPageBrand(PageableModel<SearchBrandDTO> pageableModel) {
            var name = pageableModel.SearchModel?.Name ?? string.Empty;
            var address = pageableModel.SearchModel?.Address ?? string.Empty;

            var sortField = pageableModel.SortField ?? BRAND_PK;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);

            var brandEntities = _brandRepository.QueryHelper()
                .Filter(brand => !brand.IsDeleted &&
                                 brand.Name.ToLower().Contains(name.ToLower()) &&
                                 brand.Address.ToLower().Contains(address.ToLower()))
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));

            return _mapper.Map<IPage<Brand>, PageResponse<BrandDTO>>(brandEntities).GetTotalPage();
        }
    }
}