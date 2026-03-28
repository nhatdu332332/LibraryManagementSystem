using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories
{
	public class BorrowRequestRepository : GenericRepository<BorrowRequest>, IBorrowRequestRepository
	{
		public BorrowRequestRepository(LibraryDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Lấy tất cả BorrowRequest đang chờ duyệt (Pending)
		/// </summary>
		public async Task<IEnumerable<BorrowRequest>> GetPendingRequestsAsync()
		{
			return await _dbSet
				.Where(r => r.Status == "Pending")
				.Include(r => r.Reader)           // eager loading Reader
				.Include(r => r.Details)          // eager loading Detail
				.ToListAsync();
		}

		/// <summary>
		/// Lấy tất cả BorrowRequest của một Reader cụ thể
		/// </summary>
		public async Task<IEnumerable<BorrowRequest>> GetByReaderIdAsync(int readerId)
		{
			return await _dbSet
				.Where(r => r.ReaderId == readerId)
				.Include(r => r.Details)
				.ToListAsync();
		}

		/// <summary>
		/// Lấy tất cả BorrowRequest được duyệt bởi một nhân viên cụ thể
		/// </summary>
		public async Task<IEnumerable<BorrowRequest>> GetApprovedByEmployeeIdAsync(int employeeId)
		{
			return await _dbSet
				.Where(r => r.ApprovedByEmployeeId == employeeId)
				.Include(r => r.Details)
				.ToListAsync();
		}

		/// <summary>
		/// Lấy chi tiết BorrowRequest kèm Detail (eager loading)
		/// </summary>
		public async Task<BorrowRequest?> GetRequestWithDetailsAsync(int requestId)
		{
			return await _dbSet
				.Include(r => r.Reader)
				.Include(r => r.Details)
					.ThenInclude(d => d.BookWork)  // chi tiết tác phẩm
				.FirstOrDefaultAsync(r => r.RequestId == requestId);
		}

		/// <summary>
		/// Lấy tất cả BorrowRequest đã được duyệt (ApprovedQuantity > 0)
		/// </summary>
		public async Task<IEnumerable<BorrowRequest>> GetApprovedRequestsAsync()
		{
			return await _dbSet
				.Where(r => r.Status == "Approved")
				.Include(r => r.Details)
				.ToListAsync();
		}
	}
}
