using System;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories.Interfaces
{
	public interface IUnitOfWork : IDisposable
	{
		IRoleRepository RoleRepository { get; }
		IEmployeeRepository EmployeeRepository { get; }
		IReaderRepository ReaderRepository { get; }
		IBookWorkRepository BookWorkRepository { get; }
		IBorrowRequestRepository BorrowRequestRepository { get; }
		IBorrowTransactionRepository BorrowTransactionRepository { get; }

		// Thêm dòng này để fix lỗi CS1061 ở BorrowService.cs (dòng 204)
		IBookCopyRepository BookCopyRepository { get; }

		// Nếu bạn có thêm repository khác (Author, Category, Publisher, Series, v.v.) thì thêm ở đây
		// Ví dụ:
		// IAuthorRepository AuthorRepository { get; }
		// ICategoryRepository CategoryRepository { get; }
		LibraryDbContext DbContext { get; }
		Task<int> SaveChangesAsync();
		int SaveChanges();
	}
}
