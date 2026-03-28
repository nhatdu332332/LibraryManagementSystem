using LibraryManagementSystem.WPF.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace LibraryManagementSystem.WPF.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }
		// Trong LoginView.xaml.cs

		private bool ValidateLoginForm()
		{
			// 1. Kiểm tra rỗng
			if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
			{
				MessageBox.Show("Vui lòng nhập đầy đủ Email và Mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
				return false;
			}

			// 2. Kiểm tra định dạng Email (Sử dụng Regex giống phần Register của bạn)
			if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			{
				MessageBox.Show("Email không đúng định dạng!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			return true;
		}

		// Sau đó, tại sự kiện Click của nút Đăng nhập:
		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
			if (ValidateLoginForm())
			{
				// Gọi lệnh xử lý đăng nhập từ ViewModel
				var viewModel = (LoginViewModel)this.DataContext;
				viewModel.LoginCommand.Execute(null);
			}
		}

		// Hiện form đăng ký
		private void ShowRegisterForm(object sender, RoutedEventArgs e)
		{
			RegisterPanel.Visibility = Visibility.Visible;
		}

		private void HideRegisterForm(object sender, RoutedEventArgs e)
		{
			RegisterPanel.Visibility = Visibility.Collapsed;
		}

		private void CheckRegisterBeforeSubmit(object sender, MouseButtonEventArgs e)
		{
			if (!ValidateRegisterForm())
			{
				// Ngăn không cho Command thực thi nếu validate fail
				e.Handled = true;
			}
		}

		private bool ValidateRegisterForm()
		{
			if (string.IsNullOrWhiteSpace(txtRegisterEmail.Text) ||
				string.IsNullOrWhiteSpace(txtRegisterPassword.Password) ||
				string.IsNullOrWhiteSpace(txtFullName.Text) ||
				string.IsNullOrWhiteSpace(txtPhone.Text) ||
				string.IsNullOrWhiteSpace(txtAddress.Text))
			{
				MessageBox.Show("Vui lòng nhập đầy đủ 5 thông tin!");
				return false;
			}

			if (!Regex.IsMatch(txtRegisterEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			{
				MessageBox.Show("Email không đúng định dạng!");
				return false;
			}

			if (!Regex.IsMatch(txtPhone.Text, @"^[0-9]{9,11}$"))
			{
				MessageBox.Show("Số điện thoại phải là số (9-11 ký tự)!");
				return false;
			}

			return true;
		}

		// Chặn đăng ký nếu form sai

    }
}