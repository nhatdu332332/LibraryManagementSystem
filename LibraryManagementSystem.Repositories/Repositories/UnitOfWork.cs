using System;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Repositories.Interfaces;

namespace LibraryManagementSystem.Repositories.Repositories
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private readonly LibraryDbContext _context;
		private bool _disposed = false;

		// Các repository cụ thể (lazy init)
		private IRoleRepository? _roleRepository;
		private IEmployeeRepository? _employeeRepository;
		private IReaderRepository? _readerRepository;
		private IBookWorkRepository? _bookWorkRepository;
		private IBorrowRequestRepository? _borrowRequestRepository;
		private IBorrowTransactionRepository? _borrowTransactionRepository;
		private IBookCopyRepository? _bookCopyRepository;

		public UnitOfWork(LibraryDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public IRoleRepository RoleRepository =>
			_roleRepository ??= new RoleRepository(_context);

		public IEmployeeRepository EmployeeRepository =>
			_employeeRepository ??= new EmployeeRepository(_context);

		public IReaderRepository ReaderRepository =>
			_readerRepository ??= new ReaderRepository(_context);

		public IBookWorkRepository BookWorkRepository =>
			_bookWorkRepository ??= new BookWorkRepository(_context);

		public IBorrowRequestRepository BorrowRequestRepository =>
			_borrowRequestRepository ??= new BorrowRequestRepository(_context);

		public IBorrowTransactionRepository BorrowTransactionRepository =>
			_borrowTransactionRepository ??= new BorrowTransactionRepository(_context);

		public IBookCopyRepository BookCopyRepository =>
			_bookCopyRepository ??= new BookCopyRepository(_context);

		public LibraryDbContext DbContext => _context;

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public int SaveChanges()
		{
			return _context.SaveChanges();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_context?.Dispose();
				}
				_disposed = true;
			}
		}
	}
}