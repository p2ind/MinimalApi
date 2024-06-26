﻿using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface IGenresRepository
    {
        Task<int> CreateAsync(Genre genre);
        Task<Genre?> GetByIdAsync(int id);
        Task<List<Genre>> GetAllAsync();
        Task<bool> Exists(int id);
        Task Update(Genre genre);
        Task Delete(int id);
        Task<List<int>> Exists(List<int> ids);
        Task<bool> Exists(int id, string name);
    }
}
