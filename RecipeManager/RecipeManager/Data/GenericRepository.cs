using RecipeManager.DataInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Data
{
    public class GenericRepository : IGenericRepository
    {
        private readonly DataContext _context;

        public GenericRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

		public void DeleteRange<T>(ICollection<T> entities) where T : class
		{
			_context.RemoveRange(entities);
		}	

		public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

		public virtual Task<List<string>> SearchObjects(int userId, string term)
		{
			throw new NotImplementedException();
		}

		public virtual Task<object> GetSearchResult(int userId, string name)
		{
			throw new NotImplementedException();
		}
	}
}
