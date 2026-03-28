using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories.Interfaces
{
	public interface IBookWorkRepository : IGenericRepository<BookWork>
	{
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Series>> GetAllSeriesAsync();
        Task<List<Author>> GetAllAuthorsAsync();
        // Add BookWork together with multiple categories and authors
		Task<bool> AddBookWorkAsync(BookWork book, System.Collections.Generic.List<int> categoryIds, System.Collections.Generic.List<int> authorIds);
        /// <summary>
        /// Tìm tất cả BookWork theo tiêu đề (title) chứa từ khóa (không phân biệt hoa thường)
        /// </summary>
        Task<IEnumerable<BookWork>> FindByTitleAsync(string titleKeyword);

		/// <summary>
		/// Tìm tất cả BookWork của một tác giả cụ thể (qua AuthorId)
		/// </summary>
		Task<IEnumerable<BookWork>> FindByAuthorIdAsync(int authorId);

		/// <summary>
		/// Tìm tất cả BookWork thuộc một category cụ thể (qua CategoryId)
		/// </summary>
		Task<IEnumerable<BookWork>> FindByCategoryIdAsync(int categoryId);

		/// <summary>
		/// Tìm BookWork theo SeriesId (thuộc một series cụ thể) và VolumeNumber
		/// </summary>
		Task<IEnumerable<BookWork>> FindBySeriesIdAsync(int seriesId);

		/// <summary>
		/// Tìm BookWork có số lượng bản sao (Copies) còn available >= số lượng yêu cầu
		/// (dùng cho việc kiểm tra khi duyệt request mượn)
		/// </summary>
		Task<IEnumerable<BookWork>> FindAvailableForBorrowAsync(int minAvailableCopies);
		// Thêm method cho junction table (WorkAuthor, WorkCategory)
		Task AddWorkAuthorAsync(WorkAuthor workAuthor);
		Task AddWorkCategoryAsync(WorkCategory workCategory);
		Task RemoveWorkAuthorsAsync(int workId);
		Task RemoveWorkCategoriesAsync(int workId);
        List<BookWork> GetAll();
        List<BookWork> Search(string keyword);
        void Add(BookWork b);
        void Update(BookWork b);
        void Delete(int id);
        BookWork GetById(int id);

        Task<BookWork?> GetBookWorkDetailAsync(int workId);

    }
}
