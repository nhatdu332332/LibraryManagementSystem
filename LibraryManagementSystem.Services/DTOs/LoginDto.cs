using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.DTOs
{
	public class LoginDto
	{
		/// <summary>
		/// Email của Employee hoặc Reader
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Mật khẩu (plaintext, sẽ hash trong service)
		/// </summary>
		public string Password { get; set; } = string.Empty;

		/// <summary>
		/// (Optional) Loại tài khoản để phân biệt login Employee hay Reader
		/// Nếu không gửi, service sẽ tự kiểm tra cả hai bảng
		/// </summary>
		public string? AccountType { get; set; } // "Employee" hoặc "Reader"
	}
}
