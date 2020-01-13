using Microsoft.EntityFrameworkCore;
using RecipeManager.DataInterfaces;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Data
{
	public class ShoppingRepository : GenericRepository, IShoppingRepository
	{
		private readonly DataContext _context;

		public ShoppingRepository(DataContext context) : base(context)
		{
			_context = context;
		}

		public async Task<List<Shopping>> GetAllShoppingLists(int userId)
		{
			return await _context.Shoppings.Where(s => s.UserId == userId).Include(p => p.Products).ToListAsync();
		}

		public async Task<Shopping> GetShoppingList(int id)
		{
			return await _context.Shoppings.Include(p => p.Products).FirstOrDefaultAsync(s => s.Id == id);
		}

	}
}
