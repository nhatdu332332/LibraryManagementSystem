using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;
using LibraryManagementSystem.WPF.Views;

namespace LibraryManagementSystem.WPF.ViewModels
{
	public class CreateBorrowTransactionViewModel : ObservableObject
	{
		private readonly IBorrowService _borrowService;
		private readonly IBookService _bookService;
		private readonly IReaderService _readerService;  // giả sử bạn có interface này
		private readonly IAuthService _authService;

		// Reader
		public string SearchReaderKeyword { get; set; } = "";
		public ObservableCollection<ReaderDto> ReaderResults { get; } = new();
		public ReaderDto? SelectedReader { get; set; }

		// Work
		public string SearchWorkKeyword { get; set; } = "";
		public ObservableCollection<BookWorkDto> WorkResults { get; } = new();
		public BookWorkDto? SelectedWork { get; set; }

		// Edition
		public string SearchEditionKeyword { get; set; } = "";
		public ObservableCollection<BookEditionDto> EditionResults { get; } = new();
		public BookEditionDto? SelectedEdition { get; set; }

		// Copy
		public string SearchCopyKeyword { get; set; } = "";
		public ObservableCollection<BookCopyDto> CopyResults { get; } = new();
		public BookCopyDto? SelectedCopy { get; set; }

		// Danh sách mượn tạm
		public ObservableCollection<BorrowDetailTemp> BorrowDetails { get; } = new();

		public DateTime BorrowDate { get; set; } = DateTime.Today;
		public int DurationDays { get; set; } = 14;

		public string StatusMessage { get; set; } = "Chọn độc giả và sách để tạo giao dịch...";

		// Commands
		public ICommand SearchReaderCmd { get; }
		public ICommand SearchWorkCmd { get; }
		public ICommand SearchEditionCmd { get; }
		public ICommand SearchCopyCmd { get; }
		public ICommand AddCopyCmd { get; }
		public ICommand SaveTransactionCmd { get; }
		public ICommand RemoveDetailCommand { get; }
		public CreateBorrowTransactionViewModel(
			IBorrowService borrowService,
			IBookService bookService,
			IReaderService readerService,
			IAuthService authService)
		{
			_borrowService = borrowService;
			_bookService = bookService;
			_readerService = readerService;
			_authService = authService ?? throw new ArgumentNullException(nameof(authService));

			SearchReaderCmd = new RelayCommand(async _ => await SearchReadersAsync());
			SearchWorkCmd = new RelayCommand(async _ => await SearchWorksAsync());
			SearchEditionCmd = new RelayCommand(async _ => await SearchEditionsAsync());
			SearchCopyCmd = new RelayCommand(async _ => await SearchCopiesAsync());
			AddCopyCmd = new RelayCommand(_ => AddSelectedCopy());
			RemoveDetailCommand = new RelayCommand<BorrowDetailTemp>(RemoveDetail);

			SaveTransactionCmd = new RelayCommand(async _ => await SaveTransactionAsync());

		}

		private async Task SearchReadersAsync()
		{
			if (string.IsNullOrWhiteSpace(SearchReaderKeyword)) return;

			// Giả sử bạn có method SearchReadersAsync trong IReaderService
			// Nếu chưa có, dùng GetAllReadersAsync rồi filter client-side
			var all = await _readerService.GetAllReadersAsync();  // hoặc SearchReadersAsync nếu thêm
			var filtered = all.Where(r =>
				r.FullName.Contains(SearchReaderKeyword, StringComparison.OrdinalIgnoreCase) ||
				r.CardNumber.Contains(SearchReaderKeyword, StringComparison.OrdinalIgnoreCase))
				.ToList();

			ReaderResults.Clear();
			foreach (var r in filtered) ReaderResults.Add(r);
			StatusMessage = $"Tìm thấy {filtered.Count} độc giả.";
		}

		private async Task SearchWorksAsync()
		{
			if (string.IsNullOrWhiteSpace(SearchWorkKeyword)) return;

			var results = await _bookService.SearchBooksAsync(SearchWorkKeyword, null, null, null);
			WorkResults.Clear();
			foreach (var w in results) WorkResults.Add(w);
			StatusMessage = $"Tìm thấy {results.Count()} tác phẩm.";
		}

		private async Task SearchEditionsAsync()
		{
			if (SelectedWork == null)
			{
				StatusMessage = "Vui lòng chọn tác phẩm trước!";
				return;
			}

			var allEditions = await _bookService.GetEditionsByWorkIdAsync(SelectedWork.WorkId);
			var filtered = string.IsNullOrWhiteSpace(SearchEditionKeyword)
				? allEditions
				: allEditions.Where(e =>
					e.PublisherName.Contains(SearchEditionKeyword, StringComparison.OrdinalIgnoreCase) ||
					e.ISBN.Contains(SearchEditionKeyword) ||
					e.PublishYear.ToString().Contains(SearchEditionKeyword));

			EditionResults.Clear();
			foreach (var e in filtered) EditionResults.Add(e);
			StatusMessage = $"Tìm thấy {filtered.Count()} phiên bản cho tác phẩm đã chọn.";
		}

