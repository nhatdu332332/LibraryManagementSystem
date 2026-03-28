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
	public interface IRoleService
	{
		/// <summary>
		/// Lấy tất cả role trong hệ thống
		/// </summary>
		Task<IEnumerable<RoleDto>> GetAllRolesAsync();

		/// <summary>
		/// Lấy role theo ID
		/// </summary>
		Task<RoleDto> GetRoleByIdAsync(int roleId);

		/// <summary>
		/// Tạo role mới (chỉ Admin)
		/// </summary>
		Task<RoleDto> CreateRoleAsync(CreateRoleDto dto, int currentUserId, string currentRoleName);

		/// <summary>
		/// Cập nhật role (tên, mô tả – chỉ Admin)
		/// </summary>
		Task UpdateRoleAsync(int roleId, UpdateRoleDto dto, int currentUserId, string currentRoleName);

		/// <summary>
		/// Xóa role (chỉ Admin, kiểm tra không có Employee nào dùng role này)
		/// </summary>
		Task DeleteRoleAsync(int roleId, int currentUserId, string currentRoleName);

		/// <summary>
		/// Tìm role theo tên (không phân biệt hoa thường)
		/// </summary>
		Task<RoleDto?> GetByNameAsync(string roleName);
	}
}