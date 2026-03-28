using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;

namespace LibraryManagementSystem.WPF.ViewModels
{
	public class ManageAccountsViewModel : ObservableObject
	{
		private readonly IReaderAccountService _readerService;
		private readonly IEmployeeAccountService _employeeService;
		private readonly IRoleService _roleService;

		public event Action<EmployeeDto>? EmployeeUpdated;

		private ObservableCollection<ReaderDto> _readers = new();
		private ObservableCollection<EmployeeDto> _employees = new();
		private ObservableCollection<RoleDto> _roles = new();

		private ReaderDto? _selectedReader;
		private EmployeeDto? _selectedEmployee;

		private string _readerStatusMessage = string.Empty;
		private string _employeeStatusMessage = string.Empty;
		private string _createReaderMessage = string.Empty;
		private string _createEmployeeMessage = string.Empty;

		private int _editReaderId;
		private string _editReaderCardNumber = string.Empty;
		private string _editReaderEmail = string.Empty;
		private string _editReaderFullName = string.Empty;
		private string? _editReaderPhone;
		private string? _editReaderAddress;
		private string _editReaderStatus = string.Empty;
		private DateTime? _editReaderExpiredDate;

		private int _editEmployeeId;
		private string _editEmployeeEmail = string.Empty;
		private string _editEmployeeFullName = string.Empty;
		private string _editEmployeeStatus = string.Empty;
		private int _editEmployeeRoleId;
		private DateTime? _editEmployeeHireDate;

		private string _readerResetPassword = string.Empty;
		private string _readerResetConfirm = string.Empty;
		private string _employeeResetPassword = string.Empty;
		private string _employeeResetConfirm = string.Empty;

		private string _newReaderEmail = string.Empty;
		private string _newReaderFullName = string.Empty;
		private string _newReaderPassword = string.Empty;
		private string _newReaderConfirm = string.Empty;
		private string? _newReaderPhone;
		private string? _newReaderAddress;
		private DateTime? _newReaderExpiredDate;

		private string _newEmployeeEmail = string.Empty;
		private string _newEmployeeFullName = string.Empty;
		private string _newEmployeePassword = string.Empty;
		private string _newEmployeeConfirm = string.Empty;
		private int _newEmployeeRoleId;
		private DateTime? _newEmployeeHireDate;
		private string _newEmployeeStatus = "Active";
		private bool _canManageEmployees;
		public bool CanManageEmployees
		{
			get => _canManageEmployees;
			set => SetProperty(ref _canManageEmployees, value);
		}
		public ObservableCollection<ReaderDto> Readers
		{
			get => _readers;
			set => SetProperty(ref _readers, value);
		}

		public ObservableCollection<EmployeeDto> Employees
		{
			get => _employees;
			set => SetProperty(ref _employees, value);
		}
		public ObservableCollection<RoleDto> Roles
		{
			get => _roles;
			set => SetProperty(ref _roles, value);
		}
		public ObservableCollection<RoleDto> CreateEmployeeRoles { get; } = new();
		public ObservableCollection<RoleDto> EditEmployeeRoles { get; } = new();
		public ObservableCollection<string> ReaderStatusOptions { get; } = new()
		{
			"Active",
			"Expired",
			"Suspended"
		};

		public ObservableCollection<string> EmployeeStatusOptions { get; } = new()
		{
			"Active",
			"Inactive"
		};

		public ReaderDto? SelectedReader
		{
			get => _selectedReader;
			set
			{
				if (SetProperty(ref _selectedReader, value))
				{
					OnPropertyChanged(nameof(HasSelectedReader));
					LoadReaderToEdit(value);
				}
			}
		}

		public EmployeeDto? SelectedEmployee
		{
			get => _selectedEmployee;
			set
			{
				if (SetProperty(ref _selectedEmployee, value))
				{
					OnPropertyChanged(nameof(HasSelectedEmployee));
					OnPropertyChanged(nameof(IsSelectedEmployeeAdministrator));
					OnPropertyChanged(nameof(CanEditSelectedEmployeeRole));
					OnPropertyChanged(nameof(CanEditSelectedEmployeeStatus));
					OnPropertyChanged(nameof(CanDeleteSelectedEmployee));
					LoadEmployeeToEdit(value);
					RefreshEmployeeRoleOptions();
					CommandManager.InvalidateRequerySuggested();
				}
			}
		}

