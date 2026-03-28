using LibraryManagementSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories.Interfaces
{
    public interface IBookEditionRepository
    {
        Task<bool> AddEditionAsync(BookEdition bookEdition);
        Task<List<BookEdition>> GetAllAsync();
        Task<IEnumerable<BookEdition>> GetBookEditionAsync();

        Task<IEnumerable<Publisher>> GetPublishersAsync();
        Task<bool> IsIsbnExistsAsync(string isbn);
    }
}
