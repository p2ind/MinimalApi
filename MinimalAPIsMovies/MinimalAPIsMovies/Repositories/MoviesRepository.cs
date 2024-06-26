﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using System.Linq.Dynamic.Core;

namespace MinimalAPIsMovies.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public MoviesRepository(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, IMapper mapper, ILogger<MoviesRepository> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<Movie>> GetAll(PaginationDTO pagination)
        {
            var queryable = _context.Movies.AsQueryable();
            await _httpContextAccessor.HttpContext!.InsertPaginationParameterInResponseHeader(queryable);
            return await queryable.OrderBy(m => m.Title).Paginate(pagination).ToListAsync();
        }

        public async Task<Movie?> GetById(int id)
        {
            return await _context.Movies
                .Include(m=>m.Comments)
                .Include(m=>m.GenresMovies)
                    .ThenInclude(gm=>gm.Genre)
                .Include(m=>m.ActorMovies.OrderBy(am=>am.Order))
                    .ThenInclude(am=>am.Actor)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Movies.AnyAsync(m=>m.Id== id);
        }

        public async Task<int> Create(Movie movie)
        {
            _context.Add(movie);
            await _context.SaveChangesAsync();
            return movie.Id;
        }

        public async Task Update(Movie movie)
        {
            _context.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Movies.Where(m => m.Id == id).ExecuteDeleteAsync();
        }

        public async Task Assign(int id, List<int> genresIds)
        {
            var movie = await _context.Movies.Include(m => m.GenresMovies).FirstOrDefaultAsync(m=>m.Id ==id);

            if(movie is null)
            {
                throw new ArgumentException($"Thiere's no movie with id {id}");
            }

            var genresMovies = genresIds.Select(genreId => new GenreMovie { GenreId = genreId });

            movie.GenresMovies = _mapper.Map(genresMovies, movie.GenresMovies);

            await _context.SaveChangesAsync();
        }

        public async Task Assign(int id, List<ActorMovie> actors)
        {
            for (int i = 1; i <= actors.Count; i++)
            {
                actors[i - 1].Order = i;
            }

            var movie = await _context.Movies.Include(m => m.ActorMovies).FirstOrDefaultAsync();

            if(movie is null)
            {
                throw new ArgumentException($"There's not movie with id {id}");
            }

            movie.ActorMovies = _mapper.Map(actors,movie.ActorMovies);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Movie>> Filter(MovieFilterDTO movieFilterDTO)
        {
            var moviesQueryable = _context.Movies.AsQueryable();

            if(!string.IsNullOrEmpty(movieFilterDTO.Title))
            {
                moviesQueryable = moviesQueryable
                                    .Where(x => x.Title.Contains(movieFilterDTO.Title));
            }

            if(movieFilterDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable
                                    .Where(x => x.InTheaters);
            }

            if (movieFilterDTO.FutureReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable
                                    .Where(x => x.ReleaseDate > today);
            }

            if (movieFilterDTO.GenreId !=0)
            {
                moviesQueryable = moviesQueryable
                                    .Where(x => x.GenresMovies
                                    .Select(y=>y.GenreId)
                                    .Contains(movieFilterDTO.GenreId));
            }

            // TODO: work on the order
            if(!string.IsNullOrEmpty(movieFilterDTO.OrderByField))
            {
                var orderKind = movieFilterDTO.OrderByAscending ? "ascending" : "descending";

                try
                {
                    //title ascending, title descending, releaseDate ascending
                    moviesQueryable = moviesQueryable.
                                        OrderBy($"{movieFilterDTO.OrderByField} {orderKind}");
                }
                catch (Exception ex)
                {
;                  _logger.LogError(ex.Message, ex);
                }
            }


            await _httpContextAccessor.HttpContext!
                .InsertPaginationParameterInResponseHeader(moviesQueryable);

            var movies = await moviesQueryable.Paginate(movieFilterDTO.PaginationDTO).ToListAsync();

            return movies;
        }
    }
}
