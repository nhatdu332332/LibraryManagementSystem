using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace LibraryManagementSystem.Services.DTOs
{
	public class SubmitBorrowRequestDto
	{
		/// <summary>
		/// Danh sách chi tiết yêu cầu mượn (tác phẩm + số lượng muốn mượn)
		/// </summary>
		public List<BorrowRequestDetailSubmitDto> Details { get; set; } = new List<BorrowRequestDetailSubmitDto>();
	}

	/// <summary>
	/// Chi tiết từng dòng yêu cầu mượn
	/// </summary>
	public class BorrowRequestDetailSubmitDto
	{
		/// <summary>
		/// ID tác phẩm (BookWork) muốn mượn
		/// </summary>
		public int WorkId { get; set; }

		/// <summary>
		/// Số lượng bản sao muốn mượn (của tác phẩm đó)
		/// </summary>
		public int RequestedQuantity { get; set; }
	}
}
