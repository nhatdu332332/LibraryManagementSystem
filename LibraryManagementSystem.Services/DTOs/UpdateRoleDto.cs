using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.DTOs
{
	public class UpdateRoleDto
	{
		/// <summary>
		/// Tên role mới (nếu không thay đổi thì null – phải unique nếu thay đổi)
		/// </summary>
		public string? RoleName { get; set; }

		/// <summary>
		/// Mô tả mới (nếu không thay đổi thì null)
		/// </summary>
		public string? Description { get; set; }
	}
}
