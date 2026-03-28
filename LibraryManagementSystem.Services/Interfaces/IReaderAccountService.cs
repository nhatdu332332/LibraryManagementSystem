using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagementSystem.Services.DTOs;

namespace LibraryManagementSystem.Services.Interfaces
{
	public interface IReaderAccountService
	{
		Task<ReaderDto> GetReaderByIdAsync(int readerId);
		Task<IEnumerable<ReaderDto>> GetAllReadersAsync();
		Task UpdateReaderAsync(int readerId, UpdateReaderDto dto);
		Task DeleteReaderAsync(int readerId);
		Task<ReaderDto> CreateReaderAsync(CreateReaderDto dto);

		Task<bool> Register(string email, string password, string fullName);

		Task ChangeReaderPasswordAsync(int readerId, string currentPassword, string newPassword);
		Task ResetReaderPasswordAsync(int readerId, string newPassword);
	}
}