		public bool HasSelectedReader => SelectedReader != null;
		public bool HasSelectedEmployee => SelectedEmployee != null;
		public bool IsSelectedEmployeeAdministrator =>
			SelectedEmployee != null &&
			SelectedEmployee.RoleName.Equals("Administrator", StringComparison.OrdinalIgnoreCase);
		public bool CanEditSelectedEmployeeRole => HasSelectedEmployee && !IsSelectedEmployeeAdministrator;
		public bool CanEditSelectedEmployeeStatus => HasSelectedEmployee && !IsSelectedEmployeeAdministrator;
		public bool CanDeleteSelectedEmployee => HasSelectedEmployee && !IsSelectedEmployeeAdministrator;

		public string ReaderStatusMessage
		{
			get => _readerStatusMessage;
			set => SetProperty(ref _readerStatusMessage, value);
		}

		public string EmployeeStatusMessage
		{
			get => _employeeStatusMessage;
			set => SetProperty(ref _employeeStatusMessage, value);
		}

		public string CreateReaderMessage
		{
			get => _createReaderMessage;
			set => SetProperty(ref _createReaderMessage, value);
		}

		public string CreateEmployeeMessage
		{
			get => _createEmployeeMessage;
			set => SetProperty(ref _createEmployeeMessage, value);
		}

		public int EditReaderId
		{
			get => _editReaderId;
			set => SetProperty(ref _editReaderId, value);
		}

		public string EditReaderCardNumber
		{
			get => _editReaderCardNumber;
			set => SetProperty(ref _editReaderCardNumber, value);
		}

		public string EditReaderEmail
		{
			get => _editReaderEmail;
			set => SetProperty(ref _editReaderEmail, value);
		}

		public string EditReaderFullName
		{
			get => _editReaderFullName;
			set => SetProperty(ref _editReaderFullName, value);
		}

		public string? EditReaderPhone
		{
			get => _editReaderPhone;
			set => SetProperty(ref _editReaderPhone, value);
		}

		public string? EditReaderAddress
		{
			get => _editReaderAddress;
			set => SetProperty(ref _editReaderAddress, value);
		}

		public string EditReaderStatus
		{
			get => _editReaderStatus;
			set => SetProperty(ref _editReaderStatus, value);
		}


		public DateTime? EditReaderExpiredDate
		{
			get => _editReaderExpiredDate;
			set => SetProperty(ref _editReaderExpiredDate, value);
		}

		public int EditEmployeeId
		{
			get => _editEmployeeId;
			set => SetProperty(ref _editEmployeeId, value);
		}

		public string EditEmployeeEmail
		{
			get => _editEmployeeEmail;
			set => SetProperty(ref _editEmployeeEmail, value);
		}

		public string EditEmployeeFullName
		{
			get => _editEmployeeFullName;
			set => SetProperty(ref _editEmployeeFullName, value);
		}

		public string EditEmployeeStatus
		{
			get => _editEmployeeStatus;
			set => SetProperty(ref _editEmployeeStatus, value);
		}

		public int EditEmployeeRoleId
		{
			get => _editEmployeeRoleId;
			set => SetProperty(ref _editEmployeeRoleId, value);
		}

		public DateTime? EditEmployeeHireDate
		{
			get => _editEmployeeHireDate;
			set => SetProperty(ref _editEmployeeHireDate, value);
		}

		public string ReaderResetPassword
		{
			get => _readerResetPassword;
			set => SetProperty(ref _readerResetPassword, value);
		}

		public string ReaderResetConfirm
		{
			get => _readerResetConfirm;
			set => SetProperty(ref _readerResetConfirm, value);
		}

		public string EmployeeResetPassword
		{
			get => _employeeResetPassword;
			set => SetProperty(ref _employeeResetPassword, value);
		}

