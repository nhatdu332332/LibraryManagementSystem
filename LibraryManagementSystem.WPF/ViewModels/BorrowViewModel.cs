using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;
using LibraryManagementSystem.WPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem.WPF.ViewModels
{
	public class BorrowViewModel : ObservableObject
	{
		private readonly IBorrowService _borrowService;
		private readonly IAuthService _authService;

		private ObservableCollection<BorrowTransactionDto> _borrowTransactions = new();
		private string _statusMessage = "Đang tải dữ liệu...";
		private int _transactionCount;
		

		public ObservableCollection<BorrowTransactionDto> BorrowTransactions
		{
			get => _borrowTransactions;
			set => SetProperty(ref _borrowTransactions, value);
		}

		public string StatusMessage
		{
			get => _statusMessage;
			set => SetProperty(ref _statusMessage, value);
		}

		public int TransactionCount
		{
			get => _transactionCount;
			set => SetProperty(ref _transactionCount, value);
		}
		private string _searchText = string.Empty;
		public string SearchText
		{
			get => _searchText;
			set
			{
				SetProperty(ref _searchText, value);
				// Nếu bạn muốn tìm kiếm ngay khi đang gõ, có thể gọi ExecuteSearch ở đây
			}
		}

		// Danh sách tạm để giữ toàn bộ dữ liệu gốc khi lọc
		private List<BorrowTransactionDto> _allTransactions = new();

		public ICommand SearchCommand { get; }
		public ICommand RefreshTransactionsCommand { get; }

		// Phần mới: Property cho item được chọn trong DataGrid
		private BorrowTransactionDto? _selectedTransaction;
		public BorrowTransactionDto? SelectedTransaction
		{
			get => _selectedTransaction;
			set
			{
				SetProperty(ref _selectedTransaction, value);
				CommandManager.InvalidateRequerySuggested(); // ← THÊM DÒNG NÀY
			}
		}

		// Phần mới: 3 command cho các nút Tạo / Cập nhật / Xóa
		public ICommand CreateDirectBorrowCommand { get; }
		public ICommand UpdateBorrowCommand { get; }
		public ICommand DeleteBorrowCommand { get; }
		public ICommand CompleteItemCommand { get; }
		public ICommand ToggleCompleteCommand { get; }
		public BorrowViewModel(IBorrowService borrowService, IAuthService authService)
		{
			_borrowService = borrowService ?? throw new ArgumentNullException(nameof(borrowService));
			_authService = authService ?? throw new ArgumentNullException(nameof(authService));

			// Optional: Thay bằng logging
			Debug.WriteLine("BorrowViewModel đã được khởi tạo!");

			RefreshTransactionsCommand = new RelayCommand(async _ =>
			{
				SearchText = string.Empty; // Xóa text tìm kiếm khi refresh
				await LoadTransactionsAsync();
			});

			// Phần mới: Khởi tạo 3 command
			CreateDirectBorrowCommand = new RelayCommand(_ => CreateDirectBorrow(), _ => CanManageBorrowTransactions());
			UpdateBorrowCommand = new RelayCommand(_ => UpdateSelectedBorrow(), _ => CanEditSelectedBorrow());
			DeleteBorrowCommand = new RelayCommand(_ => DeleteSelectedBorrow(), _ => CanDeleteSelectedBorrow());
			CompleteItemCommand = new RelayCommand<int>(async (detailId) => await ExecuteCompleteItemAsync(detailId));
			ToggleCompleteCommand = new RelayCommand<BorrowTransactionDetailDto>(async (detail) => await ExecuteToggleCompleteAsync(detail));
			SearchCommand = new RelayCommand(async (obj) => await ExecuteSearchAsync());

			_ = LoadTransactionsAsync();
		}

		private async Task LoadTransactionsAsync()
		{
			// Optional: Thay bằng logging
			Debug.WriteLine("Bắt đầu LoadTransactionsAsync");

			try
			{
				StatusMessage = "Đang tải danh sách giao dịch...";

				var transactions = await _borrowService.GetAllBorrowTransactionsAsync();

				int count = transactions?.Count() ?? 0;
				// Optional: Debug.WriteLine($"Load được {count} giao dịch. Nếu 0 → DB có thể trống hoặc query sai.");

				var list = transactions?.ToList() ?? new List<BorrowTransactionDto>();
				_allTransactions = list;

				BorrowTransactions = new ObservableCollection<BorrowTransactionDto>(list);

				TransactionCount = BorrowTransactions.Count;
				StatusMessage = $"Đã tải {TransactionCount} giao dịch.";

				if (TransactionCount == 0)
				{
					Debug.WriteLine("Không có giao dịch nào trong DB. Hãy kiểm tra bảng BorrowTransaction bằng SSMS hoặc thêm data test.");
				}
			}
			catch (Exception ex)
			{
				StatusMessage = $"Lỗi: {ex.Message}";
				// Optional: Thay bằng logging
				Debug.WriteLine($"Lỗi chi tiết khi load: {ex.Message}\nStackTrace: {ex.StackTrace}");
			}
		}


		private async void UpdateSelectedBorrow()
		{
			if (SelectedTransaction == null)
			{
				StatusMessage = "Vui lòng chọn một giao dịch để cập nhật.";
				return;
			}

			var window = new UpdateBorrowTransactionWindow();
			var vm = App.ServiceProvider.GetRequiredService<UpdateBorrowTransactionViewModel>();
			vm.BorrowId = SelectedTransaction.BorrowId;
			window.DataContext = vm;

			// Load dữ liệu async
			await vm.LoadAsync();

			window.ShowDialog();

			// Refresh danh sách
			await LoadTransactionsAsync();
		}

		private async Task ExecuteSearchAsync()
		{
			if (string.IsNullOrWhiteSpace(SearchText))
			{
				BorrowTransactions = new ObservableCollection<BorrowTransactionDto>(_allTransactions);
				TransactionCount = BorrowTransactions.Count;
				StatusMessage = "Đã hiển thị toàn bộ danh sách.";
				return;
			}

			StatusMessage = $"Đang tìm kiếm cho: {SearchText}...";

			// Lọc dựa trên các thuộc tính ĐANG CÓ trong DTO
			var filtered = _allTransactions.Where(t =>
				// 1. Tìm theo tên độc giả (không phân biệt hoa thường)
				(t.ReaderFullName != null && t.ReaderFullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||

				// 2. Tìm theo tên nhân viên (nếu cần)
				(t.EmployeeFullName != null && t.EmployeeFullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||

				// 3. Tìm theo mã giao dịch mượn (BorrowId)
				t.BorrowId.ToString().Contains(SearchText)
			).ToList();

			BorrowTransactions = new ObservableCollection<BorrowTransactionDto>(filtered);
			TransactionCount = BorrowTransactions.Count;
			StatusMessage = $"Tìm thấy {TransactionCount} kết quả.";
		}

		private async void DeleteSelectedBorrow()
		{
			if (SelectedTransaction == null)
			{
				StatusMessage = "Vui lòng chọn một giao dịch để xóa.";
				return;
			}
			// THÊM MỚI: Bổ sung kiểm tra chặn xóa nếu có sách chưa "Complete"
			if (SelectedTransaction.Details == null || !SelectedTransaction.Details.All(d => d.ItemStatus == "Complete"))
			{
				MessageBox.Show("Không thể xóa! Tất cả các cuốn sách trong giao dịch này phải được đánh dấu hoàn thành (Complete).", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var result = MessageBox.Show(
				$"Bạn có chắc muốn xóa giao dịch #{SelectedTransaction.BorrowId}?\nHành động này không thể hoàn tác.",
				"Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				try
				{
					StatusMessage = $"Đang xóa giao dịch #{SelectedTransaction.BorrowId}...";
					await _borrowService.DeleteBorrowTransactionAsync(SelectedTransaction.BorrowId);
					StatusMessage = $"Đã xóa giao dịch #{SelectedTransaction.BorrowId} thành công!";
					await LoadTransactionsAsync();
				}
				catch (Exception ex)
				{
					StatusMessage = $"Lỗi xóa: {ex.Message}";
					MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
		private async Task ExecuteCompleteItemAsync(int borrowDetailId)
		{
			try
			{
				// 1. Cập nhật DB
				await _borrowService.CompleteBorrowDetailAsync(borrowDetailId);

				// 2. Refresh lại dữ liệu trên màn hình để ô tích hiện lên chính xác
				MessageBox.Show("Đã đánh dấu hoàn thành cuốn sách này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

				// Gọi lại hàm load dữ liệu của bạn ở đây (ví dụ: LoadBorrowTransactionsAsync)
				// Lưu ý: Nhớ gán lại SelectedTransaction để grid chi tiết hiển thị lại dữ liệu mới nhất
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}");
			}
		}

		// Helper kiểm tra quyền / điều kiện enable nút
		private bool CanManageBorrowTransactions()
		{
			// Giả sử bạn có cách lấy role từ login (có thể cần thêm property RoleName hoặc dùng IAuthService)
			// Tạm thời cho phép Librarian/Staff/Admin
			return true; // Thay bằng logic thực tế: CurrentRole == "Librarian" || ...
		}

		private bool CanEditSelectedBorrow()
		{
			return SelectedTransaction != null &&
				   CanManageBorrowTransactions() &&
				   SelectedTransaction.Status != "FullyReturned" &&
				   SelectedTransaction.Status != "Cancelled";
		}

		private bool CanDeleteSelectedBorrow()
		{
			// Điều kiện: Phải có giao dịch được chọn, người dùng có quyền quản lý,
			// phải có chi tiết sách, và TẤT CẢ sách (Details) đều phải có ItemStatus là "Complete"
			return SelectedTransaction != null &&
				   CanManageBorrowTransactions() &&
				   SelectedTransaction.Details != null &&
				   SelectedTransaction.Details.Count > 0 &&
				   SelectedTransaction.Details.All(d => d.ItemStatus == "Complete");
		}
		private void CreateDirectBorrow()
		{
			var window = new CreateBorrowTransactionWindow();
			window.DataContext = App.ServiceProvider.GetRequiredService<CreateBorrowTransactionViewModel>();

			// Load dữ liệu async nếu cần
			// if (window.DataContext is CreateBorrowTransactionViewModel vm)
			// {
			//     _ = vm.LoadReadersAsync();   // ← COMMENT HOẶC XÓA DÒNG NÀY ĐỂ FIX LỖI
			// }

			window.ShowDialog();

			// Sau khi đóng window, refresh danh sách nếu cần
			_ = LoadTransactionsAsync();
		}
		private async Task ExecuteToggleCompleteAsync(BorrowTransactionDetailDto detail)
		{
			if (detail == null) return;
			try
			{
				// Gọi Service cập nhật trực tiếp DB
				await _borrowService.ToggleItemStatusAsync(detail.BorrowDetailId, detail.IsComplete);
				// (Tùy chọn) Cập nhật trạng thái
				StatusMessage = $"Đã cập nhật trạng thái chi tiết mượn #{detail.BorrowDetailId} thành {detail.ItemStatus}";
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi cập nhật trạng thái: {ex.Message}");
				// Refresh lại danh sách nếu cập nhật DB thất bại để tránh sai lệch UI
				await LoadTransactionsAsync();
			}
		}
	}
}
