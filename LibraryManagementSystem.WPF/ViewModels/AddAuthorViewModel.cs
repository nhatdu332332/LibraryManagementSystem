using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.WPF.ViewModels
{
    public class AuthorViewModel : INotifyPropertyChanged
    {
        private readonly IAuthorService _authorService;

        // ObservableCollection nên dùng để chứa danh sách hiển thị
        public ObservableCollection<Author> Authors { get; set; } = new ObservableCollection<Author>();

        private Author? _selectedAuthor;
        public Author? SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                OnPropertyChanged();
                if (_selectedAuthor != null)
                {
                    AuthorName = _selectedAuthor.AuthorName;
                    Note = _selectedAuthor.Note;
                }
            }
        }

        private string _authorName = string.Empty;
        public string AuthorName
        {
            get => _authorName;
            set { _authorName = value; OnPropertyChanged(); }
        }

        private string? _note;
        public string? Note
        {
            get => _note;
            set { _note = value; OnPropertyChanged(); }
        }

        public ICommand SaveOrUpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }

        public AuthorViewModel(IAuthorService authorService)
        {
            _authorService = authorService;

            // SỬA LỖI: Cần truyền object vào Action của RelayCommand nếu nó yêu cầu
            SaveOrUpdateCommand = new RelayCommand(async (obj) => await ExecuteSaveOrUpdate());
            DeleteCommand = new RelayCommand(async (obj) => await ExecuteDelete(), (obj) => SelectedAuthor != null);
            AddCommand = new RelayCommand((obj) => ClearForm());

            // Tải dữ liệu ban đầu
            _ = LoadAuthorsAsync();
        }

        private async Task LoadAuthorsAsync()
        {
            try
            {
                var data = await _authorService.GetAllAuthorAsync();
                Authors.Clear();
                if (data != null)
                {
                    foreach (var author in data)
                    {
                        Authors.Add(author);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách: {ex.Message}");
            }
        }

        private async Task ExecuteSaveOrUpdate()
        {
            if (string.IsNullOrWhiteSpace(AuthorName))
            {
                MessageBox.Show("Vui lòng nhập tên tác giả!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string trimmedName = AuthorName.Trim();

            bool isDuplicate = Authors.Any(a =>
                a.AuthorName.Equals(trimmedName, StringComparison.OrdinalIgnoreCase) &&
                (SelectedAuthor == null || a.AuthorId != SelectedAuthor.AuthorId));
            if (isDuplicate)
            {
                MessageBox.Show($"Tác giả '{trimmedName}' đã tồn tại trong hệ thống!", "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                if (SelectedAuthor != null)
                {
                    // Cập nhật
                    SelectedAuthor.AuthorName = trimmedName;
                    SelectedAuthor.Note = Note;
                    var result = await _authorService.UpdateAuthorAsync(SelectedAuthor);
                    if (result) MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Thêm mới
                    var newAuthor = new Author { AuthorName = trimmedName, Note = Note };
                    var result = await _authorService.AddAuthorAsync(newAuthor);
                    if (result != null) MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                await LoadAuthorsAsync();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ExecuteDelete()
        {
            if (SelectedAuthor == null) return;

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa tác giả '{SelectedAuthor.AuthorName}'?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var isDeleted = await _authorService.DeleteAuthorAsync(SelectedAuthor.AuthorId);
                if (isDeleted)
                {
                    MessageBox.Show("Xóa tác giả thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadAuthorsAsync();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Xóa tác giả thất bại (có thể do ràng buộc dữ liệu).", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            SelectedAuthor = null;
            AuthorName = string.Empty;
            Note = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}