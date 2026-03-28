using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Services.DTOs
{
	public class BorrowRequestDto
	{
		/// <summary>
		/// ID yêu cầu mượn
		/// </summary>
		public int RequestId { get; set; }

		/// <summary>
		/// ID độc giả gửi request
		/// </summary>
		public int ReaderId { get; set; }

		/// <summary>
		/// Tên độc giả
		/// </summary>
		public string ReaderFullName { get; set; } = string.Empty;

		/// <summary>
		/// Ngày gửi request
		/// </summary>
		public DateTime RequestDate { get; set; }

		/// <summary>
		/// Trạng thái (Pending, Approved, Rejected)
		/// </summary>
		public string Status { get; set; } = string.Empty;

		/// <summary>
		/// ID nhân viên duyệt (nếu Approved)
		/// </summary>
		public int? ApprovedByEmployeeId { get; set; }

		/// <summary>
		/// Tên nhân viên duyệt
		/// </summary>
		public string? ApprovedByEmployeeName { get; set; }

		/// <summary>
		/// Ngày duyệt
		/// </summary>
		public DateTime? ApprovedDate { get; set; }

		/// <summary>
		/// Lý do từ chối (nếu Rejected)
		/// </summary>
		public string? RejectReason { get; set; }

		/// <summary>
		/// Danh sách chi tiết yêu cầu (tác phẩm + số lượng yêu cầu/duyệt)
		/// </summary>
		public List<BorrowRequestDetailDto> Details { get; set; } = new List<BorrowRequestDetailDto>();
	}

	/// <summary>
	/// Chi tiết từng dòng trong request
	/// </summary>
	public class BorrowRequestDetailDto
	{
		public int WorkId { get; set; }
		public string Title { get; set; } = string.Empty;
		public int RequestedQuantity { get; set; }
		public int? ApprovedQuantity { get; set; }
	}
}
