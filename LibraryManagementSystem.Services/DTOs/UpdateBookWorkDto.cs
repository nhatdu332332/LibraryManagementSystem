using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace LibraryManagementSystem.Services.DTOs
{
	public class UpdateBookWorkDto
	{
		/// <summary>
		/// Tên sách (nếu không thay đổi thì null)
		/// </summary>
		public string? Title { get; set; }

		/// <summary>
		/// Tên gốc (nếu không thay đổi thì null)
		/// </summary>
		public string? OriginalTitle { get; set; }

		/// <summary>
		/// Tóm tắt nội dung (nếu không thay đổi thì null)
		/// </summary>
		public string? Summary { get; set; }

		/// <summary>
		/// Năm xuất bản đầu tiên (nếu không thay đổi thì null)
		/// </summary>
		public int? FirstPublishYear { get; set; }

		/// <summary>
		/// ID series (nếu không thay đổi thì null)
		/// </summary>
		public int? SeriesId { get; set; }

		/// <summary>
		/// Số thứ tự trong series (nếu không thay đổi thì null)
		/// </summary>
		public int? VolumeNumber { get; set; }

		/// <summary>
		/// Danh sách ID tác giả mới (thay thế hoàn toàn danh sách cũ nếu gửi)
		/// Nếu null thì giữ nguyên
		/// </summary>
		public List<int>? AuthorIds { get; set; }

		/// <summary>
		/// Danh sách ID thể loại mới (thay thế hoàn toàn danh sách cũ nếu gửi)
		/// Nếu null thì giữ nguyên
		/// </summary>
		public List<int>? CategoryIds { get; set; }
	}
}
