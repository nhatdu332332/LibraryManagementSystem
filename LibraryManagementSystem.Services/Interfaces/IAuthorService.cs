using LibraryManagementSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IAuthorService
    {

        Task<List<Author>> GetAllAuthorAsync();
        Task<bool> AddAuthorAsync(Author Author);
        Task<bool> UpdateAuthorAsync(Author Author);
        Task<bool> DeleteAuthorAsync(int AuthorID);
    }
}
