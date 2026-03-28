using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.DTOs
{
	public class CreateRoleDto
	{
		/// <summary>
		/// Tên role (ví dụ: "Guest", "Moderator" – phải unique)
		/// </summary>
		public string RoleName { get; set; } = string.Empty;

		/// <summary>
		/// Mô tả ngắn gọn về quyền hạn của role (optional)
		/// </summary>
		public string? Description { get; set; }
	}
}