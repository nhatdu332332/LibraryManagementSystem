using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.Implementations
{
    public class BookEditionService : IBookEditionService
    {
        private readonly IBookEditionRepository _editionRepository;

        public BookEditionService(IBookEditionRepository editionRepository)
        {
            _editionRepository = editionRepository;
        }

        public async Task<bool> AddEditionAsync(BookEdition edition)
        {
            // 1. Kiểm tra logic: ISBN không được để trống
            if (string.IsNullOrWhiteSpace(edition.ISBN))
                return false;

            // 2. Kiểm tra trùng ISBN trước khi lưu
            bool isDuplicate = await _editionRepository.IsIsbnExistsAsync(edition.ISBN);
            if (isDuplicate)
            {
                // Bạn có thể quăng Exception ở đây để ViewModel bắt được lỗi cụ thể
                throw new Exception("Mã ISBN này đã tồn tại trong hệ thống!");
            }

            // 3. Gọi Repository để lưu vào DB
            return await _editionRepository.AddEditionAsync(edition);
        }

        public async Task<List<BookEdition>> GetAllAsync()
        {
            return await _editionRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Publisher>> GetPublishersAsync()
        {
            return await _editionRepository.GetPublishersAsync();
        }

        public async Task<bool> IsIsbnDuplicateAsync(string isbn)
        {
            return await _editionRepository.IsIsbnExistsAsync(isbn);
        }
        
    }
}