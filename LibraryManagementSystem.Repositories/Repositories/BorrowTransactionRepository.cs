// Full code cho BorrowTransactionRepository.cs (override GetAllAsync với Include để load đầy đủ)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories
{
	public class BorrowTransactionRepository : GenericRepository<BorrowTransaction>, IBorrowTransactionRepository
	{
		public BorrowTransactionRepository(LibraryDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<BorrowTransaction>> GetByReaderIdAsync(int readerId)
		{
			return await _dbSet
				.AsNoTracking()
				.Where(t => t.ReaderId == readerId)
				.Include(t => t.Details)
				.ToListAsync();
		}

		public async Task<IEnumerable<BorrowTransaction>> GetActiveTransactionsAsync()
		{
			return await _dbSet
				.AsNoTracking()
				.Where(t => t.Status == "Borrowed")
				.Include(t => t.Details)
				.ToListAsync();
		}

		public async Task<IEnumerable<BorrowTransaction>> GetByEmployeeIdAsync(int employeeId)
		{
			return await _dbSet
				.Where(t => t.EmployeeId == employeeId)
				.Include(t => t.Details)
				.ToListAsync();
		}

		public async Task<BorrowTransaction?> GetTransactionWithDetailsAsync(int borrowId)
		{
			return await _dbSet
				.AsNoTracking()
				.Include(t => t.Details)
					.ThenInclude(d => d.BookCopy)
						.ThenInclude(c => c.BookEdition)
							.ThenInclude(e => e.BookWork)
				.FirstOrDefaultAsync(t => t.BorrowId == borrowId);
		}

		public async Task<IEnumerable<BorrowTransaction>> GetFromRequestAsync(int requestId)
		{
			return await _dbSet
				.Where(t => t.RequestId == requestId)
				.Include(t => t.Details)
				.ToListAsync();
		}

		public async Task<IEnumerable<BorrowTransaction>> GetOverdueTransactionsAsync()
		{
			return await _dbSet
				.AsNoTracking()
				.Where(t => t.Details.Any(d =>
					d.ItemStatus == "Borrowing" &&   // Chỉ xét những cuốn đang mượn
					d.ReturnDate == null &&         // Và chưa được trả
					d.DueDate < DateTime.Today))    // Và đã quá hạn trả
				.Include(t => t.Details)            // Lấy kèm chi tiết để hiển thị
				.Include(t => t.Reader)             // Nên lấy thêm Reader để biết ai nợ
				.ToListAsync();
		}

		// Override GetAllAsync để include đầy đủ data (fix lỗi null khi load Title, Reader, Employee)
		public override async Task<IEnumerable<BorrowTransaction>> GetAllAsync()
		{
			return await _dbSet
				.AsNoTracking()
				.Include(t => t.Reader)
				.Include(t => t.Employee)
				.Include(t => t.Details)
					.ThenInclude(d => d.BookCopy)
						.ThenInclude(c => c.BookEdition)
							.ThenInclude(e => e.BookWork)
				.ToListAsync();
		}
	}
}