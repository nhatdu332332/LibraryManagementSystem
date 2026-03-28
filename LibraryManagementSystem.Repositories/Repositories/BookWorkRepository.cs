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
    public class BookWorkRepository : GenericRepository<BookWork>, IBookWorkRepository
    {
        public BookWorkRepository(LibraryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BookWork>> FindByTitleAsync(string titleKeyword)
        {
            return await _dbSet
                .Where(b => b.Title.ToLower().Contains(titleKeyword.ToLower()))
                .Include(b => b.BookEditions)
                .Include(b => b.WorkAuthors).ThenInclude(wa => wa.Author)
                .Include(b => b.WorkCategories).ThenInclude(wc => wc.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookWork>> FindByAuthorIdAsync(int authorId)
        {
            return await _dbSet
                .Where(b => b.WorkAuthors.Any(wa => wa.AuthorId == authorId))
                .Include(b => b.WorkAuthors).ThenInclude(wa => wa.Author)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookWork>> FindByCategoryIdAsync(int categoryId)
        {
            return await _dbSet
                .Where(b => b.WorkCategories.Any(wc => wc.CategoryId == categoryId))
                .Include(b => b.WorkCategories).ThenInclude(wc => wc.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookWork>> FindBySeriesIdAsync(int seriesId)
        {
            return await _dbSet
                .Where(b => b.SeriesId == seriesId)
                .OrderBy(b => b.VolumeNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookWork>> FindAvailableForBorrowAsync(int minAvailableCopies)
        {
            return await _dbSet
                .Where(b => b.BookEditions.Any(e => e.BookCopies.Count(c => c.CirculationStatus == "Available") >= minAvailableCopies))
                .Include(b => b.BookEditions).ThenInclude(e => e.BookCopies)
                .ToListAsync();
        }

        public async Task AddWorkAuthorAsync(WorkAuthor workAuthor)
        {
            await _context.WorkAuthors.AddAsync(workAuthor);
        }

        public async Task AddWorkCategoryAsync(WorkCategory workCategory)
        {
            await _context.WorkCategories.AddAsync(workCategory);
        }

        public async Task RemoveWorkAuthorsAsync(int workId)
        {
            var workAuthors = await _context.WorkAuthors.Where(wa => wa.WorkId == workId).ToListAsync();
            _context.WorkAuthors.RemoveRange(workAuthors);
        }

        public async Task RemoveWorkCategoriesAsync(int workId)
        {
            var workCategories = await _context.WorkCategories.Where(wc => wc.WorkId == workId).ToListAsync();
            _context.WorkCategories.RemoveRange(workCategories);
        }
        public List<BookWork> GetAll()
        {
            return _context.BookWorks
                .Include(b => b.Series) // JOIN
                .ToList();
        }
        public BookWork GetById(int id)
        {
            return _context.BookWorks.Find(id);
        }
        public List<BookWork> Search(string keyword)
        {
            return _context.BookWorks
                .Include(b => b.Series)
                .Where(b => b.Title.Contains(keyword))
                .ToList();
        }

        public void Add(BookWork b)
        {
            _context.BookWorks.Add(b);
            _context.SaveChanges();
        }

        public void Update(BookWork b)
        {
            _context.BookWorks.Update(b);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var book = _context.BookWorks.Find(id);

            if (book != null)
            {

                var hasBorrow = _context.BorrowTransactionDetails
                    .Join(_context.BookCopies,
                          b => b.CopyId,
                          c => c.CopyId,
                          (b, c) => c.EditionId)
                    .Join(_context.BookEditions,
                          eId => eId,
                          e => e.EditionId,
                          (eId, e) => e.WorkId)
                    .Any(wId => wId == id);

                if (hasBorrow)
                {
                    throw new Exception("Sách này đã từng được mượn, không thể xóa!");
                }

                if (book != null)
                {
                    // 1. WorkAuthor
                    var workAuthors = _context.WorkAuthors.Where(wa => wa.WorkId == id);
                    _context.WorkAuthors.RemoveRange(workAuthors);

                    // 2. WorkCategory
                    var workCategories = _context.WorkCategories.Where(wc => wc.WorkId == id);
                    _context.WorkCategories.RemoveRange(workCategories);

                    // 3. BorrowRequestDetail
                    var borrowDetails = _context.BorrowRequestDetails.Where(b => b.WorkId == id);
                    _context.BorrowRequestDetails.RemoveRange(borrowDetails);

                    // 4. BookEdition + BookCopy
                    var editions = _context.BookEditions.Where(be => be.WorkId == id).ToList();
                    foreach (var edition in editions)
                    {
                        var copies = _context.BookCopies
                            .Where(c => c.EditionId == edition.EditionId)
                            .ToList();

                        foreach (var copy in copies)
                        {
                            // 🔥 XÓA BorrowTransactionDetail TRƯỚC
                            var transactionDetails = _context.BorrowTransactionDetails
                                .Where(b => b.CopyId == copy.CopyId);

                            _context.BorrowTransactionDetails.RemoveRange(transactionDetails);
                        }

                        // 🔥 SAU ĐÓ mới xóa BookCopy
                        _context.BookCopies.RemoveRange(copies);
                    }

                    // 5. Xóa BookWork
                    _context.BookWorks.Remove(book);

                    _context.SaveChanges();
                }
            }
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
        }

        public async Task<List<Series>> GetAllSeriesAsync()
        {
            return await _context.Series.OrderBy(s => s.SeriesName).ToListAsync();
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors.OrderBy(a => a.AuthorName).ToListAsync();
        }

        public async Task<bool> AddBookWorkAsync(BookWork book, System.Collections.Generic.List<int> categoryIds, System.Collections.Generic.List<int> authorIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.BookWorks.Add(book);
                await _context.SaveChangesAsync();

                if (categoryIds != null)
                {
                    foreach (var catId in categoryIds)
                    {
                        _context.WorkCategories.Add(new WorkCategory { WorkId = book.WorkId, CategoryId = catId });
                    }
                }

                if (authorIds != null)
                {
                    foreach (var authId in authorIds)
                    {
                        _context.WorkAuthors.Add(new WorkAuthor { WorkId = book.WorkId, AuthorId = authId });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }

        }
        public async Task<BookWork?> GetBookWorkDetailAsync(int workId)
        {
            return await _context.BookWorks
                .Include(w => w.Series)
                .Include(w => w.WorkAuthors).ThenInclude(wa => wa.Author)
                .Include(w => w.WorkCategories).ThenInclude(wc => wc.Category)
                .Include(w => w.BookEditions)
                .FirstOrDefaultAsync(w => w.WorkId == workId);
        }
    }
}