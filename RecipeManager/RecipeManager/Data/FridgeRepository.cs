using Microsoft.EntityFrameworkCore;
using RecipeManager.DataInterfaces;
using RecipeManager.Helpers;
using RecipeManager.Models;
using RecipeManager.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Data
{
	public class FridgeRepository : GenericRepository, IFridgeRepository
	{
		private readonly DataContext _context;
		

		public FridgeRepository(DataContext context) : base(context)
		{
			_context = context;
		}

		public async Task<Product> GetProduct(int id)
		{
			return await _context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
		}

		public async Task<Fridge> GetFridge(int userId)
		{
			return await _context.Fridges.Where(f => f.UserId == userId).Include(p => p.Products).FirstOrDefaultAsync();
		}

		public PagedList<Product> GetFridgeContent(FridgeParam fridgeParams, IQueryable<Product> fridge)
		{
			return PagedList<Product>.CreateList(fridge.AsQueryable(), fridgeParams.PageNumber, fridgeParams.PageSize);
		}

		public override async Task<List<string>> SearchObjects(int userId, string term)
		{
			var fridge = await (from F in (from p in _context.Products
										   join f in _context.Fridges on p.FridgeId equals f.Id
										   where f.UserId == userId && p.Name.Contains(term)
										   select p.Name).Union(from p in _context.Products
																join r in _context.Recipes on p.RecipeId equals r.Id
																where r.UserId == userId && p.Name.Contains(term)
																select p.Name) select F).ToListAsync();

			return fridge;
		}

		public override async Task<object> GetSearchResult(int userId, string name)
		{
			return await _context.Products.Where(p => p.Fridge.UserId == userId && p.Name == name).FirstOrDefaultAsync();
		}
	}
}
