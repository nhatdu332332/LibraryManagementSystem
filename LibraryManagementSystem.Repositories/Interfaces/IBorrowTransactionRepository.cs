using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

using LibraryManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories.Interfaces
{
	public interface IBorrowTransactionRepository : IGenericRepository<BorrowTransaction>
	{
		Task<IEnumerable<BorrowTransaction>> GetByReaderIdAsync(int readerId);
		Task<IEnumerable<BorrowTransaction>> GetActiveTransactionsAsync();
		Task<IEnumerable<BorrowTransaction>> GetByEmployeeIdAsync(int employeeId);
		Task<BorrowTransaction?> GetTransactionWithDetailsAsync(int borrowId);
		Task<IEnumerable<BorrowTransaction>> GetFromRequestAsync(int requestId);
		Task<IEnumerable<BorrowTransaction>> GetOverdueTransactionsAsync();
	}
}
