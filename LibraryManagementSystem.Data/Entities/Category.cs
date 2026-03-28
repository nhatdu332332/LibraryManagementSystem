using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Data.Entities
{
	public class Category
	{
		[Key]
		public int CategoryId { get; set; }

		[Required]
		[StringLength(100)]
		public string CategoryName { get; set; } = null!;

		[StringLength(500)]
		public string? Description { get; set; }

		// Navigation property: 1 Category → nhiều WorkCategory
		public virtual ICollection<WorkCategory> WorkCategories { get; set; } = new List<WorkCategory>();
	}
}
