using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.WPF.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.WPF.ViewModels
{
	public class BookDetailViewModel : ObservableObject
	{
		private readonly IUnitOfWork _uow;

		// --- Thuộc tính cho Cột Trái ---
		private string _workTitle = string.Empty;
		public string WorkTitle { get => _workTitle; set => SetProperty(ref _workTitle, value); }

		private string _originalTitle = string.Empty;
		public string OriginalTitle { get => _originalTitle; set => SetProperty(ref _originalTitle, value); }

		private string _summary = string.Empty;
		public string Summary { get => _summary; set => SetProperty(ref _summary, value); }

		private string _firstPublishYear = string.Empty;
		public string FirstPublishYear { get => _firstPublishYear; set => SetProperty(ref _firstPublishYear, value); }

		private string _seriesInfo = string.Empty;
		public string SeriesInfo { get => _seriesInfo; set => SetProperty(ref _seriesInfo, value); }

		private string _authors = string.Empty;
		public string Authors { get => _authors; set => SetProperty(ref _authors, value); }

		private string _categories = string.Empty;
		public string Categories { get => _categories; set => SetProperty(ref _categories, value); }

		// --- Thuộc tính cho Cột Phải ---
		public ObservableCollection<EditionDetailModel> Editions { get; } = new();

		public BookDetailViewModel(IUnitOfWork uow)
		{
			_uow = uow;
		}

		public async Task LoadDetailsAsync(int workId)
		{
			// Dùng Entity Framework Include để lôi sạch sành sanh dữ liệu từ DB lên
			var work = await _uow.DbContext.BookWorks
				.Include(w => w.Series)
				.Include(w => w.WorkAuthors).ThenInclude(wa => wa.Author)
				.Include(w => w.WorkCategories).ThenInclude(wc => wc.Category)
				.Include(w => w.BookEditions).ThenInclude(e => e.Publisher)
				.Include(w => w.BookEditions).ThenInclude(e => e.BookCopies)
				.FirstOrDefaultAsync(w => w.WorkId == workId);

			if (work == null) return;

			// Map dữ liệu cột trái
			WorkTitle = work.Title;
			OriginalTitle = work.OriginalTitle ?? "N/A";
			Summary = work.Summary ?? "Không có tóm tắt.";
			FirstPublishYear = work.FirstPublishYear?.ToString() ?? "N/A";

			SeriesInfo = work.Series != null
				? $"{work.Series.SeriesName} (Tập {work.VolumeNumber})"
				: "Không thuộc series nào";

			Authors = work.WorkAuthors.Any()
				? string.Join(", ", work.WorkAuthors.Select(wa => wa.Author.AuthorName))
				: "Chưa cập nhật";

			Categories = work.WorkCategories.Any()
				? string.Join(", ", work.WorkCategories.Select(wc => wc.Category.CategoryName))
				: "Chưa cập nhật";

			// Map dữ liệu cột phải (List Edition kèm theo List Copies bên trong)
			Editions.Clear();
			foreach (var edition in work.BookEditions)
			{
				var editionModel = new EditionDetailModel
				{
					EditionTitle = $"ISBN: {edition.ISBN} | Định dạng: {edition.Format ?? "N/A"} | {edition.PageCount} trang | Lần XB: {edition.EditionNumber}",
					PublisherInfo = $"Nhà xuất bản: {edition.Publisher.PublisherName} (Năm XB: {edition.PublishYear})",
					Copies = new ObservableCollection<CopyDetailModel>(
						edition.BookCopies.Select(c => new CopyDetailModel
						{
							Barcode = c.Barcode,
							CirculationStatus = c.CirculationStatus,
							PhysicalCondition = c.PhysicalCondition,
							ShelfLocation = c.ShelfLocation ?? "Chưa xếp kệ"
						}))
				};
				Editions.Add(editionModel);
			}
		}
	}

	// Model hỗ trợ hiển thị UI (Nested data)
	public class EditionDetailModel
	{
		public string EditionTitle { get; set; } = string.Empty;
		public string PublisherInfo { get; set; } = string.Empty;
		public ObservableCollection<CopyDetailModel> Copies { get; set; } = new();
	}

	public class CopyDetailModel
	{
		public string Barcode { get; set; } = string.Empty;
		public string CirculationStatus { get; set; } = string.Empty;
		public string PhysicalCondition { get; set; } = string.Empty;
		public string ShelfLocation { get; set; } = string.Empty;
	}
}