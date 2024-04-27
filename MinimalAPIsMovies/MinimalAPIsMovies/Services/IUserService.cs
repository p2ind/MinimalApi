using Microsoft.AspNetCore.Identity;

namespace MinimalAPIsMovies.Services
{
    public interface IUserService
    {
        Task<IdentityUser?> GetUser();
    }
}