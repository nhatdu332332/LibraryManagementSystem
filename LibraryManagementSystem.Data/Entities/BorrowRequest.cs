using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Data.Entities
{
	public class BorrowRequest
	{
		[Key]
		public int RequestId { get; set; }

		[Required]
		public int ReaderId { get; set; }

		[ForeignKey(nameof(ReaderId))]
		public virtual Reader Reader { get; set; } = null!;

		[Required]
		public DateTime RequestDate { get; set; }

		public int? ApprovedByEmployeeId { get; set; }

		[ForeignKey(nameof(ApprovedByEmployeeId))]
		public virtual Employee? ApprovedByEmployee { get; set; }

		public DateTime? ApprovedDate { get; set; }

		[StringLength(500)]
		public string? RejectReason { get; set; }

		[Required]
		[StringLength(20)]
		public string Status { get; set; } = null!;   // Pending, Approved, Rejected, Cancelled, ...

		// Navigation properties
		public virtual ICollection<BorrowRequestDetail> Details { get; set; } = new List<BorrowRequestDetail>();

		public virtual BorrowTransaction? BorrowTransaction { get; set; }   // 1 request → có thể 1 transaction (nếu approved)
	}
}
