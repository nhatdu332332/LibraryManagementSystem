using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.WPF.Helpers;
using LibraryManagementSystem.WPF.Views;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using LibraryManagementSystem.WPF.ViewModels;
public class BookManagerViewModel : INotifyPropertyChanged
{
    private BookWorkService service;

    public ObservableCollection<BookWork> BookWorks { get; set; }

    private string searchText;
    public string SearchText
    {
        get => searchText;
        set
        {
            searchText = value;
            OnPropertyChanged(nameof(SearchText));
        }
    }

    public ICommand EditCommand { get; set; }
    public ICommand SearchCommand { get; set; }
    public ICommand DeleteCommand { get; set; }
    public ICommand DetailCommand { get; set; }
    public ICommand AddCommand { get; set; } // 🔥 Thêm property AddCommand

    public BookManagerViewModel()
    {
        var context = new LibraryDbContext();
        var repo = new BookWorkRepository(context);
        service = new BookWorkService(repo);

        LoadData();

        SearchCommand = new RelayCommand(
            execute: _ => Search(),
            canExecute: _ => true
        );

        DeleteCommand = new RelayCommand(
            execute: param => Delete((int)param),
            canExecute: param => param != null
        );

        EditCommand = new RelayCommand(
            execute: param => OpenEdit(param as BookWork),
            canExecute: param => param != null
        );

        DetailCommand = new RelayCommand(param => OpenDetail((int)param), param => param != null);

     
        AddCommand = new RelayCommand(
            execute: _ => OpenAddWindow(),
            canExecute: _ => true
        );
    }
    private BookWork _selectedBook;
    public BookWork SelectedBook
    {
        get => _selectedBook;
        set
        {
            _selectedBook = value;
            OnPropertyChanged(nameof(SelectedBook));
        }
    }

    public void LoadData()
    {
        BookWorks = new ObservableCollection<BookWork>(service.GetAll());
        OnPropertyChanged(nameof(BookWorks));
    }

    public void Search()
    {
        BookWorks = new ObservableCollection<BookWork>(
            service.Search(SearchText ?? "")
        );
        OnPropertyChanged(nameof(BookWorks));
    }

    public void Delete(int id)
    {
        var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa sách này không?",
                                      "Xác nhận xóa",
                                      MessageBoxButton.YesNo,
                                      MessageBoxImage.Warning);

        if (confirm == MessageBoxResult.Yes)
        {
            try
            {
                service.Delete(id);
                MessageBox.Show("Xóa thành công!", "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
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

    private void OpenDetail(int workId)
    {
        try
        {
            var context = new LibraryManagementSystem.Data.LibraryDbContext();
            var uow = new LibraryManagementSystem.Repositories.Repositories.UnitOfWork(context);
            var vm = new LibraryManagementSystem.WPF.ViewModels.BookDetailViewModel(uow);

            var win = new LibraryManagementSystem.WPF.Views.BookDetailManagerView { DataContext = vm };

            // Load data async then show dialog
            _ = vm.LoadDetailsAsync(workId).ContinueWith(t =>
            {
                // show on UI thread
                Application.Current.Dispatcher.Invoke(() => win.ShowDialog());
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi mở chi tiết sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
  
    // INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    private void OpenEdit(BookWork book)
    {
        if (book == null)
        {
            MessageBox.Show("Không có dữ liệu!");
            return;
        }

        var view = new EditBookView();
        view.DataContext = new EditBookViewModel(book);

        view.ShowDialog();

        LoadData();
    }

}
