using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace LibraryManagementSystem.Services.DTOs
{
	public class CreateReaderDto
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty; // plaintext, service sẽ hash
		public string FullName { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public DateTime? ExpiredDate { get; set; } // nếu null, service tự set mặc định 2 năm
	}
}
