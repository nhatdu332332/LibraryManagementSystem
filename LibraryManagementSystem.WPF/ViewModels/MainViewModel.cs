// Full code cho MainViewModel.cs (thêm MessageBox debug)
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;
using LibraryManagementSystem.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem.WPF.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		public LoginViewModel LoginVM { get; }
		public BorrowViewModel BorrowVM { get; }
		public MyAccountViewModel MyAccountVM { get; }
		public ManageAccountsViewModel ManageAccountsVM { get; }
		public ManageBooksViewModel ManageBooksVM { get; }

		private int _selectedTabIndex = 0;
		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				if (SetProperty(ref _selectedTabIndex, value))
				{
					HandleSelectedTabChanged(value);
				}
			}
		}

		private bool _isLoggedIn;
		public bool IsLoggedIn
		{
			get => _isLoggedIn;
			set => SetProperty(ref _isLoggedIn, value);
		}

		private string _welcomeMessage = "Chưa đăng nhập";
		public string WelcomeMessage
		{
			get => _welcomeMessage;
			set => SetProperty(ref _welcomeMessage, value);
		}
		private string _roleDisplay = string.Empty;
		public string RoleDisplay
		{
			get => _roleDisplay;
			set => SetProperty(ref _roleDisplay, value);
		}

		private bool _canManageBorrow;
		public bool CanManageBorrow
		{
			get => _canManageBorrow;
			private set => SetProperty(ref _canManageBorrow, value);
		}

		private bool _canManageBooks;
		public bool CanManageBooks
		{
			get => _canManageBooks;
			private set => SetProperty(ref _canManageBooks, value);
		}

		private bool _canManageAccounts;
		public bool CanManageAccounts
		{
			get => _canManageAccounts;
			private set => SetProperty(ref _canManageAccounts, value);
		}

		public ICommand LogoutCommand { get; }

		private const int MyAccountTabIndex = 4;
		private const int ManageAccountsTabIndex = 5;

		public MainViewModel(
			IAuthService authService,
			IBorrowService borrowService,
			IBookService bookService,
			MyAccountViewModel myAccountViewModel,
			ManageAccountsViewModel manageAccountsViewModel)
		{
			LoginVM = new LoginViewModel(authService);
			BorrowVM = new BorrowViewModel(borrowService, authService);
			MyAccountVM = myAccountViewModel;
			ManageAccountsVM = manageAccountsViewModel;
			ManageBooksVM = new ManageBooksViewModel(bookService);

			LogoutCommand = new RelayCommand(ExecuteLogout);
			MyAccountVM.ProfileUpdated += HandleProfileUpdated;
			ManageAccountsVM.EmployeeUpdated += HandleManagedEmployeeUpdated;

			LoginVM.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(LoginVM.LoginSuccessTriggered))
				{
					CheckLoginSuccess();
				}
			};
		}

		private async void HandleProfileUpdated(int userId, string accountType, string fullName)
		{
			if (!string.IsNullOrWhiteSpace(fullName))
			{
				WelcomeMessage = $"Chào mừng {fullName}";
				LoginVM.LoginSuccessFullName = fullName;
			}

			await ManageAccountsVM.RefreshCurrentAccountAsync(userId, accountType);
		}

		private async void HandleManagedEmployeeUpdated(EmployeeDto employee)
		{
			if (employee == null)
			{
				return;
			}

			if (LoginVM.LoginSuccessAccountType?.Equals("Employee", StringComparison.OrdinalIgnoreCase) != true ||
				LoginVM.LoginSuccessUserId != employee.EmployeeId)
			{
				return;
			}

			if (!string.IsNullOrWhiteSpace(employee.FullName))
			{
				WelcomeMessage = $"Chào mừng {employee.FullName}";
				LoginVM.LoginSuccessFullName = employee.FullName;
			}

			MyAccountVM.ApplyEmployeeProfile(employee);
			await MyAccountVM.RefreshProfileAsync();
		}

		private async void HandleSelectedTabChanged(int tabIndex)
		{
			if (!IsLoggedIn)
			{
				return;
			}

			if (tabIndex == MyAccountTabIndex)
			{
				await MyAccountVM.RefreshProfileAsync();
				return;
			}

			if (tabIndex == ManageAccountsTabIndex)
			{
				await ManageAccountsVM.RefreshCurrentAccountAsync(
					LoginVM.LoginSuccessUserId,
					LoginVM.LoginSuccessAccountType ?? string.Empty);
			}
		}

		private void CheckLoginSuccess()
		{
			if (LoginVM.LoginSuccessTriggered)
			{
				//MessageBox.Show("Login success detected in MainViewModel! Proceeding to update UI.", "Debug Info", MessageBoxButton.OK, MessageBoxImage.Information);

				IsLoggedIn = true;
				WelcomeMessage = $"Chào mừng {LoginVM.LoginSuccessFullName}";
				// Lấy RoleName từ LoginViewModel
				string accountType = LoginVM.LoginSuccessAccountType?.Trim() ?? "";
				string roleName = LoginVM.LoginSuccessRoleName?.Trim() ?? "";

				// Reset tất cả quyền trước khi gán mới
				CanManageBooks = false;
				CanManageBorrow = false;
				CanManageAccounts = false;
				ManageAccountsVM.CanManageEmployees = false;

				// === LOGIC ĐÚNG Ý BẠN: Reader thì hiện ngay, Employee thì lấy RoleName ===
				if (LoginVM.LoginSuccessAccountType?.Trim().Equals("Reader", StringComparison.OrdinalIgnoreCase) == true)
				{
					RoleDisplay = "Bạn là Reader";
				}
				else // Employee
				{
					string role = LoginVM.LoginSuccessRoleName?.Trim() ?? "Staff";

					RoleDisplay = role switch
					{
						"Administrator" => "Bạn là Admin",
						"Librarian" => "Bạn là Librarian",
						"Staff" => "Bạn là Staff",
						_ => "Bạn là " + role
					};
					// Phân quyền theo role
					switch (role)
					{
						case "Administrator":
							CanManageBooks = true;
							CanManageBorrow = true;
							CanManageAccounts = true;
							ManageAccountsVM.CanManageEmployees = true;
							break;

						case "Librarian":
							CanManageBorrow = true;     // được quản lý mượn/trả sách
							CanManageAccounts = true;
							break;

						case "Staff":
							CanManageBooks = true;      // được quản lý danh mục sách
							break;

						default:
							// role lạ → không có quyền đặc biệt
							break;
					}
				}


				SelectedTabIndex = 1;
				MyAccountVM.SetCurrentUser(
					LoginVM.LoginSuccessUserId,
					LoginVM.LoginSuccessAccountType,
					LoginVM.LoginSuccessRoleName,
					LoginVM.LoginSuccessFullName);
				LoginVM.ClearLoginSuccessTriggered();

				Debug.WriteLine($"[DEBUG] Hiển thị Role: {RoleDisplay} (AccountType={LoginVM.LoginSuccessAccountType}, RoleName={LoginVM.LoginSuccessRoleName})");
				Debug.WriteLine($"[DEBUG] Quyền: ManageBooks={CanManageBooks}, ManageBorrow={CanManageBorrow}, ManageAccounts={CanManageAccounts}");
			}
		}

		private void ExecuteLogout(object parameter)
		{
			IsLoggedIn = false;
			WelcomeMessage = "Chưa đăng nhập";
			RoleDisplay = string.Empty;

			// Reset tất cả quyền
			CanManageBooks = false;
			CanManageBorrow = false;
			CanManageAccounts = false;
			ManageAccountsVM.CanManageEmployees = false;

			SelectedTabIndex = 0;
			LoginVM.Email = "";
			LoginVM.StatusMessage = "";
			LoginVM.LoginSuccessTriggered = false;
			LoginVM.LoginSuccessFullName = "";
			LoginVM.LoginSuccessUserId = 0;
			MyAccountVM.ClearCurrentUser();

			Debug.WriteLine("[DEBUG] Logout executed - IsLoggedIn set to false");
		}
	}
}