		private async Task SearchCopiesAsync()
		{
			if (SelectedEdition == null)
			{
				StatusMessage = "Vui lòng chọn phiên bản trước!";
				return;
			}

			var allCopies = await _bookService.GetAvailableCopiesByEditionIdAsync(SelectedEdition.EditionId);
			var filtered = string.IsNullOrWhiteSpace(SearchCopyKeyword)
				? allCopies
				: allCopies.Where(c =>
					c.Barcode.Contains(SearchCopyKeyword) ||
					c.ShelfLocation?.Contains(SearchCopyKeyword) == true);

			CopyResults.Clear();
			foreach (var c in filtered) CopyResults.Add(c);
			StatusMessage = $"Tìm thấy {filtered.Count()} bản sao khả dụng.";
		}

		private void AddSelectedCopy()
		{
			if (SelectedCopy == null || SelectedWork == null || SelectedEdition == null) return;

			BorrowDetails.Add(new BorrowDetailTemp
			{
				Title = SelectedWork.Title,
				EditionInfo = $"{SelectedEdition.PublisherName} - {SelectedEdition.PublishYear}",
				CopyId = SelectedCopy.CopyId,
				ShelfLocation = SelectedCopy.ShelfLocation ?? "N/A",
				DueDate = BorrowDate.AddDays(DurationDays)
			});

			StatusMessage = $"Đã thêm Copy #{SelectedCopy.CopyId} vào danh sách mượn.";
			SelectedCopy = null; // reset sau khi thêm
		}
		private void RemoveDetail(BorrowDetailTemp? detail)
		{
			if (detail != null && BorrowDetails.Contains(detail))
			{
				BorrowDetails.Remove(detail);
				StatusMessage = $"Đã xóa khỏi danh sách: {detail.Title}";
			}
		}
		private async Task SaveTransactionAsync()
		{
			string debugInfo = $"CurrentUserId: {_authService.CurrentUserId}\n" +
						  $"CurrentAccountType: '{_authService.CurrentAccountType}'\n" +
						  $"CurrentFullName: '{_authService.CurrentFullName}'\n" +
						  $"IsAuthenticated: {_authService.IsAuthenticated}";

			MessageBox.Show(debugInfo, "Debug Auth State", MessageBoxButton.OK, MessageBoxImage.Information); 
			
			if (SelectedReader == null || BorrowDetails.Count == 0)
			{
				StatusMessage = "Chưa chọn độc giả hoặc chưa thêm sách!";
				return;
			}

			try
			{
				if (!_authService.IsAuthenticated || _authService.CurrentUserId == null)
				{
					MessageBox.Show("Chưa đăng nhập! Vui lòng đăng nhập trước khi tạo giao dịch.",
									"Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				// Cho phép tất cả nhân viên (Employee / Administrator / Librarian / Staff)
				bool isEmployee = _authService.CurrentAccountType == "Employee" ||
								  (_authService.CurrentRoleName?.Contains("Admin", StringComparison.OrdinalIgnoreCase) == true) ||
								  (_authService.CurrentRoleName?.Contains("Librarian", StringComparison.OrdinalIgnoreCase) == true) ||
								  (_authService.CurrentRoleName?.Contains("Staff", StringComparison.OrdinalIgnoreCase) == true);

				if (!isEmployee)
				{
					MessageBox.Show($"Tài khoản của bạn ({_authService.CurrentRoleName}) không có quyền tạo giao dịch mượn.",
									"Không có quyền", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				int employeeId = _authService.CurrentUserId.Value;

				var copyIds = BorrowDetails.Select(d => d.CopyId).ToList();

				var transaction = await _borrowService.CreateDirectBorrowTransactionAsync(
					SelectedReader.ReaderId,
					employeeId,
					copyIds,
					BorrowDate,
					DurationDays);

				StatusMessage = $"Tạo giao dịch #{transaction.BorrowId} thành công!";

				MessageBox.Show($"Tạo giao dịch mượn thành công!\nMã GD: #{transaction.BorrowId}",
								"Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

				// Đóng window
				Application.Current.Windows.OfType<CreateBorrowTransactionWindow>()
					.FirstOrDefault()?.Close();
			}
			catch (Exception ex)
			{
				StatusMessage = $"Lỗi: {ex.Message}";
				MessageBox.Show($"Lỗi khi tạo giao dịch:\n{ex.Message}", "Lỗi",
								MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}

	public class BorrowDetailTemp : ObservableObject
	{
		public string Title { get; set; } = "";
		public string EditionInfo { get; set; } = "";
		public int CopyId { get; set; }
		public string ShelfLocation { get; set; } = "";
		public DateTime DueDate { get; set; }
	}


}