		public string EmployeeResetConfirm
		{
			get => _employeeResetConfirm;
			set => SetProperty(ref _employeeResetConfirm, value);
		}

		public string NewReaderEmail
		{
			get => _newReaderEmail;
			set => SetProperty(ref _newReaderEmail, value);
		}

		public string NewReaderFullName
		{
			get => _newReaderFullName;
			set => SetProperty(ref _newReaderFullName, value);
		}

		public string NewReaderPassword
		{
			get => _newReaderPassword;
			set => SetProperty(ref _newReaderPassword, value);
		}

		public string NewReaderConfirm
		{
			get => _newReaderConfirm;
			set => SetProperty(ref _newReaderConfirm, value);
		}

		public string? NewReaderPhone
		{
			get => _newReaderPhone;
			set => SetProperty(ref _newReaderPhone, value);
		}

		public string? NewReaderAddress
		{
			get => _newReaderAddress;
			set => SetProperty(ref _newReaderAddress, value);
		}

		public DateTime? NewReaderExpiredDate
		{
			get => _newReaderExpiredDate;
			set => SetProperty(ref _newReaderExpiredDate, value);
		}

		public string NewEmployeeEmail
		{
			get => _newEmployeeEmail;
			set => SetProperty(ref _newEmployeeEmail, value);
		}

		public string NewEmployeeFullName
		{
			get => _newEmployeeFullName;
			set => SetProperty(ref _newEmployeeFullName, value);
		}

		public string NewEmployeePassword
		{
			get => _newEmployeePassword;
			set => SetProperty(ref _newEmployeePassword, value);
		}

		public string NewEmployeeConfirm
		{
			get => _newEmployeeConfirm;
			set => SetProperty(ref _newEmployeeConfirm, value);
		}

		public int NewEmployeeRoleId
		{
			get => _newEmployeeRoleId;
			set => SetProperty(ref _newEmployeeRoleId, value);
		}

		public DateTime? NewEmployeeHireDate
		{
			get => _newEmployeeHireDate;
			set => SetProperty(ref _newEmployeeHireDate, value);
		}

		public string NewEmployeeStatus
		{
			get => _newEmployeeStatus;
			set => SetProperty(ref _newEmployeeStatus, value);
		}

		public ICommand RefreshReadersCommand { get; }
		public ICommand RefreshEmployeesCommand { get; }
		public ICommand UpdateReaderCommand { get; }
		public ICommand DeleteReaderCommand { get; }
		public ICommand UpdateEmployeeCommand { get; }
		public ICommand DeleteEmployeeCommand { get; }
		public ICommand ResetReaderPasswordCommand { get; }
		public ICommand ResetEmployeePasswordCommand { get; }
		public ICommand CreateReaderCommand { get; }
		public ICommand CreateEmployeeCommand { get; }

		public ManageAccountsViewModel(
			IReaderAccountService readerService,
			IEmployeeAccountService employeeService,
			IRoleService roleService)
		{
			_readerService = readerService;
			_employeeService = employeeService;
			_roleService = roleService;

			RefreshReadersCommand = new RelayCommand(async _ => await LoadReadersAsync());
			RefreshEmployeesCommand = new RelayCommand(async _ => await LoadEmployeesAsync());
			UpdateReaderCommand = new RelayCommand(async _ => await UpdateReaderAsync());
			DeleteReaderCommand = new RelayCommand(async _ => await DeleteReaderAsync());
			UpdateEmployeeCommand = new RelayCommand(async _ => await UpdateEmployeeAsync(), _ => HasSelectedEmployee);
			DeleteEmployeeCommand = new RelayCommand(async _ => await DeleteEmployeeAsync(), _ => CanDeleteSelectedEmployee);
			ResetReaderPasswordCommand = new RelayCommand(async _ => await ResetReaderPasswordAsync());
			ResetEmployeePasswordCommand = new RelayCommand(async _ => await ResetEmployeePasswordAsync(), _ => HasSelectedEmployee);
			CreateReaderCommand = new RelayCommand(async _ => await CreateReaderAsync());
			CreateEmployeeCommand = new RelayCommand(async _ => await CreateEmployeeAsync());

			_ = LoadAllAsync();
		}

