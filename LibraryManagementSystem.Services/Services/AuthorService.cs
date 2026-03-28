using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly LibraryDbContext _context;

        public AuthorService(LibraryDbContext context)
        {
            _context = context;
        }

        // Thêm Author mới với validate
        public async Task<bool> AddAuthorAsync(Author author)
        {
            if (author == null || string.IsNullOrWhiteSpace(author.AuthorName))
                throw new ArgumentException("AuthorName không được rỗng.");

            author.AuthorName = author.AuthorName.Trim();

            var exists = await _context.Authors.AnyAsync(a => a.AuthorName == author.AuthorName);
            if (exists)
                throw new InvalidOperationException("Đã tồn tại Author cùng tên.");

            author.AuthorId = 0;
            _context.Authors.Add(author);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Xóa Author theo ID
        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null) return false;

            _context.Authors.Remove(author);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Lấy tất cả Author
        public async Task<List<Author>> GetAllAuthorAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        // Cập nhật Author với validate
        public async Task<bool> UpdateAuthorAsync(Author author)
        {
            if (author == null || string.IsNullOrWhiteSpace(author.AuthorName))
                throw new ArgumentException("AuthorName không được rỗng.");

            author.AuthorName = author.AuthorName.Trim();

            var existingAuthor = await _context.Authors.FindAsync(author.AuthorId);
            if (existingAuthor == null) return false;

            // Check trùng tên (ngoại trừ chính bản thân)
            var exists = await _context.Authors
                .AnyAsync(a => a.AuthorName == author.AuthorName && a.AuthorId != author.AuthorId);
            if (exists)
                throw new InvalidOperationException("Đã tồn tại Author cùng tên.");

            existingAuthor.AuthorName = author.AuthorName;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        
    }
}