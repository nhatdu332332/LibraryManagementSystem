using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace LibraryManagementSystem.Services.DTOs
{
	public class ApproveBorrowRequestDto
	{
		/// <summary>
		/// Danh sách số lượng được duyệt cho từng WorkId (có thể < RequestedQuantity nếu hết hàng)
		/// </summary>
		public List<BorrowRequestDetailApproveDto> ApprovedDetails { get; set; } = new List<BorrowRequestDetailApproveDto>();
	}

	/// <summary>
	/// Chi tiết duyệt từng dòng
	/// </summary>
	public class BorrowRequestDetailApproveDto
	{
		/// <summary>
		/// ID chi tiết request (RequestDetailId)
		/// </summary>
		public int RequestDetailId { get; set; }

		/// <summary>
		/// Số lượng được duyệt (0 nếu từ chối hoàn toàn dòng này)
		/// </summary>
		public int ApprovedQuantity { get; set; }
	}
}
