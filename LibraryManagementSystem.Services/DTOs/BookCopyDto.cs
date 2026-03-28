using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.DTOs
{
	public class BookCopyDto
	{
		public int CopyId { get; set; }
		public string Barcode { get; set; } = string.Empty;
		public string CirculationStatus { get; set; } = string.Empty;
		public string PhysicalCondition { get; set; } = string.Empty;
		public string? ShelfLocation { get; set; }
	}
}
