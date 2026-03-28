using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly LibraryDbContext _context;

        public SeriesRepository(LibraryDbContext context)
        {
            _context = context;
        }

        // Thêm Series mới
        public async Task AddSeriesAsync(Series series)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(series?.SeriesName))
                throw new ArgumentException("SeriesName không được rỗng.");

            series.SeriesName = series.SeriesName.Trim();

            var exists = await _context.Series.AnyAsync(s => s.SeriesName == series.SeriesName);
            if (exists)
                throw new InvalidOperationException("Đã tồn tại series cùng tên.");

            series.SeriesId = 0;
            _context.Series.Add(series);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Rethrow with more context
                throw new Exception($"Lỗi khi lưu Series vào DB: {ex.Message}", ex);
            }
        }

        // Xóa Series theo ID
        public async Task DeleteSeriesAsync(int seriesId)
        {
            var series = await _context.Series.FindAsync(seriesId);
            if (series != null)
            {
                _context.Series.Remove(series);
                await _context.SaveChangesAsync();
            }
        }

        // Lấy tất cả Series
        public async Task<List<Series>> GetAllSeriesAsync()
        {
            return await _context.Series.ToListAsync();
        }

        // Cập nhật Series
        public async Task UpdateSeriesAsync(Series series)
        {
            var existingSeries = await _context.Series.FindAsync(series.SeriesId);
            if (existingSeries != null)
            {
                existingSeries.SeriesName = series.SeriesName;
                existingSeries.Description = series.Description;
                await _context.SaveChangesAsync();
            }
        }
    }
}