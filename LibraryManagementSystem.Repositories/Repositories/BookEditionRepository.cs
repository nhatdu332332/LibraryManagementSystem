using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    public class BookEditionRepository : IBookEditionRepository
    {
        private readonly LibraryDbContext _context;

        public BookEditionRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddEditionAsync(BookEdition bookEdition)
        {
            await _context.BookEditions.AddAsync(bookEdition);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<BookEdition>> GetBookEditionAsync()
        {
            return await _context.BookEditions.ToListAsync();
        }

        public async Task<IEnumerable<Publisher>> GetPublishersAsync()
        {
            // Lấy toàn bộ danh sách NXB, sắp xếp theo tên cho dễ tìm
            return await _context.Publishers
                                 .OrderBy(p => p.PublisherName)
                                 .ToListAsync();
        }
        public async Task<List<BookEdition>> GetAllAsync()
        {
            return await _context.BookEditions.ToListAsync();
        }
        public async Task<bool> IsIsbnExistsAsync(string isbn)
        {
            return await _context.BookEditions.AnyAsync(e => e.ISBN == isbn);
        }
    }
}