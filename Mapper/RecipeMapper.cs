using AutoMapper;
using JHipsterNet.Core.Pagination;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Mapper {
    public class RecipeMapper : Profile {
        public RecipeMapper() {
            CreateMap<RecipeDTO, Recipe>()
                .ForMember(recipe => recipe.IsUsing, opt => opt.Ignore())
                .ForMember(recipe => recipe.RecipeTools, opt => opt.Ignore())
                .ForMember(recipe => recipe.RecipeDetails, opt => opt.Ignore())
                .ForMember(recipe => recipe.ProcessingSteps, opt => opt.Ignore())
                .ForMember(recipe => recipe.Qas, opt => opt.Ignore())
                .ForMember(recipe => recipe.Dish, opt => opt.Ignore())
                .ForMember(recipe => recipe.DishId, opt => opt.Ignore());
            CreateMap<Recipe, RecipeDTO>();

            CreateMap<RecipeDetailDTO, RecipeDetail>()
                .ForMember(rd => rd.RecipeId, opt => opt.Ignore())
                .ForMember(rd => rd.Ingredient, opt => opt.Ignore())
                .ForMember(rd => rd.Recipe, opt => opt.Ignore());
            CreateMap<RecipeDetail, RecipeDetailDTO>();

            CreateMap<RecipeToolDTO, RecipeTool>()
                .ForMember(rt => rt.RecipeId, opt => opt.Ignore())
                .ForMember(rt => rt.Tool, opt => opt.Ignore())
                .ForMember(rt => rt.Recipe, opt => opt.Ignore());
            CreateMap<RecipeTool, RecipeToolDTO>();
            
            CreateMap<ProcessingStepDTO, ProcessingStep>()
                .ForMember(ps => ps.RecipeId, opt => opt.Ignore())
                .ForMember(ps => ps.Recipe, opt => opt.Ignore());
            CreateMap<ProcessingStep, ProcessingStepDTO>();
            
            CreateMap<QaDTO, Qa>()
                .ForMember(qa => qa.IsReply, opt => opt.Ignore())
                .ForMember(qa => qa.Account, opt => opt.Ignore())
                .ForMember(qa => qa.QaParentId, opt => opt.Ignore())
                .ForMember(qa => qa.Recipe, opt => opt.Ignore())
                .ForMember(qa => qa.RecipeId, opt => opt.Ignore());
            CreateMap<Qa, QaDTO>()
                .ForMember(dto => dto.DishId, opt => opt.MapFrom(qa => qa.Recipe.DishId));

            // Pagination
            CreateMap<IPage<Recipe>, PageResponse<RecipeDTO>>();
            CreateMap<IPage<Qa>, PageResponse<QaDTO>>();
            
            CreateMap<CreateRecipeDTO, Recipe>();
            CreateMap<CreateQADTO, Qa>()
                .ForMember(qa => qa.QaParent, opt => opt.Ignore())
                .ForMember(qa => qa.QaParentId, opt => opt.Ignore())
                .ForMember(qa => qa.IsReply, opt => opt.Ignore());
        }
    }
}