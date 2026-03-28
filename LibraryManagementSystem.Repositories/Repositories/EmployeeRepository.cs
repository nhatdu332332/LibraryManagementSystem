using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories
{
	public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
	{
		public EmployeeRepository(LibraryDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Tìm Employee theo email (dùng khi login)
		/// </summary>
		public async Task<Employee?> GetByEmailAsync(string email)
		{
			return await _dbSet
				.Include(e => e.Role)  // ← THÊM DÒNG NÀY để load Role
				.FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());
		}

		/// <summary>
		/// Lấy tất cả Employee theo RoleId (ví dụ lấy tất cả Librarian)
		/// </summary>
		public async Task<IEnumerable<Employee>> GetByRoleIdAsync(int roleId)
		{
			return await _dbSet
				.Where(e => e.RoleId == roleId)
				.ToListAsync();
		}

		/// <summary>
		/// Lấy Employee kèm Role (eager loading)
		/// </summary>
		public async Task<Employee?> GetWithRoleAsync(int employeeId)
		{
			return await _dbSet
				.Include(e => e.Role)
				.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
		}
	}
}
