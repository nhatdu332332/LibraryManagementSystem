using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using LibraryManagementSystem.Data.Entities;
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
	public class RoleService : IRoleService
	{
		private readonly IUnitOfWork _uow;

		public RoleService(IUnitOfWork uow)
		{
			_uow = uow ?? throw new ArgumentNullException(nameof(uow));
		}

		public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
		{
			var roles = await _uow.RoleRepository.GetAllAsync();
			var roleDtos = new List<RoleDto>();

			foreach (var role in roles)
			{
				var count = await GetEmployeeCountForRole(role.RoleId);
				roleDtos.Add(new RoleDto
				{
					RoleId = role.RoleId,
					RoleName = role.RoleName,
					Description = role.Description ?? string.Empty,
					EmployeeCount = count
				});
			}

			return roleDtos;
		}

		public async Task<RoleDto> GetRoleByIdAsync(int roleId)
		{
			var role = await _uow.RoleRepository.GetByIdAsync(roleId);
			if (role == null) throw new KeyNotFoundException("Không tìm thấy role");

			return new RoleDto
			{
				RoleId = role.RoleId,
				RoleName = role.RoleName,
				Description = role.Description ?? string.Empty,
				EmployeeCount = await GetEmployeeCountForRole(role.RoleId)
			};
		}

		public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto, int currentUserId, string currentRoleName)
		{
			// Kiểm tra quyền: chỉ Administrator được tạo role
			if (currentRoleName != "Administrator")
				throw new UnauthorizedAccessException("Chỉ Administrator được tạo role mới");

			if (string.IsNullOrWhiteSpace(dto.RoleName))
				throw new ArgumentException("RoleName không được để trống");

			if (await _uow.RoleRepository.GetByNameAsync(dto.RoleName) != null)
				throw new ArgumentException("RoleName đã tồn tại");

			var role = new Role
			{
				RoleName = dto.RoleName,
				Description = dto.Description
			};

			await _uow.RoleRepository.AddAsync(role);
			await _uow.SaveChangesAsync();

			return new RoleDto
			{
				RoleId = role.RoleId,
				RoleName = role.RoleName,
				Description = role.Description ?? string.Empty,
				EmployeeCount = 0
			};
		}

		public async Task UpdateRoleAsync(int roleId, UpdateRoleDto dto, int currentUserId, string currentRoleName)
		{
			var role = await _uow.RoleRepository.GetByIdAsync(roleId);
			if (role == null) throw new KeyNotFoundException("Không tìm thấy role");

			// Kiểm tra quyền: chỉ Administrator được update role
			if (currentRoleName != "Administrator")
				throw new UnauthorizedAccessException("Chỉ Administrator được update role");

			if (dto.RoleName != null)
			{
				if (await _uow.RoleRepository.GetByNameAsync(dto.RoleName) != null && dto.RoleName != role.RoleName)
					throw new ArgumentException("RoleName đã tồn tại");

				role.RoleName = dto.RoleName;
			}

			if (dto.Description != null)
				role.Description = dto.Description;

			await _uow.RoleRepository.UpdateAsync(role);
			await _uow.SaveChangesAsync();
		}

		public async Task DeleteRoleAsync(int roleId, int currentUserId, string currentRoleName)
		{
			var role = await _uow.RoleRepository.GetByIdAsync(roleId);
			if (role == null) throw new KeyNotFoundException("Không tìm thấy role");

			// Kiểm tra quyền: chỉ Administrator được xóa role
			if (currentRoleName != "Administrator")
				throw new UnauthorizedAccessException("Chỉ Administrator được xóa role");

			// Kiểm tra không có Employee nào dùng role này
			var employees = await _uow.EmployeeRepository.GetByRoleIdAsync(roleId);
			if (employees.Any())
				throw new InvalidOperationException("Không thể xóa role đang được sử dụng bởi nhân viên");

			await _uow.RoleRepository.DeleteAsync(role);
			await _uow.SaveChangesAsync();
		}

		public async Task<RoleDto?> GetByNameAsync(string roleName)
		{
			var role = await _uow.RoleRepository.GetByNameAsync(roleName);
			if (role == null) return null;

			return new RoleDto
			{
				RoleId = role.RoleId,
				RoleName = role.RoleName,
				Description = role.Description ?? string.Empty,
				EmployeeCount = await GetEmployeeCountForRole(role.RoleId)
			};
		}

		private async Task<int> GetEmployeeCountForRole(int roleId)
		{
			var employees = await _uow.EmployeeRepository.GetByRoleIdAsync(roleId);
			return employees.Count();
		}
	}
}
