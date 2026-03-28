using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace LibraryManagementSystem.Services.DTOs
{
	public class BookWorkDto
	{
		/// <summary>
		/// ID tác phẩm
		/// </summary>
		public int WorkId { get; set; }

		/// <summary>
		/// Tên sách (title)
		/// </summary>
		public string Title { get; set; } = string.Empty;

		/// <summary>
		/// Tên gốc (nếu có)
		/// </summary>
		public string? OriginalTitle { get; set; }

		/// <summary>
		/// Tóm tắt nội dung
		/// </summary>
		public string? Summary { get; set; }

		/// <summary>
		/// Năm xuất bản đầu tiên
		/// </summary>
		public int? FirstPublishYear { get; set; }

		/// <summary>
		/// ID series (nếu thuộc series)
		/// </summary>
		public int? SeriesId { get; set; }

		/// <summary>
		/// Số thứ tự trong series (VolumeNumber)
		/// </summary>
		public int VolumeNumber { get; set; }

		/// <summary>
		/// Danh sách tên tác giả (từ WorkAuthor)
		/// </summary>
		public List<string> Authors { get; set; } = new List<string>();

		/// <summary>
		/// Danh sách thể loại (từ WorkCategory)
		/// </summary>
		public List<string> Categories { get; set; } = new List<string>();
		// Thêm 2 dòng này
		public string AuthorsString => string.Join(", ", Authors ?? Enumerable.Empty<string>());
		public string CategoriesString => string.Join(", ", Categories ?? Enumerable.Empty<string>());


		/// <summary>
		/// Số lượng bản sao còn available (tính từ BookEdition + BookCopy)
		/// </summary>
		public int AvailableCopies { get; set; }

		/// <summary>
		/// Tổng số bản sao (tất cả edition)
		/// </summary>
		public int TotalCopies { get; set; }
	}
}