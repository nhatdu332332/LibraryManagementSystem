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
	public class EmployeeService : IEmployeeService
	{
		private readonly IUnitOfWork _uow;

		public EmployeeService(IUnitOfWork uow)
		{
			_uow = uow ?? throw new ArgumentNullException(nameof(uow));
		}

		public async Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId)
		{
			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			return new EmployeeDto
			{
				EmployeeId = employee.EmployeeId,
				Email = employee.Email,
				FullName = employee.FullName,
				RoleName = (await _uow.RoleRepository.GetByIdAsync(employee.RoleId))?.RoleName ?? "Unknown",
				HireDate = employee.HireDate,
				Status = employee.Status,
				ProcessedTransactionsCount = await GetProcessedTransactionsCount(employeeId)
			};
		}

		public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
		{
			var employees = await _uow.EmployeeRepository.GetAllAsync();
			var employeeDtos = await Task.WhenAll(employees.Select(async e => new EmployeeDto
			{
				EmployeeId = e.EmployeeId,
				Email = e.Email,
				FullName = e.FullName,
				RoleName = (await _uow.RoleRepository.GetByIdAsync(e.RoleId))?.RoleName ?? "Unknown",
				HireDate = e.HireDate,
				Status = e.Status
			}));

			return employeeDtos;
		}

		public async Task UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto dto, int currentUserId, string currentRoleName)
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

			// Kiểm tra quyền
			if (currentRoleName == "Employee")
			{
				if (currentUserId != employeeId)
					throw new UnauthorizedAccessException("Employee chỉ được update profile bản thân");

				// Employee chỉ được update FullName và Status (không được đổi RoleId)
				if (dto.FullName != null) employee.FullName = AccountValidationHelper.ValidateRequiredText(dto.FullName, "Ho ten");
				if (dto.Status != null) employee.Status = dto.Status;

				// Không cho Employee đổi RoleId
				if (dto.RoleId != null)
					throw new UnauthorizedAccessException("Employee không có quyền thay đổi Role");
			}
			else if (currentRoleName == "Administrator")
			{
				// Administrator được update đầy đủ
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
					if (isAdministrator && dto.RoleId.Value != employee.RoleId)
					{
						throw new InvalidOperationException("Tai khoan Administrator khong duoc thay doi vai tro.");
					}

					if (administratorRoleId.HasValue &&
						dto.RoleId.Value == administratorRoleId.Value &&
						employee.RoleId != administratorRoleId.Value)
					{
						var administrators = await _uow.EmployeeRepository.GetByRoleIdAsync(administratorRoleId.Value);
						if (administrators.Any())
						{
							throw new InvalidOperationException("He thong chi cho phep 1 tai khoan Administrator.");
						}
					}

					employee.RoleId = dto.RoleId.Value;
				}
			}
			else
			{
				throw new UnauthorizedAccessException("Không có quyền update Employee");
			}

			await _uow.EmployeeRepository.UpdateAsync(employee);
			await _uow.SaveChangesAsync();
		}

		public async Task DeleteEmployeeAsync(int employeeId, int currentUserId, string currentRoleName)
		{
			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			// Chỉ Administrator được xóa
			if (currentRoleName != "Administrator")
				throw new UnauthorizedAccessException("Chỉ Administrator được xóa nhân viên");

			var administratorRole = await _uow.RoleRepository.GetByNameAsync("Administrator");
			if (administratorRole != null && employee.RoleId == administratorRole.RoleId)
			{
				throw new InvalidOperationException("Khong duoc xoa tai khoan Administrator.");
			}

			await _uow.EmployeeRepository.DeleteAsync(employee);
			await _uow.SaveChangesAsync();
		}

		public async Task ChangeEmployeeRoleAsync(int employeeId, int newRoleId, int currentUserId, string currentRoleName)
		{
			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			// Chỉ Administrator được đổi role
			if (currentRoleName != "Administrator")
				throw new UnauthorizedAccessException("Chỉ Administrator được thay đổi role nhân viên");

			employee.RoleId = newRoleId;

			await _uow.EmployeeRepository.UpdateAsync(employee);
			await _uow.SaveChangesAsync();
		}

		public async Task<EmployeeDto?> FindByEmailAsync(string email)
		{
			var employee = await _uow.EmployeeRepository.GetByEmailAsync(email);
			if (employee == null) return null;

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

		public async Task<IEnumerable<EmployeeDto>> GetByRoleNameAsync(string roleName)
		{
			var role = await _uow.RoleRepository.GetByNameAsync(roleName);
			if (role == null) return Enumerable.Empty<EmployeeDto>();

			var employees = await _uow.EmployeeRepository.GetByRoleIdAsync(role.RoleId);
			return employees.Select(e => new EmployeeDto
			{
				EmployeeId = e.EmployeeId,
				Email = e.Email,
				FullName = e.FullName,
				RoleName = roleName,
				HireDate = e.HireDate,
				Status = e.Status
			});
		}

		private async Task<int> GetProcessedTransactionsCount(int employeeId)
		{
			var transactions = await _uow.BorrowTransactionRepository.GetByEmployeeIdAsync(employeeId);
			return transactions.Count();
		}
	}
}
