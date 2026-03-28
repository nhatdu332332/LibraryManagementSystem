using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibraryManagementSystem.Data.Entities;

namespace LibraryManagementSystem.Repositories.Interfaces
{
    public interface IBookCopyRepository
    {
        Task AddBookCopyAsync(BookCopy bookCopy);
        Task<bool> IsBarcodeExistAsync(string barcode);
        Task<BookCopy> GetByIdAsync(int id);
        Task UpdateAsync(BookCopy bookCopy);
    }
}
