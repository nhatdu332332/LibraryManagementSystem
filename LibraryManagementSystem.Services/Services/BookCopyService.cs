using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LibraryManagementSystem.Services.Services
{
    public class BookCopyService : IBookCopyService
    {
        private readonly IBookCopyRepository _repo;

        public BookCopyService(IBookCopyRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> AddBookCopyAsync(BookCopy bookCopy)
        {
            // check barcode unique
            if (await _repo.IsBarcodeExistAsync(bookCopy.Barcode))
                return false;

            await _repo.AddBookCopyAsync(bookCopy);
            return true;
        }
    }
}
