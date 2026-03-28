using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories
{
	public class RoleRepository : GenericRepository<Role>, IRoleRepository
	{
		public RoleRepository(LibraryDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Tìm Role theo tên (không phân biệt hoa thường)
		/// Dùng khi kiểm tra quyền hoặc login
		/// </summary>
		public async Task<Role?> GetByNameAsync(string roleName)
		{
			return await _dbSet
				.FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower());
		}
	}
}

