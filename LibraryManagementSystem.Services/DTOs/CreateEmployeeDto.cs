using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.DTOs
{
	public class CreateEmployeeDto
	{
		/// <summary>
		/// Email (unique, dùng để login)
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Mật khẩu plaintext (service sẽ hash trước khi lưu)
		/// </summary>
		public string Password { get; set; } = string.Empty;

		/// <summary>
		/// Họ tên đầy đủ
		/// </summary>
		public string FullName { get; set; } = string.Empty;

		/// <summary>
		/// RoleId (từ bảng Role: Administrator, Staff, Librarian)
		/// </summary>
		public int RoleId { get; set; }

		/// <summary>
		/// Ngày vào làm (mặc định Today nếu null)
		/// </summary>
		public DateTime? HireDate { get; set; }

		/// <summary>
		/// Trạng thái (Active/Inactive – mặc định Active)
		/// </summary>
		public string? Status { get; set; }
	}
}
