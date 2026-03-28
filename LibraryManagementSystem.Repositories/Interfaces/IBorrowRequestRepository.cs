using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories.Interfaces
{
	public interface IBorrowRequestRepository : IGenericRepository<BorrowRequest>
	{
		/// <summary>
		/// Lấy tất cả BorrowRequest đang Pending (chờ duyệt)
		/// </summary>
		Task<IEnumerable<BorrowRequest>> GetPendingRequestsAsync();

		/// <summary>
		/// Lấy tất cả BorrowRequest của một Reader cụ thể (theo ReaderId)
		/// </summary>
		Task<IEnumerable<BorrowRequest>> GetByReaderIdAsync(int readerId);

		/// <summary>
		/// Lấy tất cả BorrowRequest được duyệt bởi một Librarian/Staff cụ thể (ApprovedByEmployeeId)
		/// </summary>
		Task<IEnumerable<BorrowRequest>> GetApprovedByEmployeeIdAsync(int employeeId);

		/// <summary>
		/// Lấy chi tiết BorrowRequest kèm BorrowRequestDetail (eager loading)
		/// </summary>
		Task<BorrowRequest?> GetRequestWithDetailsAsync(int requestId);

		/// <summary>
		/// Lấy tất cả BorrowRequest có ApprovedQuantity > 0 (đã duyệt một phần hoặc toàn bộ)
		/// </summary>
		Task<IEnumerable<BorrowRequest>> GetApprovedRequestsAsync();
	}
}
