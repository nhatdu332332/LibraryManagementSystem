using System.Windows;

namespace LibraryManagementSystem.WPF.Views
{
	public partial class CreateBorrowTransactionWindow : Window
	{
		public CreateBorrowTransactionWindow()
		{
			InitializeComponent();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

	}
}