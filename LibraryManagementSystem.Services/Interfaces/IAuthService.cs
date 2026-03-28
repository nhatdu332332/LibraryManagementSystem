using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;
using LibraryManagementSystem.Services.DTOs;

namespace LibraryManagementSystem.Services.Interfaces
{
	public interface IAuthService
	{
		/// <summary>
		/// Login cho mọi role (Employee hoặc Reader)
		/// </summary>
		Task<LoginResponseDto> LoginAsync(LoginDto dto);
      
        /// <summary>
        /// Đăng ký tài khoản Reader (public)
        /// </summary>
        Task<RegisterResponseDto> RegisterReaderAsync(RegisterDto dto);

		/// <summary>
		/// Admin tạo tài khoản Employee (Staff/Librarian/Admin)
		/// </summary>
		Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);
		bool IsAuthenticated { get; }
		int? CurrentUserId { get; }               // EmployeeId hoặc ReaderId tùy loại
		string? CurrentFullName { get; }
		string? CurrentRoleName { get; }
		string? CurrentAccountType { get; }       // "Employee" hoặc "Reader"

		void SetCurrentUser(LoginResponseDto response);
		void Logout();
	}
}
