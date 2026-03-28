using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly ISeriesRepository _repo;

        public SeriesService(ISeriesRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Series>> GetAllSeriesAsync()
        {
            return await _repo.GetAllSeriesAsync();
        }

        public async Task<bool> AddSeriesAsync(Series series)
        {
            try
            {
                await _repo.AddSeriesAsync(series);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateSeriesAsync(Series series)
        {
            try
            {
                await _repo.UpdateSeriesAsync(series);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteSeriesAsync(int seriesId)
        {
            try
            {
                await _repo.DeleteSeriesAsync(seriesId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}