		public async Task RefreshCurrentAccountAsync(int userId, string accountType)
		{
			if (accountType.Equals("Reader", StringComparison.OrdinalIgnoreCase))
			{
				await LoadReadersAsync();
				SelectedReader = Readers.FirstOrDefault(r => r.ReaderId == userId);
				return;
			}

			if (accountType.Equals("Employee", StringComparison.OrdinalIgnoreCase))
			{
				await LoadEmployeesAsync();
				SelectedEmployee = Employees.FirstOrDefault(e => e.EmployeeId == userId);
			}
		}

		private async Task LoadAllAsync()
		{
			await LoadRolesAsync();
			await LoadReadersAsync();
			await LoadEmployeesAsync();
		}

		private async Task LoadRolesAsync()
		{
			try
			{
				var roles = await _roleService.GetAllRolesAsync();
				Roles = new ObservableCollection<RoleDto>(roles ?? new List<RoleDto>());
				RefreshEmployeeRoleOptions();

				if (HasSelectedEmployee)
				{
					LoadEmployeeToEdit(SelectedEmployee);
				}
			}
			catch (Exception ex)
			{
				EmployeeStatusMessage = $"Loi tai role: {ex.Message}";
			}
		}

		private async Task LoadReadersAsync()
		{
			try
			{
				ReaderStatusMessage = "Dang tai danh sach doc gia...";
				var readers = await _readerService.GetAllReadersAsync();
				Readers = new ObservableCollection<ReaderDto>(readers ?? new List<ReaderDto>());
				ReaderStatusMessage = $"Da tai {Readers.Count} doc gia.";
			}
			catch (Exception ex)
			{
				ReaderStatusMessage = $"Loi: {ex.Message}";
			}
		}

		private async Task LoadEmployeesAsync()
		{
			try
			{
				EmployeeStatusMessage = "Dang tai danh sach nhan vien...";
				var employees = await _employeeService.GetAllEmployeesAsync();
				Employees = new ObservableCollection<EmployeeDto>(employees ?? new List<EmployeeDto>());
				RefreshEmployeeRoleOptions();
				EmployeeStatusMessage = $"Da tai {Employees.Count} nhan vien.";
			}
			catch (Exception ex)
			{
				EmployeeStatusMessage = $"Loi: {ex.Message}";
			}
		}

		private void LoadReaderToEdit(ReaderDto? reader)
		{
			if (reader == null)
			{
				ClearReaderEditForm();
				return;
			}

			EditReaderId = reader.ReaderId;
			EditReaderCardNumber = reader.CardNumber;
			EditReaderEmail = reader.Email;
			EditReaderFullName = reader.FullName;
			EditReaderPhone = reader.PhoneNumber;
			EditReaderAddress = reader.Address;
			EditReaderStatus = reader.ReaderStatus;
			EditReaderExpiredDate = reader.ExpiredDate;
			ReaderResetPassword = string.Empty;
			ReaderResetConfirm = string.Empty;
		}

		private void LoadEmployeeToEdit(EmployeeDto? employee)
		{
			if (employee == null)
			{
				ClearEmployeeEditForm();
				return;
			}

			EditEmployeeId = employee.EmployeeId;
			EditEmployeeEmail = employee.Email;
			EditEmployeeFullName = employee.FullName;
			EditEmployeeStatus = employee.Status;
			EditEmployeeHireDate = employee.HireDate;

			var role = Roles.FirstOrDefault(r => r.RoleName.Equals(employee.RoleName, StringComparison.OrdinalIgnoreCase));
			EditEmployeeRoleId = role?.RoleId ?? 0;
			EmployeeResetPassword = string.Empty;
			EmployeeResetConfirm = string.Empty;
		}

