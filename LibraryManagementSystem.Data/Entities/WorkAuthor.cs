using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Data.Entities
{
	public class WorkAuthor
	{
		[Key]
		[Column(Order = 0)]
		public int WorkId { get; set; }

		[ForeignKey(nameof(WorkId))]
		public virtual BookWork BookWork { get; set; } = null!;

		[Key]
		[Column(Order = 1)]
		public int AuthorId { get; set; }

		[ForeignKey(nameof(AuthorId))]
		public virtual Author Author { get; set; } = null!;

	}
}
