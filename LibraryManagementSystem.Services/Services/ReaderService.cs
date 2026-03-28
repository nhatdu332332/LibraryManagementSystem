using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;

namespace LibraryManagementSystem.Services
{
	public class ReaderService : IReaderService
	{
		private readonly IUnitOfWork _uow;

		public ReaderService(IUnitOfWork uow)
		{
			_uow = uow ?? throw new ArgumentNullException(nameof(uow));
		}

		public async Task<ReaderDto> GetReaderByIdAsync(int readerId)
		{
			var reader = await _uow.ReaderRepository.GetByIdAsync(readerId);
			if (reader == null) throw new KeyNotFoundException("Không tìm thấy độc giả");

			return new ReaderDto
			{
				ReaderId = reader.ReaderId,
				CardNumber = reader.CardNumber,
				Email = reader.Email,
				FullName = reader.FullName,
				PhoneNumber = reader.PhoneNumber,
				Address = reader.Address,
				RegisterDate = reader.RegisterDate,
				ExpiredDate = reader.ExpiredDate,
				ReaderStatus = reader.ReaderStatus,
				BorrowedBooksCount = await GetBorrowedBooksCount(readerId),
				PendingRequestsCount = await GetPendingRequestsCount(readerId)
			};
		}

		public async Task<IEnumerable<ReaderDto>> GetAllReadersAsync()
		{
			var readers = await _uow.ReaderRepository.GetAllAsync();
			return readers.Select(r => new ReaderDto
			{
				ReaderId = r.ReaderId,
				CardNumber = r.CardNumber,
				Email = r.Email,
				FullName = r.FullName,
				PhoneNumber = r.PhoneNumber,
				Address = r.Address,
				RegisterDate = r.RegisterDate,
				ExpiredDate = r.ExpiredDate,
				ReaderStatus = r.ReaderStatus
			});
		}

		public async Task UpdateReaderAsync(int readerId, UpdateReaderDto dto, int currentUserId, string currentRoleName)
		{
			var reader = await _uow.ReaderRepository.GetByIdAsync(readerId);
			if (reader == null) throw new KeyNotFoundException("Không tìm thấy độc giả");

			if (dto.Email != null)
			{
				var normalizedEmail = AccountValidationHelper.ValidateEmail(dto.Email);
				var existingReader = await _uow.ReaderRepository.GetByEmailAsync(normalizedEmail);
				if (existingReader != null && existingReader.ReaderId != readerId)
				{
					throw new ArgumentException("Email đã tồn tại");
				}

				reader.Email = normalizedEmail;
			}

			// Kiểm tra quyền
			if (currentRoleName == "Reader")
			{
				if (currentUserId != readerId)
					throw new UnauthorizedAccessException("Reader chỉ được update profile bản thân");

				// Reader chỉ được update FullName, PhoneNumber, Address
				if (dto.FullName != null) reader.FullName = AccountValidationHelper.ValidateRequiredText(dto.FullName, "Ho ten");
				if (dto.PhoneNumber != null) reader.PhoneNumber = AccountValidationHelper.ValidatePhoneNumber(dto.PhoneNumber);
				if (dto.Address != null) reader.Address = dto.Address;

				// Không cho Reader update ExpiredDate hoặc ReaderStatus
				if (dto.ExpiredDate != null || dto.ReaderStatus != null)
					throw new UnauthorizedAccessException("Reader không có quyền update ExpiredDate hoặc ReaderStatus");
			}
			else if (currentRoleName == "Staff" || currentRoleName == "Librarian" || currentRoleName == "Administrator")
			{
				// Staff/Librarian/Admin được update đầy đủ
				if (dto.FullName != null) reader.FullName = AccountValidationHelper.ValidateRequiredText(dto.FullName, "Ho ten");
				if (dto.PhoneNumber != null) reader.PhoneNumber = AccountValidationHelper.ValidatePhoneNumber(dto.PhoneNumber);
				if (dto.Address != null) reader.Address = dto.Address;
				if (dto.ReaderStatus != null) reader.ReaderStatus = dto.ReaderStatus;
				if (dto.ExpiredDate != null) reader.ExpiredDate = dto.ExpiredDate.Value;
			}
			else
			{
				throw new UnauthorizedAccessException("Không có quyền update Reader");
			}

			await _uow.ReaderRepository.UpdateAsync(reader);
			await _uow.SaveChangesAsync();
		}

		public async Task DeleteReaderAsync(int readerId, int currentUserId, string currentRoleName)
		{
			var reader = await _uow.ReaderRepository.GetByIdAsync(readerId);
			if (reader == null) throw new KeyNotFoundException("Không tìm thấy độc giả");

			// Chỉ Staff/Librarian/Admin được xóa
			if (currentRoleName != "Staff" && currentRoleName != "Librarian" && currentRoleName != "Administrator")
				throw new UnauthorizedAccessException("Không có quyền xóa độc giả");

			await _uow.ReaderRepository.DeleteAsync(reader);
			await _uow.SaveChangesAsync();
		}

		public async Task RenewReaderCardAsync(int readerId, DateTime newExpiredDate, int currentUserId, string currentRoleName)
		{
			var reader = await _uow.ReaderRepository.GetByIdAsync(readerId);
			if (reader == null) throw new KeyNotFoundException("Không tìm thấy độc giả");

			// Chỉ Staff/Librarian/Admin được renew
			if (currentRoleName != "Staff" && currentRoleName != "Librarian" && currentRoleName != "Administrator")
				throw new UnauthorizedAccessException("Không có quyền gia hạn thẻ độc giả");

			reader.ExpiredDate = newExpiredDate;
			reader.ReaderStatus = "Active";

			await _uow.ReaderRepository.UpdateAsync(reader);
			await _uow.SaveChangesAsync();
		}

		public async Task<ReaderDto?> FindByCardNumberOrEmailAsync(string cardNumberOrEmail)
		{
			var reader = await _uow.ReaderRepository.GetByCardNumberAsync(cardNumberOrEmail);
			if (reader == null)
				reader = await _uow.ReaderRepository.GetByEmailAsync(cardNumberOrEmail);

			if (reader == null) return null;

			return new ReaderDto
			{
				ReaderId = reader.ReaderId,
				CardNumber = reader.CardNumber,
				Email = reader.Email,
				FullName = reader.FullName,
				PhoneNumber = reader.PhoneNumber,
				Address = reader.Address,
				RegisterDate = reader.RegisterDate,
				ExpiredDate = reader.ExpiredDate,
				ReaderStatus = reader.ReaderStatus
			};
		}

		// Helper methods (không cần quyền, dùng nội bộ)
		private async Task<int> GetBorrowedBooksCount(int readerId)
		{
			var transactions = await _uow.BorrowTransactionRepository.GetByReaderIdAsync(readerId);
			return transactions.Sum(t => t.Details.Count(d => d.ReturnDate == null));
		}

		private async Task<int> GetPendingRequestsCount(int readerId)
		{
			var requests = await _uow.BorrowRequestRepository.GetByReaderIdAsync(readerId);
			return requests.Count(r => r.Status == "Pending");
		}
	}
}
