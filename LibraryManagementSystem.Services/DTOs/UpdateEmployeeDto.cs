using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.DTOs
{
	public class UpdateEmployeeDto
	{
		public string? Email { get; set; }
		public string? FullName { get; set; }
		public string? Status { get; set; } // Active, Inactive (Admin dùng)
		public int? RoleId { get; set; } // thay đổi role (chỉ Admin)
	}
}
