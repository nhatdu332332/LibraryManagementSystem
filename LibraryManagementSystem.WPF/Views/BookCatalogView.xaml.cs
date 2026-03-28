using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using LibraryManagementSystem.WPF.ViewModels;

namespace LibraryManagementSystem.WPF.Views
{
	public partial class BookCatalogView : UserControl
	{
		public BookCatalogView()
		{
			InitializeComponent();

			// Sử dụng App.ServiceProvider (đã public static từ App.xaml.cs)
			if (App.ServiceProvider != null)
			{
				DataContext = App.ServiceProvider.GetRequiredService<BookCatalogViewModel>();
			}
			else
			{
				// Fallback nếu DI chưa sẵn sàng (ít xảy ra)
				throw new InvalidOperationException("ServiceProvider chưa được khởi tạo.");
			}
		}
	}
}