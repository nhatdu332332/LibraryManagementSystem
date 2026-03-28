using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryDbContext _context;

        public AuthorRepository(LibraryDbContext context)
        {
            _context = context;
        }

        // Thêm Author mới
        public async Task AddAuthorAsync(Author author)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(author?.AuthorName))
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
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu Author vào DB: {ex.Message}", ex);
            }
        }

        

        // Xóa Author theo ID
        public async Task DeleteAuthorAsync(int authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }

        



        // Lấy tất cả Author
        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        

        // Cập nhật Author
        public async Task UpdateAuthorAsync(Author author)
        {
            var existingAuthor = await _context.Authors.FindAsync(author.AuthorId);
            if (existingAuthor != null)
            {
                existingAuthor.AuthorName = author.AuthorName;
                await _context.SaveChangesAsync();
            }
        }

        
    }
}