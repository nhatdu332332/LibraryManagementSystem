using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Data.Entities
{
	public class Publisher
	{
		[Key]
		public int PublisherId { get; set; }

		[Required]
		[StringLength(200)]
		public string PublisherName { get; set; } = null!;

		[StringLength(300)]
		public string? Address { get; set; }

		[StringLength(500)]
		public string? Note { get; set; }

		// Navigation property: 1 Publisher → nhiều BookEdition
		public virtual ICollection<BookEdition> BookEditions { get; set; } = new List<BookEdition>();
	}
}
