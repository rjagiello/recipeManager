using RecipeManager.Helpers;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.DataInterfaces
{
	public interface IFridgeRepository : IGenericRepository
	{
		PagedList<Product> GetFridgeContent(FridgeParam fridgeParams, IQueryable<Product> fridge);
		Task<Fridge> GetFridge(int userId);
		Task<Product> GetProduct(int id);
	}
}
