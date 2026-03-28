using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace LibraryManagementSystem.Services.DTOs
{
	public class UpdateReaderDto
	{
		public string? Email { get; set; }
		public string? FullName { get; set; } // optional, nếu null thì giữ nguyên
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public string? ReaderStatus { get; set; } // Active, Expired, Suspended (Admin/Staff/Librarian dùng)
		public DateTime? ExpiredDate { get; set; } // gia hạn thẻ (Admin/Staff/Librarian dùng)
	}
}
