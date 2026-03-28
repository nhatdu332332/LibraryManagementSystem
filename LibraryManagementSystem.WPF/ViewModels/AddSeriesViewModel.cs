using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.WPF.ViewModels
{
    public class AddSeriesViewModel : INotifyPropertyChanged
    {
        private readonly ISeriesService _seriesService;

        public ObservableCollection<Series> SeriesList { get; set; } = new();

        private Series _selectedSeries;
        public Series SelectedSeries
        {
            get => _selectedSeries;
            set
            {
                _selectedSeries = value;
                OnPropertyChanged();
                if (value != null)
                {
                    SeriesName = value.SeriesName;
                    Description = value.Description;
                }
            }
        }

        private string _seriesName;
        public string SeriesName { get => _seriesName; set { _seriesName = value; OnPropertyChanged(); } }

        private string _description;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        public AddSeriesViewModel(ISeriesService seriesService)
        {
            _seriesService = seriesService;

            SaveCommand = new RelayCommand(async _ => await SaveSeries());
            DeleteCommand = new RelayCommand(async _ => await DeleteSeries());
            ClearCommand = new RelayCommand(_ => ClearForm());

            _ = LoadData();
        }

        public async Task LoadData()
        {
            try
            {
                var list = await _seriesService.GetAllSeriesAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SeriesList.Clear();
                    if (list != null)
                    {
                        foreach (var item in list) SeriesList.Add(item);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp Series: {ex.Message}");
            }
        }

        private async Task SaveSeries()
        {
            if (string.IsNullOrWhiteSpace(SeriesName)) return;

            if (SelectedSeries == null)
            {
                var newSeries = new Series { SeriesName = SeriesName, Description = Description };
                try
                {
                    var added = await _seriesService.AddSeriesAsync(newSeries);
                    if (!added)
                    {
                        MessageBox.Show("Không thêm được Series. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    MessageBox.Show("Thêm Series thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm Series: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                SelectedSeries.SeriesName = SeriesName;
                SelectedSeries.Description = Description;
                try
                {
                    var updated = await _seriesService.UpdateSeriesAsync(SelectedSeries);
                    if (!updated)
                    {
                        MessageBox.Show("Cập nhật Series thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    MessageBox.Show("Cập nhật Series thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật Series: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            await LoadData();
            ClearForm();
        }

        private async Task DeleteSeries()
        {
            if (SelectedSeries != null)
            {
                var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa series này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm != MessageBoxResult.Yes) return;

                try
                {
                    var deleted = await _seriesService.DeleteSeriesAsync(SelectedSeries.SeriesId);
                    if (!deleted)
                    {
                        MessageBox.Show("Xóa Series thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    MessageBox.Show("Xóa Series thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa Series: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await LoadData();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            SelectedSeries = null;
            SeriesName = string.Empty;
            Description = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}