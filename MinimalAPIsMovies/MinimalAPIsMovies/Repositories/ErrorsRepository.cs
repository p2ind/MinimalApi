using Error = MinimalAPIsMovies.Entities.Error;

namespace MinimalAPIsMovies.Repositories
{
    public class ErrorsRepository : IErrorsRepository
    {
        private readonly ApplicationDbContext _context;

        public ErrorsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Error error)
        {
            _context.Add(error);
            await _context.SaveChangesAsync();
        }
    }
}
