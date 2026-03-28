using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace LibraryManagementSystem.Services.DTOs
{
	public class UpdateBorrowDetailDto
	{
		public int BorrowDetailId { get; set; }
		public DateTime DueDate { get; set; }
		// Thêm để cập nhật trạng thái mượn của bản sao
		public string? CirculationStatus { get; set; }

		// Optional: nếu muốn cập nhật luôn tình trạng vật lý
		public string? PhysicalCondition { get; set; }

		
	}
}
