using LibraryManagementSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace LibraryManagementSystem.Services.Interfaces
{
    public interface ISeriesService
    {
        Task<List<Series>> GetAllSeriesAsync();
        Task<bool> AddSeriesAsync(Series series);
        Task<bool> UpdateSeriesAsync(Series series);
        Task<bool> DeleteSeriesAsync(int seriesId);
    }
}
