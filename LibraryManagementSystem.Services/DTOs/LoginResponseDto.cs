using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace LibraryManagementSystem.Services.DTOs
{
	public class LoginResponseDto
	{
		public bool Success { get; set; } = false;  // Thêm cái này để fix CS1061
		public string Message { get; set; } = "Đăng nhập thất bại";

		public int UserId { get; set; }
		public string AccountType { get; set; } = "";  // "Reader" hoặc "Employee"
		public string FullName { get; set; } = "";
		public string RoleName { get; set; } = "";

		// Các trường optional khác nếu có (giữ nguyên nếu bạn đã có)
		public DateTime? ExpiredDate { get; set; }
		public string? Token { get; set; }
		public DateTime? TokenExpiration { get; set; }
	}
}