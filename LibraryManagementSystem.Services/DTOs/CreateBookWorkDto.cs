using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace LibraryManagementSystem.Services.DTOs
{
	public class CreateBookWorkDto
	{
		/// <summary>
		/// Tên sách (title)
		/// </summary>
		public string Title { get; set; } = string.Empty;

		/// <summary>
		/// Tên gốc (nếu có, ví dụ tiếng Anh hoặc tiếng gốc)
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
		/// ID series (nếu thuộc series, optional)
		/// </summary>
		public int? SeriesId { get; set; }

		/// <summary>
		/// Số thứ tự trong series (VolumeNumber)
		/// </summary>
		public int VolumeNumber { get; set; }

		/// <summary>
		/// Danh sách ID tác giả (từ bảng Author)
		/// </summary>
		public List<int> AuthorIds { get; set; } = new List<int>();

		/// <summary>
		/// Danh sách ID thể loại (từ bảng Category)
		/// </summary>
		public List<int> CategoryIds { get; set; } = new List<int>();
	}
}
