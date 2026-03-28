using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.DTOs
{
	public class BookEditionDto
	{
		public int EditionId { get; set; }
		public string ISBN { get; set; } = string.Empty;
		public string PublisherName { get; set; } = string.Empty;
		public int PublishYear { get; set; }
		public string? Language { get; set; }
		public string? Format { get; set; }
		public int AvailableCopies { get; set; }
	}
}
