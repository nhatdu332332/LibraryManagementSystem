using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly LibraryDbContext _context;
        public CategoryRepository(LibraryDbContext context)
        {
            _context = context;
        }


        public async Task AddCategoryAsync(Category category)
            {
                if (category == null) throw new ArgumentNullException(nameof(category));
                if (string.IsNullOrWhiteSpace(category.CategoryName))
                    throw new ArgumentException("CategoryName không được rỗng.");

                category.CategoryName = category.CategoryName.Trim();

                // Kiểm tra trùng
                var exists = await _context.Categories.AnyAsync(c => c.CategoryName == category.CategoryName);
                if (exists) throw new InvalidOperationException("Đã tồn tại Category cùng tên.");

                category.CategoryId = 0;
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }


		public async Task DeleteCategoryAsync(int CategoryId)
		{
			var category = await _context.Categories.FindAsync(CategoryId);
			if (category == null)
			{
				throw new InvalidOperationException("Category không tồn tại.");
			}

			// KIỂM TRA XEM CÓ SÁCH NÀO ĐANG DÙNG CATEGORY NÀY KHÔNG
			bool isInUse = await _context.WorkCategories
										 .AnyAsync(wc => wc.CategoryId == CategoryId);

			if (isInUse)
			{
				throw new InvalidOperationException(
					"Không thể xóa thể loại này vì đang được sử dụng bởi một hoặc nhiều tác phẩm sách.\n" +
					"Hãy gỡ thể loại này khỏi các sách trước khi xóa.");
			}

			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Category>> GetAllCategoryAsync()
        {
            return await  _context.Categories.ToListAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            var CategoryToUpdate = await _context.Categories.FindAsync(category.CategoryId);

            if (CategoryToUpdate != null)
            {
                if (!string.IsNullOrWhiteSpace(category.CategoryName))
                    CategoryToUpdate.CategoryName = category.CategoryName.Trim();

                CategoryToUpdate.Description = category.Description;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Category không tồn tại để cập nhật.");
            }
        }
    }
}
