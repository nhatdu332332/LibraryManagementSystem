using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;
using LibraryManagementSystem.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.WPF.ViewModels
{
	public class ManageBooksViewModel : ObservableObject
	{
		private readonly IBookService _bookService;

		private ObservableCollection<BookWorkDto> _books = new ObservableCollection<BookWorkDto>();
		private string _searchKeyword = string.Empty;
		private string _statusMessage = "Đang tải danh sách sách...";
		// THÊM: Property để lưu sách đang được chọn
		private BookWorkDto? _selectedBook;
		public BookWorkDto? SelectedBook
		{
			get => _selectedBook;
			set
			{
				SetProperty(ref _selectedBook, value);
				CommandManager.InvalidateRequerySuggested(); // Cập nhật trạng thái bật/tắt của các nút
			}
		}
		public ObservableCollection<BookWorkDto> Books
		{
			get => _books;
			set => SetProperty(ref _books, value);
		}

		public string SearchKeyword
		{
			get => _searchKeyword;
			set
			{
				SetProperty(ref _searchKeyword, value);
				// Tự động tìm kiếm khi gõ (debounce nếu muốn sau)
				_ = SearchBooksAsync();
			}
		}

		public string StatusMessage
		{
			get => _statusMessage;
			set => SetProperty(ref _statusMessage, value);
		}
       
        public ICommand SearchCommand { get; }
		public ICommand RefreshCommand { get; }
		
		public ICommand UpdateBookCommand { get; }
		public ICommand DeleteBookCommand { get; }
		public ICommand DetailBookCommand { get; }

		public ICommand AddCommand { get; set; }
		public ManageBooksViewModel(IBookService bookService)
		{
			_bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));

			SearchCommand = new RelayCommand(async _ => await SearchBooksAsync());
			RefreshCommand = new RelayCommand(async _ => await LoadAllBooksAsync());

			AddCommand = new RelayCommand(
			execute: _ => OpenAddWindow(),
			canExecute: _ => true
			);

			UpdateBookCommand = new RelayCommand(
    _ => OpenEditWindow(),
    _ => SelectedBook != null
);
            DeleteBookCommand = new RelayCommand(_ => DeleteBook(), _ => SelectedBook != null);
            DetailBookCommand = new RelayCommand(_ => ShowBookDetails(), _ => SelectedBook != null);

			// Load ban đầu
			_ = LoadAllBooksAsync();
		}

		private async Task LoadAllBooksAsync()
		{
			try
			{
				StatusMessage = "Đang tải toàn bộ sách...";
				var allBooks = await _bookService.GetAllBookWorksAsync();
				Books = new ObservableCollection<BookWorkDto>(allBooks);
				StatusMessage = $"Tổng cộng: {Books.Count} tác phẩm.";
			}
			catch (Exception ex)
			{
				StatusMessage = $"Lỗi tải danh sách: {ex.Message}";
			}
		}
		public void LoadData()// Load lại danh sách ở màn hình chính sau khi đóng cửa sổ thêm
		{
			_searchKeyword = string.Empty;
			OnPropertyChanged(nameof(SearchKeyword));

			_ = LoadAllBooksAsync();
		}

		private async Task SearchBooksAsync()
		{
			try
			{
				StatusMessage = "Đang tìm kiếm...";

				if (string.IsNullOrWhiteSpace(SearchKeyword))
				{
					await LoadAllBooksAsync();
					return;
				}

				// Hiện tại SearchBooksAsync chỉ hỗ trợ keyword, sau này có thể mở rộng thêm authorId, categoryId, seriesId
				var results = await _bookService.SearchBooksAsync(SearchKeyword, null, null, null);
				Books = new ObservableCollection<BookWorkDto>(results);
				StatusMessage = $"Tìm thấy: {Books.Count} kết quả.";
			}
			catch (Exception ex)
			{
				StatusMessage = $"Lỗi tìm kiếm: {ex.Message}";
			}
		}
		private void OpenAddWindow()
		{
			var addView = new AddBookView();

			var context = new LibraryManagementSystem.Data.LibraryDbContext();
			var uow = new LibraryManagementSystem.Repositories.Repositories.UnitOfWork(context);
			var bookService = new LibraryManagementSystem.Services.BookService(uow);
			var seriesRepo = new LibraryManagementSystem.Repositories.SeriesRepository(context);
			var seriesService = new LibraryManagementSystem.Services.Services.SeriesService(seriesRepo);
			var categoryRepo = new LibraryManagementSystem.Repositories.Repositories.CategoryRepository(context);
			var categoryService = new LibraryManagementSystem.Services.Services.CategoryService(categoryRepo);
			var authorService = new LibraryManagementSystem.Services.Services.AuthorService(context);

			// QUAN TRỌNG: Gán DataContext thì dữ liệu mới hiện lên View
			addView.DataContext = new LibraryManagementSystem.WPF.ViewModels.AddBookViewModel(bookService, seriesService, categoryService, authorService);

			addView.ShowDialog();
			LoadData(); // Load lại danh sách ở màn hình chính sau khi đóng cửa sổ thêm
		}

		private void ShowBookDetails()
		{
			if (SelectedBook == null) return;

			var window = new BookDetailWindow();
			var vm = App.ServiceProvider.GetRequiredService<BookDetailViewModel>();
			window.DataContext = vm;

			// Load data từ Database
			_ = vm.LoadDetailsAsync(SelectedBook.WorkId);

			window.ShowDialog();
		}

        
        private async void DeleteBook()
        {
            if (SelectedBook == null) return;

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa \"{SelectedBook.Title}\"?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await _bookService.DeleteBookWorkAsync(SelectedBook.WorkId);

                await LoadAllBooksAsync();
            }
        }
        private void OpenEditWindow()
        {
            if (SelectedBook == null) return;

            var window = new EditBookView();

            var book = new BookWork
            {
                WorkId = SelectedBook.WorkId,
                Title = SelectedBook.Title,
                OriginalTitle = SelectedBook.OriginalTitle,
                Summary = SelectedBook.Summary,
                FirstPublishYear = SelectedBook.FirstPublishYear ?? 0,
                VolumeNumber = SelectedBook.VolumeNumber
            };

            var vm = new EditBookViewModel(book);

            window.DataContext = vm;
            window.ShowDialog();

            _ = LoadAllBooksAsync();
        }
    }

}
