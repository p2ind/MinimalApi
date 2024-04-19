using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public class CommentsRepository : ICommentsRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetAll(int movieId)
        {
            return await _context.Comments.Where(c => c.MovieId == movieId).ToListAsync();
        }

        public async Task<Comment?> GetById(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> Create(Comment comment)
        {
            _context.Add(comment);
            await _context.SaveChangesAsync();
            return comment.Id;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Comments.AnyAsync(c => c.Id == id);
        }

        public async Task Update(Comment comment)
        {
            _context.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Comments.Where(c => c.Id == id).ExecuteDeleteAsync();
        }
    }
}
