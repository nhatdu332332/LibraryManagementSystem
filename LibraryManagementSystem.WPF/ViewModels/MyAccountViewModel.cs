using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;

namespace LibraryManagementSystem.WPF.ViewModels
{
	public class MyAccountViewModel : ObservableObject
	{
		private readonly IReaderAccountService _readerService;
		private readonly IEmployeeAccountService _employeeService;

		public event Action<int, string, string>? ProfileUpdated;

		private int _userId;
		private string _accountType = string.Empty;
		private string _roleName = string.Empty;
		private bool _isLoggedIn;
		private bool _isBusy;

		private string _fullName = string.Empty;
		private string _email = string.Empty;
		private string? _phoneNumber;
		private string? _address;
		private string _editableFullName = string.Empty;
		private string _editableEmail = string.Empty;
		private string? _editablePhoneNumber;
		private string? _editableAddress;
		private string _status = string.Empty;
		private string _cardNumber = string.Empty;
		private DateTime? _registerDate;
		private DateTime? _expiredDate;
		private DateTime? _hireDate;

		private string _currentPassword = string.Empty;
		private string _newPassword = string.Empty;
		private string _confirmPassword = string.Empty;
		private string _statusMessage = string.Empty;

		public int UserId
		{
			get => _userId;
			private set => SetProperty(ref _userId, value);
		}

		public string AccountType
		{
			get => _accountType;
			private set
			{
				if (SetProperty(ref _accountType, value))
				{
					OnPropertyChanged(nameof(IsReader));
					OnPropertyChanged(nameof(IsEmployee));
				}
			}
		}

		public string RoleName
		{
			get => _roleName;
			private set => SetProperty(ref _roleName, value);
		}

		public bool IsLoggedIn
		{
			get => _isLoggedIn;
			private set => SetProperty(ref _isLoggedIn, value);
		}

		public bool IsBusy
		{
			get => _isBusy;
			private set => SetProperty(ref _isBusy, value);
		}

		public bool IsReader => AccountType.Equals("Reader", StringComparison.OrdinalIgnoreCase);
		public bool IsEmployee => AccountType.Equals("Employee", StringComparison.OrdinalIgnoreCase);

		public string FullName
		{
			get => _fullName;
			set => SetProperty(ref _fullName, value);
		}

		public string Email
		{
			get => _email;
			set => SetProperty(ref _email, value);
		}

		public string? PhoneNumber
		{
			get => _phoneNumber;
			set => SetProperty(ref _phoneNumber, value);
		}

		public string? Address
		{
			get => _address;
			set => SetProperty(ref _address, value);
		}

		public string EditableFullName
		{
			get => _editableFullName;
			set => SetProperty(ref _editableFullName, value);
		}

		public string EditableEmail
		{
			get => _editableEmail;
			set => SetProperty(ref _editableEmail, value);
		}

		public string? EditablePhoneNumber
		{
			get => _editablePhoneNumber;
			set => SetProperty(ref _editablePhoneNumber, value);
		}

		public string? EditableAddress
		{
			get => _editableAddress;
			set => SetProperty(ref _editableAddress, value);
		}

		public string Status
		{
			get => _status;
			private set => SetProperty(ref _status, value);
		}

		public string CardNumber
		{
			get => _cardNumber;
			private set => SetProperty(ref _cardNumber, value);
		}

		public DateTime? RegisterDate
		{
			get => _registerDate;
			private set => SetProperty(ref _registerDate, value);
		}

		public DateTime? ExpiredDate
		{
			get => _expiredDate;
			private set => SetProperty(ref _expiredDate, value);
		}

		public DateTime? HireDate
		{
			get => _hireDate;
			private set => SetProperty(ref _hireDate, value);
		}

		public string CurrentPassword
		{
			get => _currentPassword;
			set => SetProperty(ref _currentPassword, value);
		}

		public string NewPassword
		{
			get => _newPassword;
			set => SetProperty(ref _newPassword, value);
		}

		public string ConfirmPassword
		{
			get => _confirmPassword;
			set => SetProperty(ref _confirmPassword, value);
		}

		public string StatusMessage
		{
			get => _statusMessage;
			set => SetProperty(ref _statusMessage, value);
		}

		public ICommand LoadProfileCommand { get; }
		public ICommand SaveProfileCommand { get; }
		public ICommand ChangePasswordCommand { get; }

		public MyAccountViewModel(IReaderAccountService readerService, IEmployeeAccountService employeeService)
		{
			_readerService = readerService;
			_employeeService = employeeService;

			LoadProfileCommand = new RelayCommand(async _ => await LoadProfileAsync());
			SaveProfileCommand = new RelayCommand(async _ => await SaveProfileAsync());
			ChangePasswordCommand = new RelayCommand(async _ => await ChangePasswordAsync());
		}

		public void SetCurrentUser(int userId, string accountType, string roleName, string fullName)
		{
			UserId = userId;
			AccountType = accountType?.Trim() ?? string.Empty;
			RoleName = roleName?.Trim() ?? string.Empty;
			IsLoggedIn = userId > 0;
			FullName = fullName ?? string.Empty;
			SyncEditableProfileFields();

			_ = LoadProfileAsync();
		}

		public void ClearCurrentUser()
		{
			UserId = 0;
			AccountType = string.Empty;
			RoleName = string.Empty;
			IsLoggedIn = false;
			FullName = string.Empty;
			Email = string.Empty;
			PhoneNumber = string.Empty;
			Address = string.Empty;
			SyncEditableProfileFields();
			Status = string.Empty;
			CardNumber = string.Empty;
			RegisterDate = null;
			ExpiredDate = null;
			HireDate = null;
			ClearPasswordInputs();
			StatusMessage = string.Empty;
		}

