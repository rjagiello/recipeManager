using AutoMapper;
using RecipeManager.Dtos;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Settings
{
    public class AutoMapperProfiles : Profile
    {
		public AutoMapperProfiles()
		{
			CreateMap<UserForRegisterDto, User>();
			CreateMap<User, UserForDetailedDto>()
				.ForMember(dest => dest.PhotoUrl, opt =>
				{
					opt.MapFrom(src => src.Photo.Url);
				});
			CreateMap<UserForUpdateDto, User>();
			CreateMap<Photo, PhotoForReturnDto>();
			CreateMap<PhotoForCreationDto, Photo>();
			CreateMap<Recipe, RecipeForReturnDto>()
				.ForMember(dest => dest.PhotoUrl, opt =>
				{
					opt.MapFrom(src => src.Photo.Url);
				});
			CreateMap<RecipeDto, Recipe>();
			CreateMap<Recipe, RecipeDto>()
				.ForMember(dest => dest.PhotoUrl, opt =>
				{
					opt.MapFrom(src => src.Photo.Url);
				});
			CreateMap<ProductDto, Product>();
			CreateMap<Product, ProductDto>();
			CreateMap<ProductEditDto, Product>();
			CreateMap<Product, ProductEditDto>();
			CreateMap<RecipeUpdateDto, Recipe>();
			CreateMap<Shopping, ShoppingListsDto>();
			CreateMap<Product, ProductShoppingDto>();
			CreateMap<Product, ProductShoppingFinishDto>();
			CreateMap<ProductShoppingDto, Product>();
			CreateMap<ProductShoppingFinishDto, Product>()
				.ForMember(dest => dest.Count, opt =>
				{
					opt.MapFrom(src => src.BoughtCount);
				});
			CreateMap<ShoppingListToAddDto, Shopping>();
		}
    }
}
