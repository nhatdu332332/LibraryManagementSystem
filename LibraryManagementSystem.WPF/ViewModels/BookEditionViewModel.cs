using System;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.WPF.ViewModels
{
    public class BookEditionViewModel : INotifyPropertyChanged
    {
        private readonly IBookEditionService _editionService;

        // WorkId được truyền khi mở cửa sổ để gán cho edition mới
        public int WorkId { get; set; }

        // Chỉ giữ lại danh sách Publisher cho ComboBox
        public ObservableCollection<Publisher> Publishers { get; set; } = new();

        // Các thuộc tính nhập liệu
        private string _isbn = string.Empty;
        public string ISBN { get => _isbn; set { _isbn = value; OnPropertyChanged(); } }

        private int _publishYear = DateTime.Now.Year;
        public int PublishYear { get => _publishYear; set { _publishYear = value; OnPropertyChanged(); } }

        private int? _editionNumber;
        public int? EditionNumber { get => _editionNumber; set { _editionNumber = value; OnPropertyChanged(); } }

        private string? _language;
        public string? Language { get => _language; set { _language = value; OnPropertyChanged(); } }

        private int? _pageCount;
        public int? PageCount { get => _pageCount; set { _pageCount = value; OnPropertyChanged(); } }

        private string? _format;
        public string? Format { get => _format; set { _format = value; OnPropertyChanged(); } }

        private Publisher? _selectedPublisher;
        public Publisher? SelectedPublisher { get => _selectedPublisher; set { _selectedPublisher = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand SaveAndAddCommand { get; }
        public ICommand ClearCommand { get; }

        // Khi thêm thành công, lưu lại Id của edition vừa tạo để các view khác có thể sử dụng
        public int? NewEditionId { get; private set; }

        public BookEditionViewModel(IBookEditionService editionService)
        {
            _editionService = editionService;

            SaveCommand = new RelayCommand(async (_) => await ExecuteSave());
            SaveAndAddCommand = new RelayCommand(async (_) => await ExecuteSaveAndAdd());
            ClearCommand = new RelayCommand((_) => ClearForm());

            _ = LoadPublishersAsync();
        }

        private async Task ExecuteSaveAndAdd()
        {
            // Validate ISBN và Publisher
            if (string.IsNullOrWhiteSpace(ISBN) || SelectedPublisher == null)
            {
                MessageBox.Show("Vui lòng nhập ISBN và chọn Nhà xuất bản!");
                return;
            }

            try
            {
                var edition = new BookEdition
                {
                    WorkId = this.WorkId,
                    PublisherId = SelectedPublisher.PublisherId,
                    ISBN = ISBN.Trim(),
                    PublishYear = PublishYear,
                    EditionNumber = EditionNumber,
                    Language = Language,
                    PageCount = PageCount,
                    Format = Format
                };

                bool success = await _editionService.AddEditionAsync(edition);
                if (success)
                {
                    MessageBox.Show("Thêm phiên bản thành công!");
                    try { NewEditionId = edition.EditionId; } catch { NewEditionId = null; }
                    // Không đóng cửa sổ, chỉ reset form để thêm bản ghi tiếp theo cùng WorkId
                    ClearForm();
                    // giữ WorkId as-is
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task LoadPublishersAsync()
        {
            var pubs = await _editionService.GetPublishersAsync();
            Publishers.Clear();
            foreach (var p in pubs) Publishers.Add(p);
        }

        private async Task ExecuteSave()
        {
            // Validate ISBN và Publisher
            if (string.IsNullOrWhiteSpace(ISBN) || SelectedPublisher == null)
            {
                MessageBox.Show("Vui lòng nhập ISBN và chọn Nhà xuất bản!");
                return;
            }

            try
            {
                var edition = new BookEdition
                {
                    WorkId = this.WorkId,
                    // WorkId sẽ được gán ở tầng logic khác hoặc bạn truyền vào từ ngoài
                    PublisherId = SelectedPublisher.PublisherId,
                    ISBN = ISBN.Trim(),
                    PublishYear = PublishYear,
                    EditionNumber = EditionNumber,
                    Language = Language,
                    PageCount = PageCount,
                    Format = Format
                };

                bool success = await _editionService.AddEditionAsync(edition);
                if (success)
                {
                    MessageBox.Show("Thêm phiên bản thành công!");
                    // Lấy Id mới từ đối tượng edition (EF sẽ gán sau khi SaveChanges)
                    try { NewEditionId = edition.EditionId; } catch { NewEditionId = null; }
                    // Đóng cửa sổ dialog nếu đang mở dưới dạng ShowDialog
                    var win = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
                    if (win != null)
                    {
                        try { win.DialogResult = true; } catch { }
                        win.Close();
                    }
                    else
                    {
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearForm()
        {
            ISBN = string.Empty;
            SelectedPublisher = null;
            PublishYear = DateTime.Now.Year;
            EditionNumber = null;
            Language = null;
            PageCount = null;
            Format = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}