		public Task RefreshProfileAsync()
		{
			return LoadProfileAsync();
		}

		public void ApplyEmployeeProfile(EmployeeDto profile)
		{
			if (profile == null) return;

			MapEmployeeProfile(profile);
			StatusMessage = "Da tai thong tin tai khoan.";
		}

		private async Task LoadProfileAsync()
		{
			if (IsBusy) return;

			IsBusy = true;
			try
			{
				await LoadProfileCoreAsync();
			}
			catch (Exception ex)
			{
				StatusMessage = $"Loi: {ex.Message}";
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task LoadProfileCoreAsync()
		{
			if (!IsLoggedIn || UserId <= 0)
			{
				StatusMessage = "Chua dang nhap.";
				return;
			}

			StatusMessage = "Dang tai thong tin tai khoan...";

			if (IsReader)
			{
				var profile = await _readerService.GetReaderByIdAsync(UserId);
				MapReaderProfile(profile);
			}
			else if (IsEmployee)
			{
				var profile = await _employeeService.GetEmployeeByIdAsync(UserId);
				MapEmployeeProfile(profile);
			}
			else
			{
				StatusMessage = "Loai tai khoan khong hop le.";
				return;
			}

			StatusMessage = "Da tai thong tin tai khoan.";
		}

		private async Task SaveProfileAsync()
		{
			if (IsBusy) return;

			if (!IsLoggedIn || UserId <= 0)
			{
				StatusMessage = "Chua dang nhap.";
				return;
			}

			if (string.IsNullOrWhiteSpace(EditableFullName))
			{
				StatusMessage = "Ho ten khong duoc de trong.";
				return;
			}

			if (string.IsNullOrWhiteSpace(EditableEmail))
			{
				StatusMessage = "Email khong duoc de trong.";
				return;
			}

			IsBusy = true;
			try
			{
				StatusMessage = "Dang cap nhat thong tin...";

				if (IsReader)
				{
					var dto = new UpdateReaderDto
					{
						Email = EditableEmail.Trim(),
						FullName = EditableFullName.Trim(),
						PhoneNumber = string.IsNullOrWhiteSpace(EditablePhoneNumber) ? null : EditablePhoneNumber.Trim(),
						Address = string.IsNullOrWhiteSpace(EditableAddress) ? null : EditableAddress.Trim()
					};

					await _readerService.UpdateReaderAsync(UserId, dto);
				}
				else if (IsEmployee)
				{
					var dto = new UpdateEmployeeDto
					{
						Email = EditableEmail.Trim(),
						FullName = EditableFullName.Trim()
					};

					await _employeeService.UpdateEmployeeAsync(UserId, dto);
				}
				else
				{
					StatusMessage = "Loai tai khoan khong hop le.";
					return;
				}

				await LoadProfileCoreAsync();
				StatusMessage = "Cap nhat thanh cong.";
				ProfileUpdated?.Invoke(UserId, AccountType, FullName);
			}
			catch (Exception ex)
			{
				StatusMessage = $"Loi: {ex.Message}";
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task ChangePasswordAsync()
		{
			if (IsBusy) return;

			if (!IsLoggedIn || UserId <= 0)
			{
				StatusMessage = "Chua dang nhap.";
				return;
			}

			if (string.IsNullOrWhiteSpace(CurrentPassword) ||
				string.IsNullOrWhiteSpace(NewPassword) ||
				string.IsNullOrWhiteSpace(ConfirmPassword))
			{
				StatusMessage = "Vui long nhap day du mat khau.";
				return;
			}

			if (!NewPassword.Equals(ConfirmPassword, StringComparison.Ordinal))
			{
				StatusMessage = "Mat khau moi khong khop.";
				return;
			}

			IsBusy = true;
			try
			{
				if (IsReader)
				{
					await _readerService.ChangeReaderPasswordAsync(UserId, CurrentPassword, NewPassword);
				}
				else if (IsEmployee)
				{
					await _employeeService.ChangeEmployeePasswordAsync(UserId, CurrentPassword, NewPassword);
				}
				else
				{
					StatusMessage = "Loai tai khoan khong hop le.";
					return;
				}

				StatusMessage = "Doi mat khau thanh cong.";
				ClearPasswordInputs();
			}
			catch (Exception ex)
			{
				StatusMessage = $"Loi: {ex.Message}";
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void MapReaderProfile(ReaderDto profile)
		{
			if (profile == null) return;

			FullName = profile.FullName;
			Email = profile.Email;
			PhoneNumber = profile.PhoneNumber;
			Address = profile.Address;
			Status = profile.ReaderStatus;
			CardNumber = profile.CardNumber;
			RegisterDate = profile.RegisterDate;
			ExpiredDate = profile.ExpiredDate;
			HireDate = null;
			SyncEditableProfileFields();
		}

		private void MapEmployeeProfile(EmployeeDto profile)
		{
			if (profile == null) return;

			FullName = profile.FullName;
			Email = profile.Email;
			RoleName = profile.RoleName;
			Status = profile.Status;
			HireDate = profile.HireDate;
			CardNumber = string.Empty;
			RegisterDate = null;
			ExpiredDate = null;
			PhoneNumber = string.Empty;
			Address = string.Empty;
			SyncEditableProfileFields();
		}

		private void ClearPasswordInputs()
		{
			CurrentPassword = string.Empty;
			NewPassword = string.Empty;
			ConfirmPassword = string.Empty;
		}

		private void SyncEditableProfileFields()
		{
			EditableFullName = FullName;
			EditableEmail = Email;
			EditablePhoneNumber = PhoneNumber;
			EditableAddress = Address;
		}
	}
}
