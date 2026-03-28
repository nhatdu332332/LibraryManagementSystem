using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers; // chứa RelayCommand
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LibraryManagementSystem.WPF.ViewModels
{
    public class BookDetailManagerViewModel : INotifyPropertyChanged
    {
        private readonly IBookService _bookWorkService;

        public BookDetailManagerViewModel(IBookService bookWorkService, int workId)
        {
            _bookWorkService = bookWorkService;
            BookEditions = new ObservableCollection<BookEdition>();
            LoadBookWorkDetail(workId);
        }

        #region Properties with INotifyPropertyChanged
        private int _workId;
        public int WorkId { get => _workId; set { _workId = value; OnPropertyChanged(); } }

        private string _title = string.Empty;
        public string Title { get => _title; set { _title = value; OnPropertyChanged(); } }

        private string? _originalTitle;
        public string? OriginalTitle { get => _originalTitle; set { _originalTitle = value; OnPropertyChanged(); } }

        private string? _summary;
        public string? Summary { get => _summary; set { _summary = value; OnPropertyChanged(); } }

        private int? _firstPublishYear;
        public int? FirstPublishYear { get => _firstPublishYear; set { _firstPublishYear = value; OnPropertyChanged(); } }

        private int _volumeNumber;
        public int VolumeNumber { get => _volumeNumber; set { _volumeNumber = value; OnPropertyChanged(); } }

        private string _seriesName = string.Empty;
        public string SeriesName { get => _seriesName; set { _seriesName = value; OnPropertyChanged(); } }

        private string _categoryNames = string.Empty;
        public string CategoryNames { get => _categoryNames; set { _categoryNames = value; OnPropertyChanged(); } }

        private string _authorNames = string.Empty;
        public string AuthorNames { get => _authorNames; set { _authorNames = value; OnPropertyChanged(); } }

        public ObservableCollection<BookEdition> BookEditions { get; set; }
        #endregion

        private async void LoadBookWorkDetail(int workId)
        {
            var work = await _bookWorkService.GetBookWorkDetailAsync(workId);
            if (work != null)
            {
                WorkId = work.WorkId;
                Title = work.Title;
                OriginalTitle = work.OriginalTitle;
                Summary = work.Summary;
                FirstPublishYear = work.FirstPublishYear;
                VolumeNumber = work.VolumeNumber;

                SeriesName = work.Series?.SeriesName ?? "";
                AuthorNames = string.Join(", ", work.WorkAuthors.Select(a => a.Author.AuthorName));
                CategoryNames = string.Join(", ", work.WorkCategories.Select(c => c.Category.CategoryName));

                BookEditions.Clear();
                foreach (var edition in work.BookEditions)
                    BookEditions.Add(edition);
            }
        }

        #region Optional: RelayCommand mở BookEdition detail
        private ICommand? _openEditionDetailCommand;
        public ICommand OpenEditionDetailCommand
        {
            get
            {
                return _openEditionDetailCommand ??= new RelayCommand<BookEdition>((edition) =>
                {
                    // TODO: mở BookEdition detail window
                    // var vm = new BookEditionDetailViewModel(edition);
                    // var window = new BookEditionDetailView(vm);
                    // window.ShowDialog();
                });
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}