using LibraryManagementSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IBookCopyService
    {
        Task<bool> AddBookCopyAsync(BookCopy bookCopy);

    }
}
