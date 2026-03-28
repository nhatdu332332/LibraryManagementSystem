using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.DTOs
{
	public class EmployeeDto
	{
		/// <summary>
		/// ID nhân viên
		/// </summary>
		public int EmployeeId { get; set; }

		/// <summary>
		/// Email (unique, dùng để login)
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Họ tên đầy đủ
		/// </summary>
		public string FullName { get; set; } = string.Empty;

		/// <summary>
		/// Tên role (Administrator, Staff, Librarian)
		/// </summary>
		public string RoleName { get; set; } = string.Empty;

		/// <summary>
		/// Ngày vào làm
		/// </summary>
		public DateTime HireDate { get; set; }

		/// <summary>
		/// Trạng thái (Active, Inactive)
		/// </summary>
		public string Status { get; set; } = string.Empty;

		/// <summary>
		/// Số lượng giao dịch mượn đã xử lý (tùy chọn, tính từ BorrowTransaction)
		/// </summary>
		public int ProcessedTransactionsCount { get; set; }
	}
}