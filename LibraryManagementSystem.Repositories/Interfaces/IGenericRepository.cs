using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

using System;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories.Interfaces
{
	public interface IGenericRepository<TEntity> where TEntity : class
	{
		// Lấy 1 entity theo ID
		Task<TEntity> GetByIdAsync(object id);

		// Lấy tất cả
		Task<IEnumerable<TEntity>> GetAllAsync();

		// Tìm theo điều kiện (ví dụ: role.RoleName == "Administrator")
		Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

		// Thêm mới
		Task AddAsync(TEntity entity);

		// Cập nhật
		Task UpdateAsync(TEntity entity);

		// Xóa entity
		Task DeleteAsync(TEntity entity);

		// Xóa theo ID
		Task DeleteByIdAsync(object id);

		// Kiểm tra tồn tại
		Task<bool> ExistsAsync(object id);
	}
}
