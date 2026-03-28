// Full code cho LoginViewModel.cs (thêm MessageBox debug nếu login fail)
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;

namespace LibraryManagementSystem.WPF.ViewModels
{
	public class LoginViewModel : ObservableObject
	{
		private readonly IAuthService _authService;
        
        private string _email = string.Empty;
		private string _statusMessage = string.Empty;
		private bool _isReaderLogin = true;

		private bool _loginSuccessTriggered;
		private string _loginSuccessFullName = string.Empty;
		private int _loginSuccessUserId;
		private string _loginSuccessAccountType = string.Empty;
		private string _loginSuccessRoleName = string.Empty;
		private string _registerPhone = string.Empty;
		private string _registerEmail = string.Empty;
		public string RegisterEmail
		{
			get => _registerEmail;
			set => SetProperty(ref _registerEmail, value);
		}

		private string _registerPassword = string.Empty;
		public string RegisterPassword
		{
			get => _registerPassword;
			set => SetProperty(ref _registerPassword, value);
		}

		private string _registerFullName = string.Empty;
		public string RegisterFullName
		{
			get => _registerFullName;
			set => SetProperty(ref _registerFullName, value);
		}
		public string RegisterPhone
		{
			get => _registerPhone;
			set => SetProperty(ref _registerPhone, value);
		}

		private string _registerAddress = string.Empty;
		public string RegisterAddress
		{
			get => _registerAddress;
			set => SetProperty(ref _registerAddress, value);
		}
		public string Email
		{
			get => _email;
			set => SetProperty(ref _email, value);
		}

		public string StatusMessage
		{
			get => _statusMessage;
			set => SetProperty(ref _statusMessage, value);
		}

		public bool IsReaderLogin
		{
			get => _isReaderLogin;
			set
			{
				if (SetProperty(ref _isReaderLogin, value))
				{
					OnPropertyChanged(nameof(IsEmployeeLogin)); // Báo cho UI update RadioButton còn lại
				}
			}
		}

		// Thêm Property này để Binding cho RadioButton "Nhân viên"
		public bool IsEmployeeLogin
		{
			get => !IsReaderLogin;
			set
			{
				if (value) IsReaderLogin = false;
			}
		}

		public bool LoginSuccessTriggered
		{
			get => _loginSuccessTriggered;
			set => SetProperty(ref _loginSuccessTriggered, value);
		}

		public string LoginSuccessFullName
		{
			get => _loginSuccessFullName;
			set => SetProperty(ref _loginSuccessFullName, value);
		}

		public int LoginSuccessUserId
		{
			get => _loginSuccessUserId;
			set => SetProperty(ref _loginSuccessUserId, value);
		}

		public string LoginSuccessAccountType
		{
			get => _loginSuccessAccountType;
			set => SetProperty(ref _loginSuccessAccountType, value);
		}

		public string LoginSuccessRoleName   // ← THÊM PROPERTY NÀY
		{
			get => _loginSuccessRoleName;
			set => SetProperty(ref _loginSuccessRoleName, value);
		}

		public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public LoginViewModel(IAuthService authService)
		{
			_authService = authService;

			LoginCommand = new RelayCommand(ExecuteLogin);
            RegisterCommand = new RelayCommand(ExecuteRegister);
        }

		private async void ExecuteLogin(object parameter)
		{
			if (!System.Text.RegularExpressions.Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			{
				StatusMessage = "Email không hợp lệ.";
				return;
			}

			if (parameter is not PasswordBox passwordBox)
			{
				StatusMessage = "Lỗi: Không lấy được trường mật khẩu.";
				return;
			}

			string password = passwordBox.Password?.Trim() ?? "";
			string email = Email?.Trim() ?? "";

			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
			{
				StatusMessage = "Vui lòng nhập đầy đủ email và mật khẩu.";
				return;
			}

			try
			{
				StatusMessage = "Đang kiểm tra đăng nhập...";

				var dto = new LoginDto
				{
					Email = email,
					Password = password,
					AccountType = IsReaderLogin ? "Reader" : "Employee"
				};

				var result = await _authService.LoginAsync(dto);

				// Debug MessageBox để hiển thị CHI TIẾT result từ service (luôn hiện, dù success hay fail)
				/*MessageBox.Show(
					$"Kết quả từ AuthService.LoginAsync:\n" +
					$"Success: {result.Success}\n" +
					$"Message: {result.Message}\n" +
					$"UserId: {result.UserId}\n" +
					$"FullName: {result.FullName ?? "N/A"}\n" +
					$"AccountType: {result.AccountType ?? "N/A"}\n" +
					$"RoleName: {result.RoleName ?? "N/A"}",
					"Debug - Kết quả Login",
					MessageBoxButton.OK,
					result.Success ? MessageBoxImage.Information : MessageBoxImage.Warning
				);*/

				if (result.Success)
				{
					_authService.SetCurrentUser(result); 
					
					LoginSuccessFullName = result.FullName ?? "Unknown";
					LoginSuccessAccountType = result.AccountType ?? "Unknown";
					LoginSuccessUserId = result.UserId;
					LoginSuccessRoleName = result.RoleName ?? "Reader";

					Debug.WriteLine($"[DEBUG] LOGIN SUCCESS - UserId: {LoginSuccessUserId}, Type: {LoginSuccessAccountType}");

					// Trigger login success để MainViewModel nhận biết và thay đổi UI
					LoginSuccessTriggered = true;
					StatusMessage = "Đăng nhập thành công!";

					// Clear password field sau login thành công
					passwordBox.Password = "";
				}
				else
				{
					StatusMessage = result.Message ?? "Đăng nhập thất bại";
					// MessageBox debug bổ sung nếu fail
					MessageBox.Show($"Login failed: {result.Message}. Check DB for user/email: {Email}", "Debug Info", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			catch (Exception ex)
			{
				StatusMessage = "Lỗi: " + ex.Message;
				Debug.WriteLine("[DEBUG] Login exception: " + ex.Message);
				// Debug MessageBox cho exception
				MessageBox.Show($"Exception during login: {ex.Message}. Stack: {ex.StackTrace}", "Debug Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		public void ClearLoginSuccessTriggered()
		{
			LoginSuccessTriggered = false;
		}
		private async void ExecuteRegister(object parameter)
		{
			// Lấy dữ liệu trực tiếp từ các Property của form Đăng ký
			string email = RegisterEmail?.Trim() ?? "";
			string password = RegisterPassword?.Trim() ?? "";

			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
			{
				StatusMessage = "Vui lòng nhập email và mật khẩu để đăng ký.";
				return;
			}

			try
			{
				StatusMessage = "Đang đăng ký...";
				var dto = new RegisterDto
				{
					Email = email,
					Password = password,
					FullName = RegisterFullName,
					PhoneNumber = RegisterPhone,
					Address = RegisterAddress
				};

				var result = await _authService.RegisterReaderAsync(dto);
				MessageBox.Show($"Register Result\nMessage: {result.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
				StatusMessage = result.Message ?? "Đăng ký xong.";

				// Xóa form sau khi đăng ký thành công
				RegisterEmail = "";
				RegisterPassword = "";
				RegisterFullName = "";
				RegisterPhone = "";
				RegisterAddress = "";
			}
			catch (Exception ex)
			{
				StatusMessage = "Lỗi đăng ký: " + ex.Message;
				MessageBox.Show($"Exception during register:\n{ex.Message}", "Register Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}