using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace LibraryManagementSystem.Services.DTOs
{
	public class ReaderDto
	{
		/// <summary>
		/// ID độc giả
		/// </summary>
		public int ReaderId { get; set; }

		/// <summary>
		/// Mã thẻ bạn đọc (unique)
		/// </summary>
		public string CardNumber { get; set; } = string.Empty;

		/// <summary>
		/// Email (unique, dùng để login)
		/// </summary>
		public string Email { get; set; } = string.Empty;

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
		/// Ngày đăng ký
		/// </summary>
		public DateTime RegisterDate { get; set; }

		/// <summary>
		/// Ngày hết hạn thẻ
		/// </summary>
		public DateTime ExpiredDate { get; set; }

		/// <summary>
		/// Trạng thái (Active, Expired, Suspended)
		/// </summary>
		public string ReaderStatus { get; set; } = string.Empty;

		/// <summary>
		/// Số lượng sách đang mượn (tính từ BorrowTransaction)
		/// </summary>
		public int BorrowedBooksCount { get; set; }

		/// <summary>
		/// Số lượng request đang pending (tính từ BorrowRequest)
		/// </summary>
		public int PendingRequestsCount { get; set; }
	}
}
