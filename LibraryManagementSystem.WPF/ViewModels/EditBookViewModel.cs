using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.WPF.Helpers;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.WPF.ViewModels
{
    public class EditBookViewModel
    {
        private BookWork _book;

        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Summary { get; set; }
        public int? FirstPublishYear { get; set; }

        public ICommand SaveCommand { get; }

        public EditBookViewModel(BookWork book)
        {
            _book = book;

            Title = book.Title;
            OriginalTitle = book.OriginalTitle;
            Summary = book.Summary;
            FirstPublishYear = book.FirstPublishYear;

            SaveCommand = new RelayCommand(Save);
        }

        private void Save(object obj)
        {
            _book.Title = Title;
            _book.OriginalTitle = OriginalTitle;
            _book.Summary = Summary;
            _book.FirstPublishYear = FirstPublishYear;

            MessageBox.Show("Cập nhật thành công!");

            if (obj is Window w)
                w.Close();
        }
    }
}