		private async Task UpdateReaderAsync()
		{
			if (SelectedReader == null)
			{
				ReaderStatusMessage = "Chon doc gia de cap nhat.";
				return;
			}

			if (string.IsNullOrWhiteSpace(EditReaderFullName))
			{
				ReaderStatusMessage = "Ho ten khong duoc de trong.";
				return;
			}

			try
			{
				var dto = new UpdateReaderDto
				{
					Email = EditReaderEmail.Trim(),
					FullName = EditReaderFullName.Trim(),
					PhoneNumber = string.IsNullOrWhiteSpace(EditReaderPhone) ? null : EditReaderPhone.Trim(),
					Address = string.IsNullOrWhiteSpace(EditReaderAddress) ? null : EditReaderAddress.Trim(),
					ReaderStatus = string.IsNullOrWhiteSpace(EditReaderStatus) ? null : EditReaderStatus.Trim(),
					ExpiredDate = EditReaderExpiredDate
				};

				await _readerService.UpdateReaderAsync(SelectedReader.ReaderId, dto);
				ReaderStatusMessage = "Cap nhat doc gia thanh cong.";
				await LoadReadersAsync();
				SelectedReader = Readers.FirstOrDefault(r => r.ReaderId == EditReaderId);
			}
			catch (Exception ex)
			{
				ReaderStatusMessage = $"Loi: {ex.Message}";
			}
		}

		private async Task DeleteReaderAsync()
		{
			if (SelectedReader == null)
			{
				ReaderStatusMessage = "Chon doc gia de xoa.";
				return;
			}

			var confirm = MessageBox.Show(
				$"Xoa doc gia \"{SelectedReader.FullName}\"?",
				"Xac nhan xoa",
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning);

			if (confirm != MessageBoxResult.Yes) return;

			try
			{
				await _readerService.DeleteReaderAsync(SelectedReader.ReaderId);
				ReaderStatusMessage = "Da xoa doc gia.";
				await LoadReadersAsync();
				SelectedReader = null;
			}
			catch (Exception ex)
			{
				ReaderStatusMessage = $"Loi: {ex.Message}";
			}
		}

		private async Task ResetReaderPasswordAsync()
		{
			if (SelectedReader == null)
			{
				ReaderStatusMessage = "Chon doc gia de reset mat khau.";
				return;
			}

			if (string.IsNullOrWhiteSpace(ReaderResetPassword) || string.IsNullOrWhiteSpace(ReaderResetConfirm))
			{
				ReaderStatusMessage = "Vui long nhap mat khau moi.";
				return;
			}

			if (!ReaderResetPassword.Equals(ReaderResetConfirm, StringComparison.Ordinal))
			{
				ReaderStatusMessage = "Mat khau moi khong khop.";
				return;
			}

			try
			{
				await _readerService.ResetReaderPasswordAsync(SelectedReader.ReaderId, ReaderResetPassword);
				ReaderStatusMessage = "Reset mat khau doc gia thanh cong.";
				ReaderResetPassword = string.Empty;
				ReaderResetConfirm = string.Empty;
			}
			catch (Exception ex)
			{
				ReaderStatusMessage = $"Loi: {ex.Message}";
			}
		}

		private async Task CreateReaderAsync()
		{
			if (string.IsNullOrWhiteSpace(NewReaderEmail) ||
				string.IsNullOrWhiteSpace(NewReaderFullName) ||
				string.IsNullOrWhiteSpace(NewReaderPassword))
			{
				CreateReaderMessage = "Email, ho ten va mat khau khong duoc de trong.";
				return;
			}

			if (!NewReaderPassword.Equals(NewReaderConfirm, StringComparison.Ordinal))
			{
				CreateReaderMessage = "Mat khau xac nhan khong khop.";
				return;
			}

			try
			{
				var dto = new CreateReaderDto
				{
					Email = NewReaderEmail.Trim(),
					Password = NewReaderPassword,
					FullName = NewReaderFullName.Trim(),
					PhoneNumber = string.IsNullOrWhiteSpace(NewReaderPhone) ? null : NewReaderPhone.Trim(),
					Address = string.IsNullOrWhiteSpace(NewReaderAddress) ? null : NewReaderAddress.Trim(),
					ExpiredDate = NewReaderExpiredDate
				};

				await _readerService.CreateReaderAsync(dto);
				CreateReaderMessage = "Tao doc gia thanh cong.";
				ClearCreateReaderForm();
				await LoadReadersAsync();
			}
			catch (Exception ex)
			{
				CreateReaderMessage = $"Loi: {ex.Message}";
			}
		}

