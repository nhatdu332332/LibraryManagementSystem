using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.DTOs
{
	public class RoleDto
	{
		/// <summary>
		/// ID role
		/// </summary>
		public int RoleId { get; set; }

		/// <summary>
		/// Tên role (Administrator, Staff, Librarian)
		/// </summary>
		public string RoleName { get; set; } = string.Empty;

		/// <summary>
		/// Mô tả ngắn gọn về quyền hạn của role
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Số lượng Employee đang dùng role này (tùy chọn, tính từ DB)
		/// </summary>
		public int EmployeeCount { get; set; }
	}
}
