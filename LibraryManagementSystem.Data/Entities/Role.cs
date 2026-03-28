using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Data.Entities
{
	public class Role
	{
		[Key]
		public int RoleId { get; set; }

		[Required]
		[StringLength(50)]
		public string RoleName { get; set; } = null!;

		// Navigation property (1 Role - nhiều Employee)
		public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

		public string? Description { get; set; } // nullable, mô tả quyền hạn của role
	}
}
