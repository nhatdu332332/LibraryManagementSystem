using System;
using System.Threading.Tasks;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.Data.Entities;
using System.Diagnostics;

namespace LibraryManagementSystem.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUnitOfWork _uow;
		private LoginResponseDto? _currentUser;
		public AuthService(IUnitOfWork uow)
		{
			_uow = uow ?? throw new ArgumentNullException(nameof(uow));
			//_uow = uow;
		}
		public bool IsAuthenticated => _currentUser != null;
		public int? CurrentUserId => _currentUser?.UserId;
		public string? CurrentFullName => _currentUser?.FullName;
		public string? CurrentRoleName => _currentUser?.RoleName;
		public string? CurrentAccountType => _currentUser?.AccountType;

		public void SetCurrentUser(LoginResponseDto response)
		{
			if (response == null || !response.Success)
				throw new ArgumentException("Response không hợp lệ");

			_currentUser = response;
		}

		public void Logout()
		{
			_currentUser = null;
		}

		public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
		{
			Debug.WriteLine($"[LOGIN ATTEMPT] Email: {dto.Email}, Type: {dto.AccountType}, Pass length: {dto.Password?.Length ?? 0}");

			// 1. NẾU CHỌN NHÂN VIÊN -> CHỈ TÌM TRONG BẢNG EMPLOYEE
			if (dto.AccountType == "Employee")
			{
				var employee = await _uow.EmployeeRepository.GetByEmailAsync(dto.Email);
				if (employee != null)
				{
					if (employee.PasswordHash == dto.Password)
					{
						Debug.WriteLine($"[EMPLOYEE LOGIN SUCCESS] Email: {dto.Email}");
						var role = await _uow.RoleRepository.GetByIdAsync(employee.RoleId);
						return new LoginResponseDto
						{
							Success = true,
							Message = "Login Employee thành công",
							UserId = employee.EmployeeId,
							AccountType = "Employee",
							FullName = employee.FullName,
							RoleName = role?.RoleName ?? "Unknown"
						};
					}
					return new LoginResponseDto { Success = false, Message = "Mật khẩu không đúng (Nhân viên)" };
				}
				return new LoginResponseDto { Success = false, Message = "Tài khoản Nhân viên không tồn tại" };
			}

			// 2. NẾU CHỌN ĐỘC GIẢ -> CHỈ TÌM TRONG BẢNG READER
			else if (dto.AccountType == "Reader")
			{
				var reader = await _uow.ReaderRepository.GetByEmailAsync(dto.Email);
				if (reader != null)
				{
					if (reader.PasswordHash == dto.Password)
					{
						Debug.WriteLine($"[READER LOGIN SUCCESS] Email: {dto.Email}");
						return new LoginResponseDto
						{
							Success = true,
							Message = "Login Reader thành công",
							UserId = reader.ReaderId,
							AccountType = "Reader",
							FullName = reader.FullName,
							RoleName = "Reader",
							ExpiredDate = reader.ExpiredDate
						};
					}
					return new LoginResponseDto { Success = false, Message = "Mật khẩu không đúng (Độc giả)" };
				}
				return new LoginResponseDto { Success = false, Message = "Tài khoản Độc giả không tồn tại" };
			}

			return new LoginResponseDto { Success = false, Message = "Loại tài khoản không hợp lệ" };
		}

		public async Task<RegisterResponseDto> RegisterReaderAsync(RegisterDto dto)
		{
			var normalizedEmail = AccountValidationHelper.ValidateEmail(dto.Email);
			var normalizedFullName = AccountValidationHelper.ValidateRequiredText(dto.FullName, "Ho ten");
			var normalizedPhone = AccountValidationHelper.ValidatePhoneNumber(dto.PhoneNumber);

			var existingReader = await _uow.ReaderRepository.GetByEmailAsync(normalizedEmail);
			if (existingReader != null)
			{
				throw new InvalidOperationException("Email đã được đăng ký.");
			}

			var reader = new Reader
			{
				Email = normalizedEmail,
				PasswordHash = dto.Password,           // plaintext → lưu thẳng vào cột PasswordHash
				FullName = normalizedFullName,
				PhoneNumber = normalizedPhone,
				Address = dto.Address,
				RegisterDate = DateTime.Now,
				ExpiredDate = dto.ExpiredDate ?? DateTime.Now.AddYears(2),
				ReaderStatus = "Active",
				CardNumber = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()
			};

			await _uow.ReaderRepository.AddAsync(reader);
			await _uow.SaveChangesAsync();

			return new RegisterResponseDto
			{
				ReaderId = reader.ReaderId,
				CardNumber = reader.CardNumber,
				FullName = reader.FullName,
				Email = reader.Email,
				ExpiredDate = reader.ExpiredDate,
				Message = "Đăng ký tài khoản Reader thành công!"
			};
		}

		public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
		{
			var normalizedEmail = AccountValidationHelper.ValidateEmail(dto.Email);
			var normalizedFullName = AccountValidationHelper.ValidateRequiredText(dto.FullName, "Ho ten");

			var existing = await _uow.EmployeeRepository.GetByEmailAsync(normalizedEmail);
			if (existing != null)
			{
				throw new InvalidOperationException("Email đã tồn tại.");
			}

			var role = await _uow.RoleRepository.GetByIdAsync(dto.RoleId);
			if (role == null)
			{
				throw new KeyNotFoundException("Không tìm thấy vai trò.");
			}

			if (AccountValidationHelper.IsAdministratorRoleName(role.RoleName))
			{
				var administrators = await _uow.EmployeeRepository.GetByRoleIdAsync(role.RoleId);
				if (administrators.Any())
				{
					throw new InvalidOperationException("He thong chi cho phep 1 tai khoan Administrator.");
				}
			}

			var employee = new Employee
			{
				Email = normalizedEmail,
				PasswordHash = dto.Password,           // plaintext → lưu thẳng vào cột PasswordHash
				FullName = normalizedFullName,
				RoleId = dto.RoleId,
				HireDate = dto.HireDate ?? DateTime.Now,
				Status = dto.Status ?? "Active"
			};

			await _uow.EmployeeRepository.AddAsync(employee);
			await _uow.SaveChangesAsync();

			return new EmployeeDto
			{
				EmployeeId = employee.EmployeeId,
				Email = employee.Email,
				FullName = employee.FullName,
				RoleName = role.RoleName ?? "Unknown",
				HireDate = employee.HireDate,
				Status = employee.Status
			};
		}
	}
}
