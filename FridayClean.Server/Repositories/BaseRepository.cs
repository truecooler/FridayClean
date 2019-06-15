using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FridayClean.Server.Repositories
{
	public class BaseRepository<T, C> : IRepository<T>
	where T : class
	where C : DbContext
	{
		protected C _dataContext;
		private DbSet<T> _dbset;

		public BaseRepository(C context)
		{
			this._dataContext = context;
			this._dbset = context.Set<T>();

			_dataContext.Database.EnsureCreated();
		}


		public virtual List<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
		{
			IQueryable<T> query = _dbset;

			if (filter != null)
			{
				query = query.Where(filter);
			}

			foreach (var includeProperty in includeProperties.Split
				(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProperty);
			}

			if (orderBy != null)
			{
				return orderBy(query).ToList();
			}
			else
			{
				return query.ToList();
			}
		}

		//public PagedList<T> GetWithPaging(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int page = 1,
		//	int size = 50)
		//{
		//	IQueryable<T> query = _dbset;

		//	if (filter != null)
		//	{
		//		query = query.Where(filter);
		//	}

		//	foreach (var includeProperty in includeProperties.Split
		//		(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
		//	{
		//		query = query.Include(includeProperty);
		//	}

		//	if (orderBy != null)
		//	{
		//		return new PagedList<T>(orderBy(query), page, size);
		//	}
		//	else
		//	{
		//		throw new Exception("Get With Paging query must be sorted");
		//	}
		//}

		public virtual void Add(T entity)
		{
			_dbset.Add(entity);
		}

		public virtual void Update(T entity)
		{
			_dbset.Attach(entity);

			_dataContext.Entry(entity).State = EntityState.Modified;
		}

		public virtual void Delete(T entity)
		{
			_dbset.Remove(entity);
		}

		public virtual void Delete(Expression<Func<T, bool>> where)
		{
			IEnumerable<T> objects = _dbset.Where<T>(where).AsEnumerable();
			foreach (T obj in objects)
				_dbset.Remove(obj);
		}

		public virtual T GetById(int id)
		{
			return _dbset.Find(id);
		}

		public virtual IEnumerable<T> GetAll()
		{
			return _dbset.ToList();
		}

		public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
		{
			return _dbset.Where(where).ToList();
		}

		public T Get(Expression<Func<T, bool>> where)
		{
			return _dbset.Where(where).FirstOrDefault<T>();
		}

		public int Count(Expression<Func<T, bool>> where = null)
		{
			return _dbset.Count(where);
		}

		public bool IsExist(Expression<Func<T, bool>> where = null)
		{
			return _dbset.FirstOrDefault(where) != null ? true : false;
		}

		public void Save()
		{
			
				_dataContext.SaveChanges();
			
//#if DEBUG
//			catch (DbEntityValidationException e)
//			{
//				foreach (var eve in e.EntityValidationErrors)
//				{
//					Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
//						eve.Entry.Entity.GetType().Name, eve.Entry.State);
//					foreach (var ve in eve.ValidationErrors)
//					{
//						Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
//							ve.PropertyName, ve.ErrorMessage);
//					}
//				}
//				throw;
//			}
//#endif

		}
	}
}
