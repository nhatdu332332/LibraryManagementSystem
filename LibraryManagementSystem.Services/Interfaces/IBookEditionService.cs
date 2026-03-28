using LibraryManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IBookEditionService
    {
        // Thêm mới Edition kèm theo kiểm tra logic
        Task<bool> AddEditionAsync(BookEdition edition);

        // Lấy danh sách NXB để hiển thị lên ComboBox
        Task<IEnumerable<Publisher>> GetPublishersAsync();

        // Kiểm tra xem ISBN đã bị trùng chưa
        Task<bool> IsIsbnDuplicateAsync(string isbn);

        Task<List<BookEdition>> GetAllAsync();

    }
}