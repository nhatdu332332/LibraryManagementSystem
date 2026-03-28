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
	public class ReaderAccountService : IReaderAccountService
	{
		private readonly IUnitOfWork _uow;

		public ReaderAccountService(IUnitOfWork uow)
		{
			_uow = uow ?? throw new ArgumentNullException(nameof(uow));
		}

		public async Task<ReaderDto> GetReaderByIdAsync(int readerId)
		{
			_uow.DbContext.ChangeTracker.Clear();
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
				ReaderStatus = reader.ReaderStatus
			};
		}

		public async Task<IEnumerable<ReaderDto>> GetAllReadersAsync()
		{
			_uow.DbContext.ChangeTracker.Clear();
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

		public async Task UpdateReaderAsync(int readerId, UpdateReaderDto dto)
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

			if (dto.FullName != null) reader.FullName = AccountValidationHelper.ValidateRequiredText(dto.FullName, "Ho ten");
			if (dto.PhoneNumber != null) reader.PhoneNumber = AccountValidationHelper.ValidatePhoneNumber(dto.PhoneNumber);
			if (dto.Address != null) reader.Address = dto.Address;
			if (dto.ReaderStatus != null) reader.ReaderStatus = dto.ReaderStatus;
			if (dto.ExpiredDate != null) reader.ExpiredDate = dto.ExpiredDate.Value;

			await _uow.ReaderRepository.UpdateAsync(reader);
			await _uow.SaveChangesAsync();
		}

		public async Task DeleteReaderAsync(int readerId)
		{
			var reader = await _uow.ReaderRepository.GetByIdAsync(readerId);
			if (reader == null) throw new KeyNotFoundException("Không tìm thấy độc giả");

			await _uow.ReaderRepository.DeleteAsync(reader);
			await _uow.SaveChangesAsync();
		}

		public async Task<ReaderDto> CreateReaderAsync(CreateReaderDto dto)
		{
			var normalizedEmail = AccountValidationHelper.ValidateEmail(dto.Email);
			var normalizedFullName = AccountValidationHelper.ValidateRequiredText(dto.FullName, "Ho ten");
			if (string.IsNullOrWhiteSpace(dto.Password))
				throw new ArgumentException("Mat khau khong duoc de trong");

			if (await _uow.ReaderRepository.GetByEmailAsync(normalizedEmail) != null)
				throw new ArgumentException("Email đã tồn tại");

			var reader = new Reader
			{
				Email = normalizedEmail,
				PasswordHash = dto.Password,
				FullName = normalizedFullName,
				PhoneNumber = AccountValidationHelper.ValidatePhoneNumber(dto.PhoneNumber),
				Address = dto.Address,
				RegisterDate = DateTime.Now,
				ExpiredDate = dto.ExpiredDate ?? DateTime.Now.AddYears(2),
				ReaderStatus = "Active",
				CardNumber = "RD" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()
			};

			await _uow.ReaderRepository.AddAsync(reader);
			await _uow.SaveChangesAsync();

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

		public async Task<bool> Register(string email, string password, string fullName)
		{
			if (await _uow.ReaderRepository.GetByEmailAsync(email) != null)
				return false;

			var reader = new Reader
			{
				CardNumber = "RD" + new Random().Next(100000, 999999),
				Email = email,
				PasswordHash = password,
				FullName = fullName,
				RegisterDate = DateTime.Now,
				ExpiredDate = DateTime.Now.AddYears(2),
				ReaderStatus = "Active"
			};

			await _uow.ReaderRepository.AddAsync(reader);
			await _uow.SaveChangesAsync();
			return true;
		}

		public async Task ChangeReaderPasswordAsync(int readerId, string currentPassword, string newPassword)
		{
			var reader = await _uow.ReaderRepository.GetByIdAsync(readerId);
			if (reader == null) throw new KeyNotFoundException("Không tìm thấy độc giả");

			if (reader.PasswordHash != currentPassword)
				throw new InvalidOperationException("Mật khẩu hiện tại không đúng");

			reader.PasswordHash = newPassword;

			await _uow.ReaderRepository.UpdateAsync(reader);
			await _uow.SaveChangesAsync();
		}

		public async Task ResetReaderPasswordAsync(int readerId, string newPassword)
		{
			var reader = await _uow.ReaderRepository.GetByIdAsync(readerId);
			if (reader == null) throw new KeyNotFoundException("Không tìm thấy độc giả");

			reader.PasswordHash = newPassword;

			await _uow.ReaderRepository.UpdateAsync(reader);
			await _uow.SaveChangesAsync();
		}
	}
}