		private async Task UpdateEmployeeAsync()
		{
			if (SelectedEmployee == null)
			{
				EmployeeStatusMessage = "Chon nhan vien de cap nhat.";
				return;
			}

			if (string.IsNullOrWhiteSpace(EditEmployeeFullName))
			{
				EmployeeStatusMessage = "Ho ten khong duoc de trong.";
				return;
			}

			try
			{
				var employeeId = SelectedEmployee.EmployeeId;
				var fullName = EditEmployeeFullName.Trim();

				var dto = new UpdateEmployeeDto
				{
					Email = EditEmployeeEmail.Trim(),
					FullName = fullName,
					Status = string.IsNullOrWhiteSpace(EditEmployeeStatus) ? null : EditEmployeeStatus.Trim(),
					RoleId = EditEmployeeRoleId > 0 ? EditEmployeeRoleId : null
				};

				await _employeeService.UpdateEmployeeAsync(employeeId, dto);
				EmployeeStatusMessage = "Cap nhat nhan vien thanh cong.";
				await LoadEmployeesAsync();
				SelectedEmployee = Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
				if (SelectedEmployee != null)
				{
					EmployeeUpdated?.Invoke(SelectedEmployee);
				}
			}
			catch (Exception ex)
			{
				EmployeeStatusMessage = $"Loi: {ex.Message}";
			}
		}

		private async Task DeleteEmployeeAsync()
		{
			if (SelectedEmployee == null)
			{
				EmployeeStatusMessage = "Chon nhan vien de xoa.";
				return;
			}

			var confirm = MessageBox.Show(
				$"Xoa nhan vien \"{SelectedEmployee.FullName}\"?",
				"Xac nhan xoa",
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning);

			if (confirm != MessageBoxResult.Yes) return;

			try
			{
				await _employeeService.DeleteEmployeeAsync(SelectedEmployee.EmployeeId);
				EmployeeStatusMessage = "Da xoa nhan vien.";
				await LoadEmployeesAsync();
				SelectedEmployee = null;
			}
			catch (Exception ex)
			{
				EmployeeStatusMessage = $"Loi: {ex.Message}";
			}
		}

		private async Task ResetEmployeePasswordAsync()
		{
			if (SelectedEmployee == null)
			{
				EmployeeStatusMessage = "Chon nhan vien de reset mat khau.";
				return;
			}

			if (string.IsNullOrWhiteSpace(EmployeeResetPassword) || string.IsNullOrWhiteSpace(EmployeeResetConfirm))
			{
				EmployeeStatusMessage = "Vui long nhap mat khau moi.";
				return;
			}

			if (!EmployeeResetPassword.Equals(EmployeeResetConfirm, StringComparison.Ordinal))
			{
				EmployeeStatusMessage = "Mat khau moi khong khop.";
				return;
			}

			try
			{
				await _employeeService.ResetEmployeePasswordAsync(SelectedEmployee.EmployeeId, EmployeeResetPassword);
				EmployeeStatusMessage = "Reset mat khau nhan vien thanh cong.";
				EmployeeResetPassword = string.Empty;
				EmployeeResetConfirm = string.Empty;
			}
			catch (Exception ex)
			{
				EmployeeStatusMessage = $"Loi: {ex.Message}";
			}
		}

