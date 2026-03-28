using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Data.Entities;

namespace LibraryManagementSystem.Repositories.Interfaces
{
	public interface IReaderRepository : IGenericRepository<Reader>
	{
		Task<Reader> GetByCardNumberAsync(string cardNumber);
		Task<Reader> GetByEmailAsync(string email);
	}
}
