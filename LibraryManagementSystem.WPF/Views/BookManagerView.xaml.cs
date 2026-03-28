using System.Windows.Controls;

namespace LibraryManagementSystem.WPF.Views
{
    /// <summary>
    /// Interaction logic for BookManagerView.xaml
    /// </summary>
    public partial class BookManagerView : UserControl
    {
        public BookManagerView()
        {
            InitializeComponent();
            this.DataContext = new BookManagerViewModel();
        }
    }
}