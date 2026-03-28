using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagementSystem.Services.DTOs; // Sẽ tạo DTOs sau

namespace LibraryManagementSystem.Services.Interfaces
{
	public interface IEmployeeService
	{
		/// <summary>
		/// Lấy thông tin nhân viên theo ID
		/// </summary>
		Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId);

		/// <summary>
		/// Lấy tất cả nhân viên (Admin xem danh sách)
		/// </summary>
		Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();

		/// <summary>
		/// Cập nhật thông tin nhân viên (Admin/Staff/Librarian tự update profile)
		/// </summary>
		Task UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto dto, int currentUserId, string currentRoleName);

		/// <summary>
		/// Xóa hoặc deactivate nhân viên (chỉ Admin)
		/// </summary>
		Task DeleteEmployeeAsync(int employeeId, int currentUserId, string currentRoleName);

		/// <summary>
		/// Thay đổi role của nhân viên (chỉ Admin)
		/// </summary>
		Task ChangeEmployeeRoleAsync(int employeeId, int newRoleId, int currentUserId, string currentRoleName);

		/// <summary>
		/// Tìm nhân viên theo email (dùng khi login Employee)
		/// </summary>
		Task<EmployeeDto?> FindByEmailAsync(string email);

		/// <summary>
		/// Lấy nhân viên theo role (ví dụ tất cả Librarian)
		/// </summary>
		Task<IEnumerable<EmployeeDto>> GetByRoleNameAsync(string roleName);
	}
}
