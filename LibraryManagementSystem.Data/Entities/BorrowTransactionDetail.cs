using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Data.Entities
{
	public class BorrowTransactionDetail
	{
		[Key]
		public int BorrowDetailId { get; set; }

		[Required]
		public int BorrowId { get; set; }

		[ForeignKey(nameof(BorrowId))]
		public virtual BorrowTransaction BorrowTransaction { get; set; } = null!;

		[Required]
		public int CopyId { get; set; }

		[ForeignKey(nameof(CopyId))]
		public virtual BookCopy BookCopy { get; set; } = null!;

		[Required]
		public DateTime DueDate { get; set; }

		public DateTime? ReturnDate { get; set; }

		[Required]
		[StringLength(20)]
		public string ItemStatus { get; set; } = null!;   // Borrowed, Returned, Overdue, Damaged, Lost, ...

		[Column(TypeName = "decimal(10,2)")]
		public decimal FineAmount { get; set; } = 0m;

		[StringLength(500)]
		public string? ConditionNote { get; set; }
	}
}
