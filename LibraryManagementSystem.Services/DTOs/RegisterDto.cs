using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace LibraryManagementSystem.Services.DTOs
{
	public class RegisterDto
	{
		/// <summary>
		/// Email (unique, dùng để login)
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Mật khẩu (plaintext, service sẽ hash)
		/// </summary>
		public string Password { get; set; } = string.Empty;

		/// <summary>
		/// Họ tên đầy đủ
		/// </summary>
		public string FullName { get; set; } = string.Empty;

		/// <summary>
		/// Số điện thoại (optional)
		/// </summary>
		public string? PhoneNumber { get; set; }

		/// <summary>
		/// Địa chỉ (optional)
		/// </summary>
		public string? Address { get; set; }

		/// <summary>
		/// Ngày hết hạn thẻ (service tự set mặc định 1-2 năm từ RegisterDate)
		/// </summary>
		public DateTime? ExpiredDate { get; set; } // Nếu null, service tự tính
	}
}
