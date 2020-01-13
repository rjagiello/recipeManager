using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.DataInterfaces
{
	public interface IShoppingRepository : IGenericRepository
	{
		Task<List<Shopping>> GetAllShoppingLists(int userId);
		Task<Shopping> GetShoppingList(int id);
	}
}
