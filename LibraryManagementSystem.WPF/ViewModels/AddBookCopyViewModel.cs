using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System;
using System.Linq;

namespace LibraryManagementSystem.WPF.ViewModels
{
    public class AddBookCopyViewModel : INotifyPropertyChanged
    {
        public int? PreselectEditionId { get; set; }

    private readonly IBookEditionService _editionService;
    private readonly IBookCopyService _bookCopyService;

    public AddBookCopyViewModel(IBookEditionService editionService,
                                IBookCopyService bookCopyService)
    {
        _editionService = editionService;
        _bookCopyService = bookCopyService;

        SaveAndCloseCommand = new RelayCommand(async _ => await SaveAndClose());
        SaveAndAddCommand = new RelayCommand(async _ => await SaveAndAdd());
        CancelCommand = new RelayCommand(_ => Cancel());

        // default
        AddedDate = DateTime.Now;
        CirculationStatus = "Available";
        PhysicalCondition = "Good";

        LoadBookEditions();
    }

    // ================== LIST ==================
    public ObservableCollection<BookEdition> BookEditions { get; set; }
        = new ObservableCollection<BookEdition>();

    public List<string> StatusList { get; set; } = new List<string>
    {
        "Available", "Borrowed", "Reserved", "Damaged", "Lost"
    };

    public List<string> ConditionList { get; set; } = new List<string>
    {
        "Excellent", "Good", "Fair", "Poor"
    };

    // ================== SELECT ==================
    private BookEdition _selectedEdition;
    public BookEdition SelectedEdition
    {
        get => _selectedEdition;
        set { _selectedEdition = value; OnPropertyChanged(); }
    }

    // ================== INPUT ==================
    private string _barcode;
    public string Barcode
    {
        get => _barcode;
        set { _barcode = value; OnPropertyChanged(); }
    }

    private string _circulationStatus;
    public string CirculationStatus
    {
        get => _circulationStatus;
        set { _circulationStatus = value; OnPropertyChanged(); }
    }

    private string _physicalCondition;
    public string PhysicalCondition
    {
        get => _physicalCondition;
        set { _physicalCondition = value; OnPropertyChanged(); }
    }

    private string _shelfLocation;
    public string ShelfLocation
    {
        get => _shelfLocation;
        set { _shelfLocation = value; OnPropertyChanged(); }
    }

    private DateTime _addedDate;
    public DateTime AddedDate
    {
        get => _addedDate;
        set { _addedDate = value; OnPropertyChanged(); }
    }

        public ICommand SaveAndCloseCommand { get; }
        public ICommand SaveAndAddCommand { get; }
        public ICommand CancelCommand { get; }

    private async void LoadBookEditions()
    {
        try
        {
            var list = await _editionService.GetAllAsync();

            BookEditions.Clear();
            foreach (var item in list)
            {
                BookEditions.Add(item);
            }

            // auto chọn item đầu hoặc theo PreselectEditionId nếu có
            if (PreselectEditionId != null)
            {
                SelectedEdition = BookEditions.FirstOrDefault(e => e.EditionId == PreselectEditionId.Value) ?? BookEditions.FirstOrDefault();
            }
            else
            {
                SelectedEdition = BookEditions.FirstOrDefault();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi load BookEdition: " + ex.Message);
        }
    }

    // ================== SAVE ==================
    private async Task<bool> SaveInternal()
    {
        try
        {
            // validate
            if (SelectedEdition == null || string.IsNullOrWhiteSpace(Barcode))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return false;
            }

            var bookCopy = new BookCopy
            {
                EditionId = SelectedEdition.EditionId,
                Barcode = Barcode.Trim(),
                CirculationStatus = CirculationStatus,
                PhysicalCondition = PhysicalCondition,
                ShelfLocation = ShelfLocation,
                AddedDate = AddedDate
            };

            bool result = await _bookCopyService.AddBookCopyAsync(bookCopy);

            if (!result)
            {
                MessageBox.Show("Barcode đã tồn tại!");
                return false;
            }

            MessageBox.Show("Thêm thành công!");

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khi thêm: " + ex.Message);
            return false;
        }
    }

    private async Task SaveAndAdd()
    {
        var ok = await SaveInternal();
        if (!ok) return;

        // reset form for next entry
        Barcode = "";
        ShelfLocation = "";
        CirculationStatus = "Available";
        PhysicalCondition = "Good";
        AddedDate = DateTime.Now;

        OnPropertyChanged(nameof(Barcode));
        OnPropertyChanged(nameof(ShelfLocation));
        OnPropertyChanged(nameof(CirculationStatus));
        OnPropertyChanged(nameof(PhysicalCondition));
        OnPropertyChanged(nameof(AddedDate));
    }

    private async Task SaveAndClose()
    {
        var ok = await SaveInternal();
        if (!ok) return;

        // close window
        var win = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
        if (win != null) { try { win.DialogResult = true; } catch { } win.Close(); }
    }

    private void Cancel()
    {
        // TODO: nếu có window thì đóng
        MessageBox.Show("Cancel clicked");
    }


    // ================== INotify ==================

    // ================== INotify ==================
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

}
