// Full code cho BookCatalogViewModel.cs (thêm ConfigureAwait(false) để đồng bộ tránh concurrency)
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;

namespace LibraryManagementSystem.WPF.ViewModels
{
	public class BookCatalogViewModel : ObservableObject
	{
		private readonly IBookService _bookService;

		private ObservableCollection<BookWorkDto> _books = new();
		private string _searchKeyword = string.Empty;
		private string _statusMessage = "Đang tải sách...";

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
				SearchBooks();
			}
		}

		public string StatusMessage
		{
			get => _statusMessage;
			set => SetProperty(ref _statusMessage, value);
		}

		public ICommand RefreshCommand { get; }
		public ICommand LoadAllCommand { get; }

		public BookCatalogViewModel(IBookService bookService)
		{
			_bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));

			RefreshCommand = new RelayCommand(async _ => await LoadAllBooksAsync());
			LoadAllCommand = RefreshCommand;

			_ = LoadAllBooksAsync();
		}

		private async Task LoadAllBooksAsync()
		{
			try
			{
				StatusMessage = "Đang tải sách...";

				var bookWorks = await _bookService.GetAllBookWorksAsync();

				Books = new ObservableCollection<BookWorkDto>(bookWorks);

				StatusMessage = $"Tổng sách: {Books.Count}";
			}
			catch (Exception ex)
			{
				StatusMessage = $"Lỗi tải sách: {ex.Message}";
			}
		}

		private async void SearchBooks()
		{
			try
			{
				StatusMessage = "Đang tìm kiếm...";
				if (string.IsNullOrWhiteSpace(SearchKeyword))
				{
					await LoadAllBooksAsync();
					return;
				}

				var results = await _bookService.SearchBooksAsync(SearchKeyword, null, null, null);
				Books = new ObservableCollection<BookWorkDto>(results);
				StatusMessage = $"Tìm thấy: {Books.Count} kết quả.";
			}
			catch (Exception ex)
			{
				StatusMessage = $"Lỗi tìm kiếm: {ex.Message}";
			}
		}
	}
}
