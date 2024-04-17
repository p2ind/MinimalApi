using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public class ActorsRepository : IActorsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public ActorsRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _contextAccessor = httpContextAccessor;
        }

        public async Task<List<Actor>> GetAll(PaginationDTO pagination)
        {
            var queryable = _context.Actors.AsQueryable();
            await _contextAccessor
                    .HttpContext!
                    .InsertPaginationParameterInResponseHeader(queryable);
            return await queryable.OrderBy(a => a.Name).Paginate(pagination).ToListAsync();
        }

        public async Task<Actor?> GetById(int id)
        {
            return await _context.Actors.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Actor>> GetByName(string name)
        {
            return await _context.Actors
                .Where(a=>a.Name.Contains(name))
                .OrderBy(a=>a.Name).ToListAsync();
        }

        public async Task<int> Create(Actor actor)
        {
            _context.Add(actor);
            await _context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task<bool> Exist(int id)
        {
            return await _context.Actors.AnyAsync(a => a.Id == id);
        }

        public async Task Update(Actor actor)
        {
            _context.Add(actor);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Actors.Where(a=>a.Id==id).ExecuteDeleteAsync();
        }
    }
}
