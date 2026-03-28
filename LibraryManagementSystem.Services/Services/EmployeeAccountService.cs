using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	public class EmployeeAccountService : IEmployeeAccountService
	{
		private readonly IUnitOfWork _uow;

		public EmployeeAccountService(IUnitOfWork uow)
		{
			_uow = uow ?? throw new ArgumentNullException(nameof(uow));
		}

		public async Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId)
		{
			_uow.DbContext.ChangeTracker.Clear();
			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			return new EmployeeDto
			{
				EmployeeId = employee.EmployeeId,
				Email = employee.Email,
				FullName = employee.FullName,
				RoleName = (await _uow.RoleRepository.GetByIdAsync(employee.RoleId))?.RoleName ?? "Unknown",
				HireDate = employee.HireDate,
				Status = employee.Status
			};
		}

		public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
		{
			_uow.DbContext.ChangeTracker.Clear();
			var employees = await _uow.EmployeeRepository.GetAllAsync();
			var result = new List<EmployeeDto>();

			foreach (var e in employees)
			{
				var role = await _uow.RoleRepository.GetByIdAsync(e.RoleId);
				result.Add(new EmployeeDto
				{
					EmployeeId = e.EmployeeId,
					Email = e.Email,
					FullName = e.FullName,
					RoleName = role?.RoleName ?? "Unknown",
					HireDate = e.HireDate,
					Status = e.Status
				});
			}

			return result;
		}

		public async Task UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto dto)
		{
			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			var administratorRole = await _uow.RoleRepository.GetByNameAsync("Administrator");
			var administratorRoleId = administratorRole?.RoleId;
			var isAdministrator = administratorRoleId.HasValue && employee.RoleId == administratorRoleId.Value;

			if (dto.Email != null)
			{
				var normalizedEmail = AccountValidationHelper.ValidateEmail(dto.Email);
				var existingEmployee = await _uow.EmployeeRepository.GetByEmailAsync(normalizedEmail);
				if (existingEmployee != null && existingEmployee.EmployeeId != employeeId)
				{
					throw new ArgumentException("Email đã tồn tại");
				}

				employee.Email = normalizedEmail;
			}

			if (dto.FullName != null) employee.FullName = AccountValidationHelper.ValidateRequiredText(dto.FullName, "Ho ten");

			if (dto.Status != null)
			{
				var normalizedStatus = dto.Status.Trim();
				if (isAdministrator && !normalizedStatus.Equals(employee.Status, StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOperationException("Tai khoan Administrator khong duoc thay doi trang thai.");
				}

				employee.Status = normalizedStatus;
			}

			if (dto.RoleId != null)
			{
				var role = await _uow.RoleRepository.GetByIdAsync(dto.RoleId.Value);
				if (role == null) throw new KeyNotFoundException("Không tìm thấy vai trò");

				if (isAdministrator && dto.RoleId.Value != employee.RoleId)
				{
					throw new InvalidOperationException("Tai khoan Administrator khong duoc thay doi vai tro.");
				}

				if (administratorRoleId.HasValue &&
					dto.RoleId.Value == administratorRoleId.Value &&
					employee.RoleId != administratorRoleId.Value &&
					await HasAnotherAdministratorAsync())
				{
					throw new InvalidOperationException("He thong chi cho phep 1 tai khoan Administrator.");
				}

				employee.RoleId = dto.RoleId.Value;
			}

			await _uow.EmployeeRepository.UpdateAsync(employee);
			await _uow.SaveChangesAsync();
		}

		public async Task DeleteEmployeeAsync(int employeeId)
		{
			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			if (await IsAdministratorAsync(employee))
			{
				throw new InvalidOperationException("Khong duoc xoa tai khoan Administrator.");
			}

			await _uow.EmployeeRepository.DeleteAsync(employee);
			await _uow.SaveChangesAsync();
		}

		public async Task ChangeEmployeeRoleAsync(int employeeId, int newRoleId)
		{
			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			var role = await _uow.RoleRepository.GetByIdAsync(newRoleId);
			if (role == null) throw new KeyNotFoundException("Không tìm thấy vai trò");

			if (await IsAdministratorAsync(employee) && employee.RoleId != newRoleId)
			{
				throw new InvalidOperationException("Tai khoan Administrator khong duoc thay doi vai tro.");
			}

			if (AccountValidationHelper.IsAdministratorRoleName(role.RoleName) &&
				!await IsAdministratorAsync(employee) &&
				await HasAnotherAdministratorAsync())
			{
				throw new InvalidOperationException("He thong chi cho phep 1 tai khoan Administrator.");
			}

			employee.RoleId = newRoleId;
			await _uow.EmployeeRepository.UpdateAsync(employee);
			await _uow.SaveChangesAsync();
		}

		public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
		{
			var normalizedEmail = AccountValidationHelper.ValidateEmail(dto.Email);
			var normalizedFullName = AccountValidationHelper.ValidateRequiredText(dto.FullName, "Ho ten");
			if (string.IsNullOrWhiteSpace(dto.Password))
				throw new ArgumentException("Mat khau khong duoc de trong");

			if (await _uow.EmployeeRepository.GetByEmailAsync(normalizedEmail) != null)
				throw new ArgumentException("Email đã tồn tại");

			var role = await _uow.RoleRepository.GetByIdAsync(dto.RoleId);
			if (role == null) throw new KeyNotFoundException("Không tìm thấy vai trò");

			if (AccountValidationHelper.IsAdministratorRoleName(role.RoleName) && await HasAnotherAdministratorAsync())
			{
				throw new InvalidOperationException("He thong chi cho phep 1 tai khoan Administrator.");
			}

			var employee = new Employee
			{
				Email = normalizedEmail,
				PasswordHash = dto.Password,
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

		public async Task ChangeEmployeePasswordAsync(int employeeId, string currentPassword, string newPassword)
		{
			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			if (employee.PasswordHash != currentPassword)
				throw new InvalidOperationException("Mật khẩu hiện tại không đúng");

			employee.PasswordHash = newPassword;

			await _uow.EmployeeRepository.UpdateAsync(employee);
			await _uow.SaveChangesAsync();
		}

		public async Task ResetEmployeePasswordAsync(int employeeId, string newPassword)
		{
			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			employee.PasswordHash = newPassword;

			await _uow.EmployeeRepository.UpdateAsync(employee);
			await _uow.SaveChangesAsync();
		}

		private async Task<bool> IsAdministratorAsync(Employee employee)
		{
			var administratorRole = await _uow.RoleRepository.GetByNameAsync("Administrator");
			return administratorRole != null && employee.RoleId == administratorRole.RoleId;
		}

		private async Task<bool> HasAnotherAdministratorAsync()
		{
			var administratorRole = await _uow.RoleRepository.GetByNameAsync("Administrator");
			if (administratorRole == null)
			{
				return false;
			}

			var administrators = await _uow.EmployeeRepository.GetByRoleIdAsync(administratorRole.RoleId);
			return administrators.Any();
		}
	}
}
