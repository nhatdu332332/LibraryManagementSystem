using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Data.Entities
{
	public class BorrowTransaction
	{
		[Key]
		public int BorrowId { get; set; }

		[Required]
		public int ReaderId { get; set; }

		[ForeignKey(nameof(ReaderId))]
		public virtual Reader Reader { get; set; } = null!;

		[Required]
		public int EmployeeId { get; set; }

		[ForeignKey(nameof(EmployeeId))]
		public virtual Employee Employee { get; set; } = null!;

		public int? RequestId { get; set; }

		[ForeignKey(nameof(RequestId))]
		public virtual BorrowRequest? BorrowRequest { get; set; }   // optional, nếu mượn từ request

		[Required]
		public DateTime BorrowDate { get; set; }

		[Required]
		[StringLength(20)]
		public string Status { get; set; } = null!;   // Borrowed, PartiallyReturned, FullyReturned, Overdue, ...

		// Navigation property
		public virtual ICollection<BorrowTransactionDetail> Details { get; set; } = new List<BorrowTransactionDetail>();
	}
}
