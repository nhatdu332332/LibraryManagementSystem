using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Services.DTOs
{
	public class BorrowTransactionDto
	{
		/// <summary>
		/// ID giao dịch mượn
		/// </summary>
		public int BorrowId { get; set; }

		/// <summary>
		/// ID độc giả
		/// </summary>
		public int ReaderId { get; set; }

		/// <summary>
		/// Tên độc giả
		/// </summary>
		public string ReaderFullName { get; set; } = string.Empty;

		/// <summary>
		/// ID nhân viên xử lý (check-out)
		/// </summary>
		public int EmployeeId { get; set; }

		/// <summary>
		/// Tên nhân viên
		/// </summary>
		public string EmployeeFullName { get; set; } = string.Empty;

		/// <summary>
		/// ID request liên kết (nếu mượn từ request)
		/// </summary>
		public int? RequestId { get; set; }

		/// <summary>
		/// Ngày mượn
		/// </summary>
		public DateTime BorrowDate { get; set; }

		/// <summary>
		/// Trạng thái (Borrowed, PartiallyReturned, FullyReturned)
		/// </summary>
		public string Status { get; set; } = string.Empty;

		/// <summary>
		/// Danh sách chi tiết giao dịch (bản sao cụ thể, ngày đến hạn, ngày trả, phạt)
		/// </summary>
		public List<BorrowTransactionDetailDto> Details { get; set; } = new List<BorrowTransactionDetailDto>();
		// Thêm vào class BorrowTransactionDto
		public string DetailsString => Details?.Any() == true
		? string.Join(Environment.NewLine, Details.Select(d => $"{d.Title} (Copy #{d.CopyId})"))
		: "Không có chi tiết";

		public string DueDatesString => Details?.Any() == true
			? string.Join(Environment.NewLine, Details.Select(d => d.DueDate.ToString("dd/MM/yyyy")))
			: "N/A";

		public string CirculationStatusesString => Details?.Any() == true
			? string.Join(Environment.NewLine, Details.Select(d => d.CirculationStatus ?? "Unknown"))
			: "N/A";
		public string ItemStatusesString => Details?.Any() == true
	? string.Join(Environment.NewLine, Details.Select(d => d.ItemStatus ?? "Unknown"))
	: "N/A";
	}

	/// <summary>
	/// Chi tiết từng dòng trong transaction
	/// </summary>
	public class BorrowTransactionDetailDto
	{
		public int BorrowDetailId { get; set; }
		public int CopyId { get; set; }
		public string Title { get; set; } = string.Empty;
		public DateTime DueDate { get; set; }
		public DateTime? ReturnDate { get; set; }
		public string ItemStatus { get; set; } = string.Empty;
		public decimal FineAmount { get; set; }
		public string? ConditionNote { get; set; }
		public string? CirculationStatus { get; set; }     // ← thêm
		public string? PhysicalCondition { get; set; }     // ← thêm
		public bool IsComplete
		{
			get => ItemStatus == "Complete";
			set
			{
				// Khi CheckBox được tích -> True -> "Complete"
				// Khi CheckBox bỏ tích -> False -> "Borrowing"
				ItemStatus = value ? "Complete" : "Borrowing";
			}
		}
	}
}
