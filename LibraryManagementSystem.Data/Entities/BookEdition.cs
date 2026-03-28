using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Data.Entities
{
	public class BookEdition
	{
		[Key]
		public int EditionId { get; set; }

		[Required]
		public int WorkId { get; set; }

		[ForeignKey(nameof(WorkId))]
		public virtual BookWork BookWork { get; set; } = null!;

		[Required]
		[StringLength(20)]
		public string ISBN { get; set; } = null!;  // unique trong DB

		[Required]
		public int PublisherId { get; set; }

		[ForeignKey(nameof(PublisherId))]
		public virtual Publisher Publisher { get; set; } = null!;

		[Required]
		public int PublishYear { get; set; }

		public int? EditionNumber { get; set; }   // ví dụ: lần tái bản thứ mấy

		[StringLength(50)]
		public string? Language { get; set; }

		public int? PageCount { get; set; }

		[StringLength(50)]
		public string? Format { get; set; }   // Paperback, Hardcover, Ebook, ...

		// Navigation property
		public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
	}
}
