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
	public class ReaderRepository : GenericRepository<Reader>, IReaderRepository
	{
		public ReaderRepository(LibraryDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Tìm Reader theo CardNumber (mã thẻ bạn đọc)
		/// </summary>
		public async Task<Reader?> GetByCardNumberAsync(string cardNumber)
		{
			return await _dbSet
				.FirstOrDefaultAsync(r => r.CardNumber == cardNumber);
		}

		/// <summary>
		/// Tìm Reader theo email (dùng khi login Reader)
		/// </summary>
		public async Task<Reader?> GetByEmailAsync(string email)
		{
			return await _dbSet
				.FirstOrDefaultAsync(r => r.Email.ToLower() == email.ToLower());
		}

		/// <summary>
		/// Lấy tất cả Reader còn hoạt động (ReaderStatus = Active, ExpiredDate > Today)
		/// </summary>
		public async Task<IEnumerable<Reader>> GetActiveReadersAsync()
		{
			return await _dbSet
				.Where(r => r.ReaderStatus == "Active" && r.ExpiredDate >= DateTime.Today)
				.ToListAsync();
		}
	}
}
