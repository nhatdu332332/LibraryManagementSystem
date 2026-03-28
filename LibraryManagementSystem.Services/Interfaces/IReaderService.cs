using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagementSystem.Services.DTOs; // Sẽ tạo DTOs sau

namespace LibraryManagementSystem.Services.Interfaces
{
	public interface IReaderService
	{
		/// <summary>
		/// Lấy thông tin độc giả theo ID
		/// </summary>
		Task<ReaderDto> GetReaderByIdAsync(int readerId);

		/// <summary>
		/// Lấy tất cả độc giả (Admin/Staff/Librarian xem danh sách)
		/// </summary>
		Task<IEnumerable<ReaderDto>> GetAllReadersAsync();

		/// <summary>
		/// Cập nhật thông tin độc giả (tự update profile hoặc Admin/Staff/Librarian update)
		/// </summary>
		Task UpdateReaderAsync(int readerId, UpdateReaderDto dto, int currentUserId, string currentRoleName);

		/// <summary>
		/// Xóa hoặc deactivate độc giả (Admin/Staff/Librarian)
		/// </summary>
		Task DeleteReaderAsync(int readerId, int currentUserId, string currentRoleName);

		/// <summary>
		/// Gia hạn thẻ độc giả (tăng ExpiredDate)
		/// </summary>
		Task RenewReaderCardAsync(int readerId, DateTime newExpiredDate, int currentUserId, string currentRoleName);

		/// <summary>
		/// Tìm độc giả theo mã thẻ hoặc email (dùng khi login hoặc tìm kiếm)
		/// </summary>
		Task<ReaderDto?> FindByCardNumberOrEmailAsync(string cardNumberOrEmail);
	}
}