		private async Task CreateEmployeeAsync()
		{
			if (string.IsNullOrWhiteSpace(NewEmployeeEmail) ||
				string.IsNullOrWhiteSpace(NewEmployeeFullName) ||
				string.IsNullOrWhiteSpace(NewEmployeePassword))
			{
				CreateEmployeeMessage = "Email, ho ten va mat khau khong duoc de trong.";
				return;
			}

			if (NewEmployeeRoleId <= 0)
			{
				CreateEmployeeMessage = "Vui long chon vai tro.";
				return;
			}

			if (!NewEmployeePassword.Equals(NewEmployeeConfirm, StringComparison.Ordinal))
			{
				CreateEmployeeMessage = "Mat khau xac nhan khong khop.";
				return;
			}

			try
			{
				var dto = new CreateEmployeeDto
				{
					Email = NewEmployeeEmail.Trim(),
					Password = NewEmployeePassword,
					FullName = NewEmployeeFullName.Trim(),
					RoleId = NewEmployeeRoleId,
					HireDate = NewEmployeeHireDate,
					Status = string.IsNullOrWhiteSpace(NewEmployeeStatus) ? "Active" : NewEmployeeStatus.Trim()
				};

				await _employeeService.CreateEmployeeAsync(dto);
				CreateEmployeeMessage = "Tao nhan vien thanh cong.";
				ClearCreateEmployeeForm();
				await LoadEmployeesAsync();
			}
			catch (Exception ex)
			{
				CreateEmployeeMessage = $"Loi: {ex.Message}";
			}
		}

		private void ClearCreateReaderForm()
		{
			NewReaderEmail = string.Empty;
			NewReaderFullName = string.Empty;
			NewReaderPassword = string.Empty;
			NewReaderConfirm = string.Empty;
			NewReaderPhone = string.Empty;
			NewReaderAddress = string.Empty;
			NewReaderExpiredDate = null;
		}

		private void ClearCreateEmployeeForm()
		{
			NewEmployeeEmail = string.Empty;
			NewEmployeeFullName = string.Empty;
			NewEmployeePassword = string.Empty;
			NewEmployeeConfirm = string.Empty;
			NewEmployeeRoleId = 0;
			NewEmployeeHireDate = null;
			NewEmployeeStatus = "Active";
		}

		private void RefreshEmployeeRoleOptions()
		{
			var hasAdministrator = Employees.Any(employee =>
				employee.RoleName.Equals("Administrator", StringComparison.OrdinalIgnoreCase));

			ReplaceRoles(
				CreateEmployeeRoles,
				Roles.Where(role =>
					!hasAdministrator ||
					!role.RoleName.Equals("Administrator", StringComparison.OrdinalIgnoreCase)));

			var editRoles = Roles.Where(role =>
				!hasAdministrator ||
				IsSelectedEmployeeAdministrator ||
				!role.RoleName.Equals("Administrator", StringComparison.OrdinalIgnoreCase));

			ReplaceRoles(EditEmployeeRoles, editRoles);
			if (NewEmployeeRoleId > 0 && !CreateEmployeeRoles.Any(role => role.RoleId == NewEmployeeRoleId))
			{
				NewEmployeeRoleId = 0;
			}
			OnPropertyChanged(nameof(IsSelectedEmployeeAdministrator));
			OnPropertyChanged(nameof(CanEditSelectedEmployeeRole));
			OnPropertyChanged(nameof(CanEditSelectedEmployeeStatus));
			OnPropertyChanged(nameof(CanDeleteSelectedEmployee));
		}

		private void ClearReaderEditForm()
		{
			EditReaderId = 0;
			EditReaderCardNumber = string.Empty;
			EditReaderEmail = string.Empty;
			EditReaderFullName = string.Empty;
			EditReaderPhone = string.Empty;
			EditReaderAddress = string.Empty;
			EditReaderStatus = string.Empty;
			EditReaderExpiredDate = null;
			ReaderResetPassword = string.Empty;
			ReaderResetConfirm = string.Empty;
		}

		private void ClearEmployeeEditForm()
		{
			EditEmployeeId = 0;
			EditEmployeeEmail = string.Empty;
			EditEmployeeFullName = string.Empty;
			EditEmployeeStatus = string.Empty;
			EditEmployeeRoleId = 0;
			EditEmployeeHireDate = null;
			EmployeeResetPassword = string.Empty;
			EmployeeResetConfirm = string.Empty;
		}

		private static void ReplaceRoles(ObservableCollection<RoleDto> target, IEnumerable<RoleDto> roles)
		{
			target.Clear();
			foreach (var role in roles)
			{
				target.Add(role);
			}
		}

	}
}




