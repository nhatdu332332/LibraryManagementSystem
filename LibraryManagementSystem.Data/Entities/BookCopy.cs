using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Data.Entities
{
	public class BookCopy
	{
		[Key]
		public int CopyId { get; set; }

		[Required]
		public int EditionId { get; set; }

		[ForeignKey(nameof(EditionId))]
		public virtual BookEdition BookEdition { get; set; } = null!;

		[Required]
		[StringLength(50)]
		public string Barcode { get; set; } = null!;  // unique trong DB

		[Required]
		[StringLength(20)]
		public string CirculationStatus { get; set; } = null!;   // Available, Borrowed, Reserved, Damaged, Lost, ...

		[Required]
		[StringLength(20)]
		public string PhysicalCondition { get; set; } = null!;   // Excellent, Good, Fair, Poor

		[StringLength(50)]
		public string? ShelfLocation { get; set; }

		[Required]
		public DateTime AddedDate { get; set; }

		public DateTime? RemovedDate { get; set; }

		// Navigation property
		public virtual ICollection<BorrowTransactionDetail> BorrowTransactionDetails { get; set; } = new List<BorrowTransactionDetail>();
	}
}
