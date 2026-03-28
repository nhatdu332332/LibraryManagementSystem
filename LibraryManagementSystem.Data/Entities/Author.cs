using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Data.Entities
{
	public class Author
	{
		[Key]
		public int AuthorId { get; set; }

		[Required]
		[StringLength(150)]
		public string AuthorName { get; set; } = null!;

		[StringLength(500)]
		public string? Note { get; set; }

		// Navigation property: 1 Author → nhiều WorkAuthor (nhiều sách)
		public virtual ICollection<WorkAuthor> WorkAuthors { get; set; } = new List<WorkAuthor>();
	}
}
