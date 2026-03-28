using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories
{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
	{
		protected readonly LibraryDbContext _context;
		protected readonly DbSet<TEntity> _dbSet;

		public GenericRepository(LibraryDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_dbSet = _context.Set<TEntity>();
		}

		public virtual async Task<TEntity> GetByIdAsync(object id)
		{
			return await _dbSet.FindAsync(id);
		}

		public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
		{
			return await _dbSet
				.AsNoTracking()
				.ToListAsync();
		}

		public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
		{
			return await _dbSet
				.AsNoTracking()
				.Where(predicate)
				.ToListAsync();
		}

		public virtual async Task AddAsync(TEntity entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public virtual Task UpdateAsync(TEntity entity)
		{
			_dbSet.Update(entity);
			return Task.CompletedTask;
		}

		public virtual Task DeleteAsync(TEntity entity)
		{
			_dbSet.Remove(entity);
			return Task.CompletedTask;
		}

		public virtual async Task DeleteByIdAsync(object id)
		{
			var entity = await GetByIdAsync(id);
			if (entity != null)
			{
				_dbSet.Remove(entity);
			}
		}

		public virtual async Task<bool> ExistsAsync(object id)
		{
			var entity = await GetByIdAsync(id);
			return entity != null;
		}
	}
}
