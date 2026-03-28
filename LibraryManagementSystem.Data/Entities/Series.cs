using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Data.Entities
{
	public class Series
	{
		[Key]
		public int SeriesId { get; set; }

		[Required]
		[StringLength(200)]
		public string SeriesName { get; set; } = null!;

		[StringLength(500)]
		public string? Description { get; set; }

		// Navigation property
		public virtual ICollection<BookWork> BookWorks { get; set; } = new List<BookWork>();
	}
}
