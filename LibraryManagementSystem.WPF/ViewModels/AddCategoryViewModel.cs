using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.Helpers; // Chứa RelayCommand của bạn
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.WPF.ViewModels
{
    public class AddCategoryViewModel : INotifyPropertyChanged
    {
        private readonly ICategoryService _categoryService;

        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

		

		private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();

                // Khi người dùng chọn một dòng trong bảng, đổ dữ liệu vào TextBox
                if (_selectedCategory != null)
                {
                    CategoryName = _selectedCategory.CategoryName;
                    Description = _selectedCategory.Description;
                }
            }
        }

        private string _categoryName;
        public string CategoryName
        {
            get => _categoryName;
            set { _categoryName = value; OnPropertyChanged(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        // Các Command tương ứng với giao diện mới
        public ICommand SaveOrUpdateCommand { get; } // Gộp Add/Update vào 1 nút giống hình mẫu
        public ICommand DeleteCommand { get; }
        public ICommand ClearFormCommand { get; }
		public ICommand ClearCommand { get; }

		public AddCategoryViewModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            SaveOrUpdateCommand = new RelayCommand(async _ => await SaveOrUpdateAsync(), _ => !string.IsNullOrWhiteSpace(CategoryName));
            DeleteCommand = new RelayCommand(async _ => await DeleteCategoryAsync(), _ => SelectedCategory != null);
            ClearFormCommand = new RelayCommand(_ => ClearFields());
			ClearCommand = new RelayCommand(ClearForm);

			LoadCategories();
        }

		private void ClearForm(object obj)
		{
			SelectedCategory = null; // Bỏ chọn item đang edit
			CategoryName = string.Empty;
			Description = string.Empty;
			// Đảm bảo các thuộc tính này đã gọi OnPropertyChanged()
		}

		private async void LoadCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            Categories.Clear();
            foreach (var cat in categories)
            {
                Categories.Add(cat);
            }
        }

        private async Task SaveOrUpdateAsync()
        {
            try
            {
                if (SelectedCategory == null)
                {
                    // ADD
                    var category = new Category
                    {
                        CategoryName = CategoryName,
                        Description = Description
                    };

                    await _categoryService.AddCategoryAsync(category);
                    LoadCategories();

                    MessageBox.Show("Thêm category thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // UPDATE
                    SelectedCategory.CategoryName = CategoryName;
                    SelectedCategory.Description = Description;

                    await _categoryService.UpdateCategoryAsync(SelectedCategory);

                    var tempIndex = Categories.IndexOf(SelectedCategory);
                    Categories[tempIndex] = SelectedCategory;

                    LoadCategories();

                    MessageBox.Show("Cập nhật category thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

		private async Task DeleteCategoryAsync()
		{
			if (SelectedCategory == null) return;

			var confirm = MessageBox.Show(
				$"Bạn có chắc chắn muốn xóa thể loại '{SelectedCategory.CategoryName}'?",
				"Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (confirm != MessageBoxResult.Yes) return;

			try
			{
				await _categoryService.DeleteCategoryAsync(SelectedCategory.CategoryId);

				MessageBox.Show("Xóa category thành công!", "Thành công",
							   MessageBoxButton.OK, MessageBoxImage.Information);

				LoadCategories();   // refresh danh sách
				ClearFields();
			}
			catch (Exception ex)
			{
				// Hiển thị lỗi chi tiết hơn
				string msg = ex.InnerException?.Message ?? ex.Message;
				MessageBox.Show($"Không thể xóa:\n{msg}", "Lỗi",
							   MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ClearFields()
        {
            SelectedCategory = null;
            CategoryName = string.Empty;
            Description = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}