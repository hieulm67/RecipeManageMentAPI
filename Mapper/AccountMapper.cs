using AutoMapper;
using JHipsterNet.Core.Pagination;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Mapper {
    public class AccountMapper : Profile {
        public AccountMapper() {
            CreateMap<AccountDTO, Account>()
                .ForMember(account => account.Role, opt => opt.Ignore())
                .ForMember(account => account.Password, opt => opt.Ignore())
                .ForMember(account => account.UID, opt => opt.Ignore())
                .ForMember(account => account.RoleId, opt => opt.Ignore());
            CreateMap<Account, AccountDTO>();

            CreateMap<AdminDTO, Admin>()
                .ReverseMap();

            CreateMap<EmployeeDTO, Employee>()
                .ForMember(employee => employee.Brand, opt => opt.Ignore())
                // .ForMember(employee => employee.Dishes, opt => opt.Ignore())
                .ForMember(employee => employee.BrandId, opt => opt.Ignore());

            CreateMap<Employee, EmployeeDTO>();

            CreateMap<RoleDTO, Role>()
                .ReverseMap();

            // Pagination
            CreateMap<IPage<Admin>, PageResponse<AdminDTO>>();
            CreateMap<IPage<Employee>, PageResponse<EmployeeDTO>>();

            CreateMap<RegisterDTO, Account>();
        }
    }
}