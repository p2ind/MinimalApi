using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public class GenreRepository : IGenresRepository
    {
        private readonly ApplicationDbContext _context;
        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;  
        }

        public async Task<int> CreateAsync(Genre genre)
        {
            _context.Add(genre);
            await _context.SaveChangesAsync();
            return genre.Id;
        }

        public async Task Delete(int id)
        {
           await _context.Genres.Where(g=>g.Id==id).ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(int id)
        {
          return await _context.Genres.AnyAsync(x=> x.Id == id);
        }

        public async Task<List<int>> Exists(List<int> ids)
        {
            return await _context.Genres.Where(g=> ids.Contains(g.Id)).Select(g=>g.Id).ToListAsync();
        }

        public async Task<List<Genre>> GetAllAsync()
        {
            return await _context.Genres.OrderBy(x=>x.Name).ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task Update(Genre genre)
        {
            _context.Update(genre);
            await _context.SaveChangesAsync();
        }
    }
}
