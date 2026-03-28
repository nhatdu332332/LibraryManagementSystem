using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;
using LibraryManagementSystem.WPF.Views;

using System;
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
	public class UpdateBorrowTransactionViewModel : ObservableObject
	{
		private readonly IBorrowService _borrowService;

		public int BorrowId { get; set; }
		public BorrowTransactionDto Transaction { get; private set; }
		public ObservableCollection<BorrowTransactionDetailDto> EditableDetails { get; } = new();

		public string StatusMessage { get; set; } = string.Empty;
		public ICommand SaveCommand { get; }

		public UpdateBorrowTransactionViewModel(IBorrowService borrowService)
		{
			_borrowService = borrowService;
			SaveCommand = new RelayCommand(async _ => await SaveAsync());
		}

		public async Task LoadAsync()
		{
			Transaction = await _borrowService.GetBorrowTransactionByIdAsync(BorrowId);
			EditableDetails.Clear();

			foreach (var d in Transaction.Details)
			{
				var detail = new BorrowTransactionDetailDto
				{
					BorrowDetailId = d.BorrowDetailId,
					CopyId = d.CopyId,
					Title = d.Title,
					DueDate = d.DueDate,
					ReturnDate = d.ReturnDate,
					FineAmount = d.FineAmount,
					ConditionNote = d.ConditionNote,

					// Không cần ItemStatus nữa
					CirculationStatus = d.CirculationStatus ?? "Borrowed",
					PhysicalCondition = d.PhysicalCondition ?? "Good"
				};
				EditableDetails.Add(detail);
			}
		}

		private async Task SaveAsync()
		{
			try
			{
				var updates = EditableDetails.Select(d => new UpdateBorrowDetailDto
				{
					BorrowDetailId = d.BorrowDetailId,
					DueDate = d.DueDate,
					CirculationStatus = d.CirculationStatus,
					PhysicalCondition = d.PhysicalCondition
					// Không có ItemStatus
				}).ToList();

				await _borrowService.UpdateBorrowDetailsAsync(BorrowId, updates);

				StatusMessage = "Cập nhật thành công!";
				MessageBox.Show("Đã lưu thay đổi hạn trả và trạng thái mượn!", "Thành công",
					MessageBoxButton.OK, MessageBoxImage.Information);

				Application.Current.Windows.OfType<UpdateBorrowTransactionWindow>()
					.FirstOrDefault()?.Close();
			}
			catch (Exception ex)
			{
				StatusMessage = $"Lỗi: {ex.Message}";
				MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}