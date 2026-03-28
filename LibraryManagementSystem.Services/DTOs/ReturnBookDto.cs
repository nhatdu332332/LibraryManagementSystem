using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace LibraryManagementSystem.Services.DTOs
{
	public class ReturnBookDto
	{
		/// <summary>
		/// Ngày trả thực tế (mặc định Today nếu null)
		/// </summary>
		public DateTime? ReturnDate { get; set; }

		/// <summary>
		/// Trạng thái vật lý khi trả (Good, Damaged, Lost, ...)
		/// </summary>
		public string ItemStatus { get; set; } = "Good";

		/// <summary>
		/// Số tiền phạt (nếu quá hạn hoặc hư hỏng)
		/// </summary>
		public decimal FineAmount { get; set; }

		/// <summary>
		/// Ghi chú tình trạng sách khi trả (ví dụ: rách bìa, mất trang, ...)
		/// </summary>
		public string? ConditionNote { get; set; }
	}
}
