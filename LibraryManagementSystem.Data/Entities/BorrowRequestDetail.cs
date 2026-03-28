using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Data.Entities
{
	public class BorrowRequestDetail
	{
		[Key]
		public int RequestDetailId { get; set; }

		[Required]
		public int RequestId { get; set; }

		[ForeignKey(nameof(RequestId))]
		public virtual BorrowRequest BorrowRequest { get; set; } = null!;

		[Required]
		public int WorkId { get; set; }

		[ForeignKey(nameof(WorkId))]
		public virtual BookWork BookWork { get; set; } = null!;

		[Required]
		[Range(1, int.MaxValue)]
		public int RequestedQuantity { get; set; }

		[Range(0, int.MaxValue)]
		public int? ApprovedQuantity { get; set; }   // null nếu chưa duyệt, 0 nếu từ chối hoàn toàn
	}
}
