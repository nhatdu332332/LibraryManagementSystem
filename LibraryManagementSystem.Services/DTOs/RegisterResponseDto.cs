using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace LibraryManagementSystem.Services.DTOs
{
	public class RegisterResponseDto
	{
		/// <summary>
		/// ID của Reader vừa tạo
		/// </summary>
		public int ReaderId { get; set; }

		/// <summary>
		/// Mã thẻ bạn đọc (CardNumber) tự sinh
		/// </summary>
		public string CardNumber { get; set; } = string.Empty;

		/// <summary>
		/// Họ tên đầy đủ
		/// </summary>
		public string FullName { get; set; } = string.Empty;

		/// <summary>
		/// Email đã đăng ký
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Ngày hết hạn thẻ (mặc định 2 năm từ RegisterDate)
		/// </summary>
		public DateTime ExpiredDate { get; set; }

		/// <summary>
		/// Thông báo thành công
		/// </summary>
		public string Message { get; set; } = "Đăng ký tài khoản Reader thành công!";
	}
}
