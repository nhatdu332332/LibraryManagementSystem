using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    public class BookCopyRepository : IBookCopyRepository
    {
        private readonly LibraryDbContext _context;

        public BookCopyRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task AddBookCopyAsync(BookCopy bookCopy)
        {
            _context.BookCopies.Add(bookCopy);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsBarcodeExistAsync(string barcode)
        {
            return await _context.BookCopies
                .AnyAsync(x => x.Barcode == barcode);
        }

        public async Task<BookCopy> GetByIdAsync(int id)
        {
            return await _context.BookCopies
                .Include(bc => bc.BookEdition)
                .FirstOrDefaultAsync(bc => bc.CopyId == id);
        }

        public async Task UpdateAsync(BookCopy bookCopy)
        {
            _context.BookCopies.Update(bookCopy);
            await _context.SaveChangesAsync();
        }
    }
}
