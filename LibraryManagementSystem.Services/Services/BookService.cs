using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
	public class BookService : IBookService
	{
		private readonly IUnitOfWork _uow;

		public BookService(IUnitOfWork uow)
		{
			_uow = uow;
		}

		public async Task<BookWork?> GetBookWorkDetailAsync(int workId)
		{
			return await _uow.DbContext.BookWorks
				.Include(b => b.Series)
				.Include(b => b.WorkAuthors).ThenInclude(wa => wa.Author)
				.Include(b => b.WorkCategories).ThenInclude(wc => wc.Category)
				.Include(b => b.BookEditions).ThenInclude(e => e.Publisher)
				.Include(b => b.BookEditions).ThenInclude(e => e.BookCopies)
				.FirstOrDefaultAsync(b => b.WorkId == workId);
		}

		public async Task<BookWorkDto> GetBookWorkByIdAsync(int workId)
		{
			var book = await _uow.DbContext.BookWorks
				.Include(b => b.WorkAuthors).ThenInclude(wa => wa.Author)
				.Include(b => b.WorkCategories).ThenInclude(wc => wc.Category)
				.Include(b => b.BookEditions).ThenInclude(e => e.BookCopies)
				.FirstOrDefaultAsync(b => b.WorkId == workId);

			if (book == null)
			{
				return null;
			}

			return new BookWorkDto
			{
				WorkId = book.WorkId,
				Title = book.Title,
				OriginalTitle = book.OriginalTitle,
				Summary = book.Summary,
				SeriesId = book.SeriesId,
				VolumeNumber = book.VolumeNumber,
				Authors = book.WorkAuthors?.Select(wa => wa.Author.AuthorName).ToList() ?? new List<string>(),
				Categories = book.WorkCategories?.Select(wc => wc.Category.CategoryName).ToList() ?? new List<string>(),
				AvailableCopies = book.BookEditions?.Sum(e => e.BookCopies?.Count(c => c.CirculationStatus == "Available") ?? 0) ?? 0,
				TotalCopies = book.BookEditions?.Sum(e => e.BookCopies?.Count ?? 0) ?? 0
			};
		}

		public async Task<IEnumerable<BookWorkDto>> GetAllBookWorksAsync()
		{
			var query = _uow.DbContext.BookWorks
				.Include(b => b.WorkAuthors).ThenInclude(wa => wa.Author)
				.Include(b => b.WorkCategories).ThenInclude(wc => wc.Category)
				.Include(b => b.BookEditions).ThenInclude(e => e.BookCopies);

			var books = await query.ToListAsync();

			return books.Select(book => new BookWorkDto
			{
				WorkId = book.WorkId,
				Title = book.Title,
				OriginalTitle = book.OriginalTitle,
				Summary = book.Summary,
				SeriesId = book.SeriesId,
				VolumeNumber = book.VolumeNumber,
				Authors = book.WorkAuthors?.Select(wa => wa.Author.AuthorName).ToList() ?? new List<string>(),
				Categories = book.WorkCategories?.Select(wc => wc.Category.CategoryName).ToList() ?? new List<string>(),
				AvailableCopies = book.BookEditions?.Sum(e => e.BookCopies?.Count(c => c.CirculationStatus == "Available") ?? 0) ?? 0,
				TotalCopies = book.BookEditions?.Sum(e => e.BookCopies?.Count ?? 0) ?? 0
			});
		}

		public async Task<IEnumerable<BookWorkDto>> SearchBooksAsync(string keyword, int? authorId, int? categoryId, int? seriesId)
		{
			var query = _uow.DbContext.BookWorks
				.Include(b => b.WorkAuthors).ThenInclude(wa => wa.Author)
				.Include(b => b.WorkCategories).ThenInclude(wc => wc.Category)
				.Include(b => b.BookEditions).ThenInclude(e => e.BookCopies)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				keyword = keyword.Trim().ToLower();
				query = query.Where(b =>
					b.Title.ToLower().Contains(keyword) ||
					(b.OriginalTitle != null && b.OriginalTitle.ToLower().Contains(keyword)) ||
					(b.Summary != null && b.Summary.ToLower().Contains(keyword))
				);
			}

			if (authorId.HasValue)
			{
				query = query.Where(b => b.WorkAuthors.Any(wa => wa.AuthorId == authorId.Value));
			}

			if (categoryId.HasValue)
			{
				query = query.Where(b => b.WorkCategories.Any(wc => wc.CategoryId == categoryId.Value));
			}

			if (seriesId.HasValue)
			{
				query = query.Where(b => b.SeriesId == seriesId.Value);
			}

			var books = await query.ToListAsync();

			return books.Select(book => new BookWorkDto
			{
				WorkId = book.WorkId,
				Title = book.Title,
				OriginalTitle = book.OriginalTitle,
				Summary = book.Summary,
				SeriesId = book.SeriesId,
				VolumeNumber = book.VolumeNumber,
				Authors = book.WorkAuthors?.Select(wa => wa.Author.AuthorName).ToList() ?? new List<string>(),
				Categories = book.WorkCategories?.Select(wc => wc.Category.CategoryName).ToList() ?? new List<string>(),
				AvailableCopies = book.BookEditions?.Sum(e => e.BookCopies?.Count(c => c.CirculationStatus == "Available") ?? 0) ?? 0,
				TotalCopies = book.BookEditions?.Sum(e => e.BookCopies?.Count ?? 0) ?? 0
			});
		}

		public async Task<BookWorkDto> CreateBookWorkAsync(CreateBookWorkDto dto)
		{
			var bookWork = new BookWork
			{
				Title = dto.Title,
				OriginalTitle = dto.OriginalTitle,
				Summary = dto.Summary,
				FirstPublishYear = dto.FirstPublishYear,
				SeriesId = dto.SeriesId.GetValueOrDefault(),
				VolumeNumber = dto.VolumeNumber
			};

			await _uow.DbContext.BookWorks.AddAsync(bookWork);
			await _uow.SaveChangesAsync();

			if (dto.AuthorIds != null && dto.AuthorIds.Any())
			{
				foreach (var authorId in dto.AuthorIds)
				{
					await _uow.DbContext.WorkAuthors.AddAsync(new WorkAuthor
					{
						WorkId = bookWork.WorkId,
						AuthorId = authorId
					});
				}
			}

			if (dto.CategoryIds != null && dto.CategoryIds.Any())
			{
				foreach (var categoryId in dto.CategoryIds)
				{
					await _uow.DbContext.WorkCategories.AddAsync(new WorkCategory
					{
						WorkId = bookWork.WorkId,
						CategoryId = categoryId
					});
				}
			}

			await _uow.SaveChangesAsync();

			return new BookWorkDto
			{
				WorkId = bookWork.WorkId,
				Title = bookWork.Title,
				OriginalTitle = bookWork.OriginalTitle,
				Summary = bookWork.Summary,
				SeriesId = bookWork.SeriesId,
				VolumeNumber = bookWork.VolumeNumber,
				Authors = new List<string>(),
				Categories = new List<string>(),
				AvailableCopies = 0,
				TotalCopies = 0
			};
		}

		public async Task UpdateBookWorkAsync(UpdateBookWorkDto dto, int workId)
		{
			var bookWork = await _uow.DbContext.BookWorks
				.Include(b => b.WorkAuthors)
				.Include(b => b.WorkCategories)
				.FirstOrDefaultAsync(b => b.WorkId == workId);

			if (bookWork == null)
			{
				throw new KeyNotFoundException("Không tìm thấy tác phẩm");
			}

			if (dto.Title != null) bookWork.Title = dto.Title;
			if (dto.OriginalTitle != null) bookWork.OriginalTitle = dto.OriginalTitle;
			if (dto.Summary != null) bookWork.Summary = dto.Summary;

			// Fix lỗi CS0266: dùng .Value để chuyển int? thành int
			if (dto.SeriesId.HasValue) bookWork.SeriesId = dto.SeriesId.Value;

			if (dto.VolumeNumber.HasValue) bookWork.VolumeNumber = dto.VolumeNumber.Value;

			if (dto.AuthorIds != null)
			{
				_uow.DbContext.WorkAuthors.RemoveRange(bookWork.WorkAuthors);
				foreach (var authorId in dto.AuthorIds)
				{
					bookWork.WorkAuthors.Add(new WorkAuthor { WorkId = workId, AuthorId = authorId });
				}
			}

			if (dto.CategoryIds != null)
			{
				_uow.DbContext.WorkCategories.RemoveRange(bookWork.WorkCategories);
				foreach (var categoryId in dto.CategoryIds)
				{
					bookWork.WorkCategories.Add(new WorkCategory { WorkId = workId, CategoryId = categoryId });
				}
			}

			_uow.DbContext.BookWorks.Update(bookWork);
			await _uow.SaveChangesAsync();
		}

        public async Task DeleteBookWorkAsync(int workId)
        {
            var db = _uow.DbContext;

            // 1. Check BorrowRequestDetail
            var hasRequest = await db.BorrowRequestDetails
                .AnyAsync(r => r.WorkId == workId);

            if (hasRequest)
            {
                throw new Exception("Không thể xóa sách đã có yêu cầu mượn");
            }

            // 2. Lấy editionIds
            var editionIds = await db.BookEditions
                .Where(e => e.WorkId == workId)
                .Select(e => e.EditionId)
                .ToListAsync();

            // 3. Lấy copyIds
            var copyIds = await db.BookCopies
                .Where(c => editionIds.Contains(c.EditionId))
                .Select(c => c.CopyId)
                .ToListAsync();

            // 4. Check BorrowTransactionDetail
            var hasBorrow = await db.BorrowTransactionDetails
                .AnyAsync(b => copyIds.Contains(b.CopyId));

            if (hasBorrow)
            {
                throw new Exception("Không thể xóa sách đã từng được mượn");
            }

            // ==============================
            // Nếu không có liên quan → mới xóa
            // ==============================

            // Xóa bảng trung gian
            var authors = await db.WorkAuthors.Where(x => x.WorkId == workId).ToListAsync();
            var categories = await db.WorkCategories.Where(x => x.WorkId == workId).ToListAsync();

            db.WorkAuthors.RemoveRange(authors);
            db.WorkCategories.RemoveRange(categories);

            // Xóa BookCopy
            var copies = await db.BookCopies
                .Where(c => editionIds.Contains(c.EditionId))
                .ToListAsync();

            db.BookCopies.RemoveRange(copies);

            // Xóa BookEdition
            var editions = await db.BookEditions
                .Where(e => e.WorkId == workId)
                .ToListAsync();

            db.BookEditions.RemoveRange(editions);

            // Xóa BookWork
            var book = await db.BookWorks.FindAsync(workId);
            db.BookWorks.Remove(book);

            await _uow.SaveChangesAsync();
        }
        public async Task<IEnumerable<BookEditionDto>> GetEditionsByWorkIdAsync(int workId)
		{
			var editions = await _uow.DbContext.BookEditions
				.Include(e => e.Publisher)
				.Include(e => e.BookCopies)
				.Where(e => e.WorkId == workId)
				.ToListAsync();

			return editions.Select(e => new BookEditionDto
			{
				EditionId = e.EditionId,
				ISBN = e.ISBN,
				PublisherName = e.Publisher.PublisherName,
				PublishYear = e.PublishYear,
				Language = e.Language,
				Format = e.Format,
				AvailableCopies = e.BookCopies.Count(c => c.CirculationStatus == "Available")
			});
		}

		public async Task<IEnumerable<BookCopyDto>> GetAvailableCopiesByEditionIdAsync(int editionId)
		{
			var copies = await _uow.DbContext.BookCopies
				.Where(c => c.EditionId == editionId && c.CirculationStatus == "Available")
				.ToListAsync();

			return copies.Select(c => new BookCopyDto
			{
				CopyId = c.CopyId,
				Barcode = c.Barcode,
				CirculationStatus = c.CirculationStatus,
				PhysicalCondition = c.PhysicalCondition,
				ShelfLocation = c.ShelfLocation
			});
		}
		// Lấy danh sách Categories
		public async Task<List<Category>> GetAllCategoriesAsync()
		{
			var categories = await _uow.DbContext.Categories.AsNoTracking().ToListAsync();
			return categories.OrderBy(c => c.CategoryName).ToList();
		}

		// Lấy danh sách Authors
		public async Task<List<Author>> GetAllAuthorsAsync()
		{
			var authors = await _uow.DbContext.Authors.AsNoTracking().ToListAsync();
			return authors.OrderBy(a => a.AuthorName).ToList();
		}

		// Lấy danh sách Series
		public async Task<List<Series>> GetAllSeriesAsync()
		{
			var series = await _uow.DbContext.Series.AsNoTracking().ToListAsync();
			return series.OrderBy(s => s.SeriesName).ToList();
		}
		public async Task<(bool IsSuccess, string Message)> AddNewBook(BookWork book, int categoryId, int authorId)
        {
            try
            {
                // 1. Thêm vào bảng chính BookWork
                await _uow.DbContext.BookWorks.AddAsync(book);
                // Lưu để EF nạp WorkId vào object 'book' (nếu cần dùng ID ngay cho bảng trung gian)
                await _uow.DbContext.SaveChangesAsync();

                // 2. Thêm vào bảng trung gian WorkCategory
                var workCategory = new WorkCategory
                {
                    WorkId = book.WorkId,
                    CategoryId = categoryId
                };
                await _uow.DbContext.WorkCategories.AddAsync(workCategory);

                // 3. Thêm vào bảng trung gian WorkAuthor
                var workAuthor = new WorkAuthor
                {
                    WorkId = book.WorkId,
                    AuthorId = authorId
                };
                await _uow.DbContext.WorkAuthors.AddAsync(workAuthor);

                // 4. Commit tất cả thay đổi
                int result = await _uow.DbContext.SaveChangesAsync();

                return result > 0
                    ? (true, "Thêm sách thành công!")
                    : (false, "Lưu thất bại.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi hệ thống: {ex.Message}");
            }
        }

       
    }
}