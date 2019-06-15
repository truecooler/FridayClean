using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FridayClean.Server.Repositories
{
	/// <summary>
	/// Defines interface for common data access functionality for entity.
	/// </summary>
	/// <typeparam name="T">Type of entity.</typeparam>
	public interface IRepository<T>
	{
		List<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
			string includeProperties = "");

		//PagedList<T> GetWithPaging(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
		//	string includeProperties = "", int page = 1, int size = 50);

		void Add(T entity);

		void Update(T entity);

		void Delete(T entity);

		void Delete(Expression<Func<T, bool>> where);

		T GetById(int id);

		IEnumerable<T> GetAll();

		IEnumerable<T> GetMany(Expression<Func<T, bool>> where);

		T Get(Expression<Func<T, bool>> where);

		int Count(Expression<Func<T, bool>> where = null);

		bool IsExist(Expression<Func<T, bool>> where = null);

		void Save();
	}
}
