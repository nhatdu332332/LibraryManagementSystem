using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagementSystem.Services.DTOs;

namespace LibraryManagementSystem.Services.Interfaces
{
	public interface IEmployeeAccountService
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
		/// Cập nhật thông tin nhân viên (Employee tự update profile hoặc Admin update)
		/// </summary>
		Task UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto dto);

		/// <summary>
		/// Xóa hoặc deactivate nhân viên (chỉ Admin)
		/// </summary>
		Task DeleteEmployeeAsync(int employeeId);

		/// <summary>
		/// Thay đổi role của nhân viên (chỉ Admin)
		/// </summary>
		Task ChangeEmployeeRoleAsync(int employeeId, int newRoleId);

		/// <summary>
		/// Admin tạo tài khoản Employee (Staff/Librarian/Admin)
		/// </summary>
		Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);

		/// <summary>
		/// Đổi mật khẩu của Employee (yêu cầu mật khẩu hiện tại)
		/// </summary>
		Task ChangeEmployeePasswordAsync(int employeeId, string currentPassword, string newPassword);

		/// <summary>
		/// Reset mật khẩu Employee (Admin)
		/// </summary>
		Task ResetEmployeePasswordAsync(int employeeId, string newPassword);
	}
}
