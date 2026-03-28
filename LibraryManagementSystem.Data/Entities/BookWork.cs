using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Data.Entities
{
	public class BookWork
	{
		[Key]
		public int WorkId { get; set; }

		[Required]
		[StringLength(300)]
		public string Title { get; set; } = null!;

		[StringLength(300)]
		public string? OriginalTitle { get; set; }

		public string? Summary { get; set; }

		public int? FirstPublishYear { get; set; }

		public int SeriesId { get; set; }

		[ForeignKey(nameof(SeriesId))]
		public virtual Series Series { get; set; } = null!;

		public int VolumeNumber { get; set; }

		// Navigation properties
		public virtual ICollection<WorkAuthor> WorkAuthors { get; set; } = new List<WorkAuthor>();
		public virtual ICollection<WorkCategory> WorkCategories { get; set; } = new List<WorkCategory>();
		public virtual ICollection<BookEdition> BookEditions { get; set; } = new List<BookEdition>();
		public virtual ICollection<BorrowRequestDetail> BorrowRequestDetails { get; set; } = new List<BorrowRequestDetail>();
	}
}
