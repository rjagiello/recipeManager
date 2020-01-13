using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.DataInterfaces
{
    public interface IGenericRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
		void DeleteRange<T>(ICollection<T> entities) where T : class;

		Task<bool> SaveAll();
		Task<List<string>> SearchObjects(int userId, string term);
		Task<Object> GetSearchResult(int userId, string name);
	}
}
