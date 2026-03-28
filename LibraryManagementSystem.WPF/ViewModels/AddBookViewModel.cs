using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.Services.Services;
using LibraryManagementSystem.WPF.Helpers;
using LibraryManagementSystem.WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.WPF.ViewModels
{
    public class AddBookViewModel : INotifyPropertyChanged
    {
        private readonly IBookService _bookService;

        private readonly ISeriesService _seriesService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorService _authorService;

        //private readonly ICategoryService _bookService;
        //private readonly IAuthorService _bookService;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private string _title = string.Empty;
        private string? _originalTitle;
        private string? _summary;
        private int? _firstPublishYear;
        private int _volumeNumber;
        private int _selectedSeriesId;
        private int _selectedCategoryId;
        private int _selectedAuthorId;

        // Selection wrappers for multiple selection
        public class SelectionItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool IsSelected { get; set; }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public string? OriginalTitle
        {
            get => _originalTitle;
            set { _originalTitle = value; OnPropertyChanged(); }
        }

        public string? Summary
        {
            get => _summary;
            set { _summary = value; OnPropertyChanged(); }
        }

        public int? FirstPublishYear
        {
            get => _firstPublishYear;
            set { _firstPublishYear = value; OnPropertyChanged(); }
        }

        public int VolumeNumber
        {
            get => _volumeNumber;
            set { _volumeNumber = value; OnPropertyChanged(); }
        }

        public int SelectedSeriesId
        {
            get => _selectedSeriesId;
            set { _selectedSeriesId = value; OnPropertyChanged(); }
        }

        public int SelectedCategoryId
        {
            get => _selectedCategoryId;
            set { _selectedCategoryId = value; OnPropertyChanged(); }
        }

        public int SelectedAuthorId
        {
            get => _selectedAuthorId;
            set { _selectedAuthorId = value; OnPropertyChanged(); }
        }

        // --- Danh sách cho ComboBox ---
        // original lists (kept for compatibility)
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<Author> Authors { get; set; } = new();

        // For checkbox multiple selection in the view
        public ObservableCollection<SelectionItem> CategorySelections { get; set; } = new();
        public ObservableCollection<SelectionItem> AuthorSelections { get; set; } = new();
        public ObservableCollection<Series> SeriesList { get; set; } = new();

        // --- Commands ---
        public ICommand SaveCommand { get; }
        public ICommand AddSeriesCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand AddAuthorCommand { get; }



        public AddBookViewModel(IBookService bookService, ISeriesService seriesService,ICategoryService categoryService,IAuthorService authorService)
        {
            _bookService = bookService;

            _authorService = authorService;
            _seriesService = seriesService;
            _categoryService = categoryService;

            SaveCommand = new RelayCommand(async _ => await OnSave(), _ => true);
            AddSeriesCommand = new RelayCommand(_ => OpenAddSeries());
            AddCategoryCommand = new RelayCommand(_ => OpenAddCategory());
            AddAuthorCommand = new RelayCommand(_ => OpenAddAuthor());
            _ = LoadData();
        }
        private async Task LoadData()
        {
            try
            {
                var cats = await _bookService.GetAllCategoriesAsync();
                var auths = await _bookService.GetAllAuthorsAsync();
                var series = await _bookService.GetAllSeriesAsync();

                // Đổ dữ liệu vào UI Thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Categories.Clear();
                    CategorySelections.Clear();
                    if (cats != null)
                    {
                        foreach (var c in cats)
                        {
                            Categories.Add(c);
                            CategorySelections.Add(new SelectionItem { Id = c.CategoryId, Name = c.CategoryName });
                        }
                    }

                    Authors.Clear();
                    AuthorSelections.Clear();
                    if (auths != null)
                    {
                        foreach (var a in auths)
                        {
                            Authors.Add(a);
                            AuthorSelections.Add(new SelectionItem { Id = a.AuthorId, Name = a.AuthorName });
                        }
                    }

                    SeriesList.Clear();
                    series.ForEach(s => SeriesList.Add(s));
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task OnSave()
        {
            // Kiểm tra dữ liệu cơ bản
            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Tiêu đề sách không được để trống!");
                return;
            }
           var newBook = new BookWork
           {
               Title = this.Title,
               OriginalTitle = this.OriginalTitle,
               Summary = this.Summary,
               FirstPublishYear = this.FirstPublishYear,
               SeriesId = this.SelectedSeriesId,
               VolumeNumber = this.VolumeNumber
           };
            // Build DTO for multiple authors/categories
            var dto = new LibraryManagementSystem.Services.DTOs.CreateBookWorkDto
            {
                Title = newBook.Title,
                OriginalTitle = newBook.OriginalTitle,
                Summary = newBook.Summary,
                FirstPublishYear = newBook.FirstPublishYear,
                SeriesId = newBook.SeriesId == 0 ? null : newBook.SeriesId,
                VolumeNumber = newBook.VolumeNumber,
                AuthorIds = AuthorSelections.Where(x => x.IsSelected).Select(x => x.Id).ToList(),
                CategoryIds = CategorySelections.Where(x => x.IsSelected).Select(x => x.Id).ToList()
            };

            // Validate: require at least one author and one category
            if ((dto.AuthorIds == null || dto.AuthorIds.Count == 0) || (dto.CategoryIds == null || dto.CategoryIds.Count == 0))
            {
                MessageBox.Show("Vui lòng chọn ít nhất một tác giả và một thể loại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var created = await _bookService.CreateBookWorkAsync(dto);
                if (created == null)
                {
                    MessageBox.Show("Lưu thất bại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show("Thêm sách thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Mở cửa sổ thêm Edition cho WorkId vừa tạo
                var workId = created.WorkId;
                OpenEditionWindow(workId);

                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenEditionWindow(int workId)
        {
            // Tạo service edition bằng cách khởi tạo DbContext + repository + service
            var ctx = new LibraryManagementSystem.Data.LibraryDbContext();
            var editionRepo = new LibraryManagementSystem.Repositories.BookEditionRepository(ctx);
            var editionService = new LibraryManagementSystem.Services.Implementations.BookEditionService(editionRepo);

            var vm = new LibraryManagementSystem.WPF.ViewModels.BookEditionViewModel(editionService);
            // Set workId on vm via reflection or by adding constructor; easier: set a public property WorkId if exists
            // We'll set via property if present
            try
            {
                var prop = vm.GetType().GetProperty("WorkId");
                if (prop != null && prop.CanWrite) prop.SetValue(vm, workId);
            }
            catch { }

            var win = new LibraryManagementSystem.WPF.Views.BookEditionView { DataContext = vm };
            win.ShowDialog();

            // Nếu người dùng vừa thêm edition mới, mở luôn cửa sổ AddBookCopy để thêm bản sao
            try
            {
                var newIdProp = vm.GetType().GetProperty("NewEditionId");
                if (newIdProp != null)
                {
                    var val = newIdProp.GetValue(vm) as int?;
                    if (val != null)
                    {
                        // Tạo các service cần thiết
                        var ctx2 = new LibraryManagementSystem.Data.LibraryDbContext();
                        var editionRepo2 = new LibraryManagementSystem.Repositories.BookEditionRepository(ctx2);
                        var editionService2 = new LibraryManagementSystem.Services.Implementations.BookEditionService(editionRepo2);

                        var ctx3 = new LibraryManagementSystem.Data.LibraryDbContext();
                        var bookCopyRepo = new LibraryManagementSystem.Repositories.BookCopyRepository(ctx3);
                        var bookCopyService = new LibraryManagementSystem.Services.Services.BookCopyService(bookCopyRepo);

                        var vm2 = new LibraryManagementSystem.WPF.ViewModels.AddBookCopyViewModel(editionService2, bookCopyService)
                        {
                            PreselectEditionId = val
                        };

                        var win2 = new LibraryManagementSystem.WPF.Views.AddBookCopy { DataContext = vm2 };
                        win2.ShowDialog();
                    }
                }
            }
            catch { }
        }
		private void OpenAddSeries()
		{
			var vm = new AddSeriesViewModel(_seriesService);
			var win = new AddSeriesView { DataContext = vm };
			win.ShowDialog();
			_ = LoadSeries(); // Luôn gọi hàm load lại dữ liệu sau khi cửa sổ đóng
		}

		private void OpenAddCategory()
		{
			var vm = new AddCategoryViewModel(_categoryService);
			var win = new AddCategoryView { DataContext = vm };
			win.ShowDialog();
			_ = LoadCategories();
		}

		private void OpenAddAuthor()
		{
			var vm = new AuthorViewModel(_authorService);
			var win = new AddAuthorView { DataContext = vm };
			win.ShowDialog();
			_ = LoadAuthors();
		}
		private async Task LoadSeries()
        {
            var series = await _bookService.GetAllSeriesAsync();
            Application.Current.Dispatcher.Invoke(() =>
            {
                SeriesList.Clear();
                series.ForEach(s => SeriesList.Add(s));
            });
        }

		private async Task LoadCategories()
		{
			var cats = await _bookService.GetAllCategoriesAsync();
			Application.Current.Dispatcher.Invoke(() =>
			{
				// 1. Lưu lại các Id đang được tick chọn
				var selectedIds = CategorySelections.Where(x => x.IsSelected).Select(x => x.Id).ToList();

				Categories.Clear();
				CategorySelections.Clear();

				// 2. Load lại và phục hồi trạng thái tick
				foreach (var c in cats)
				{
					Categories.Add(c);
					CategorySelections.Add(new SelectionItem
					{
						Id = c.CategoryId,
						Name = c.CategoryName,
						IsSelected = selectedIds.Contains(c.CategoryId)
					});
				}
			});
		}

		private async Task LoadAuthors()
		{
			var auths = await _bookService.GetAllAuthorsAsync();
			Application.Current.Dispatcher.Invoke(() =>
			{
				// 1. Lưu lại các Id đang được tick chọn
				var selectedIds = AuthorSelections.Where(x => x.IsSelected).Select(x => x.Id).ToList();

				Authors.Clear();
				AuthorSelections.Clear();

				// 2. Load lại và phục hồi trạng thái tick
				foreach (var a in auths)
				{
					Authors.Add(a);
					AuthorSelections.Add(new SelectionItem
					{
						Id = a.AuthorId,
						Name = a.AuthorName,
						IsSelected = selectedIds.Contains(a.AuthorId)
					});
				}
			});
		}

		private void ResetForm()
        {
            Title = string.Empty;
            OriginalTitle = null;
            Summary = null;
            FirstPublishYear = null;
            VolumeNumber = 0;
        }
    }
}