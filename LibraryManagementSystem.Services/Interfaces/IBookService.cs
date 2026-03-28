using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.Interfaces
{
	public interface IBookService
	{
		Task<BookWorkDto> GetBookWorkByIdAsync(int workId);
		Task<IEnumerable<BookWorkDto>> GetAllBookWorksAsync();
		Task<BookWorkDto> CreateBookWorkAsync(CreateBookWorkDto dto);
		Task UpdateBookWorkAsync(UpdateBookWorkDto dto, int workId);
		Task DeleteBookWorkAsync(int workId);


		Task<IEnumerable<BookWorkDto>> SearchBooksAsync(string keyword, int? authorId, int? categoryId, int? seriesId);
		Task<IEnumerable<BookEditionDto>> GetEditionsByWorkIdAsync(int workId);
		Task<IEnumerable<BookCopyDto>> GetAvailableCopiesByEditionIdAsync(int editionId);


		// Lấy danh sách Categories
		Task<List<LibraryManagementSystem.Data.Entities.Category>> GetAllCategoriesAsync();

		// Lấy danh sách Authors
		Task<List<LibraryManagementSystem.Data.Entities.Author>> GetAllAuthorsAsync();

		// Lấy danh sách Series
		Task<List<LibraryManagementSystem.Data.Entities.Series>> GetAllSeriesAsync();

		// Thêm tác phẩm (BookWork) mới kèm liên kết Author/Category
		Task<(bool IsSuccess, string Message)> AddNewBook(LibraryManagementSystem.Data.Entities.BookWork book, int categoryId, int authorId);

        Task<BookWork?> GetBookWorkDetailAsync(int workId);

    }
}
