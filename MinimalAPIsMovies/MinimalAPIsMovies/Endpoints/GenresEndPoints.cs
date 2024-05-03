using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Endpoints
{
    public static class GenresEndPoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenres)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"))
                .RequireAuthorization();
            group.MapGet("/{id:int}", GetById);

            group.MapPost("/", Create)
                .AddEndpointFilter<ValidationFilter<CreateGenreDTO>>()
                .RequireAuthorization("isadmin");

            group.MapPut("/{id:int}", Update)
                .AddEndpointFilter<ValidationFilter<CreateGenreDTO>>()
                 .RequireAuthorization("isadmin")
                 .WithOpenApi(options =>
                 {
                     options.Summary = "Update a genre";
                     options.Description = "with this endpoint we can update a genre";
                     options.Parameters[0].Description = "The id of the genre to update";
                     options.RequestBody.Description = "this genre to update";

                     return options;
                 }); 

            group.MapDelete("/{id:int}", Delete).RequireAuthorization("isadmin");
            return group;
        }

        static async Task<Ok<List<GenreDTO>>> GetGenres(IGenresRepository repository, 
            IMapper mapper, ILoggerFactory loggerFactory)
        {
            var type = typeof(GenresEndPoints);
            var logger = loggerFactory.CreateLogger(type.FullName!);
            logger.LogTrace("this is a trace message");
            logger.LogDebug("this is a debug message");
            logger.LogInformation("this is a information message");
            logger.LogWarning("this is a warning message");
            logger.LogError("this is a error message");
            logger.LogCritical("this is a critical message");
            // logger.LogInformation("Getting the list of genres");

            var genres = await repository.GetAllAsync();
            var genresDTO = mapper.Map<List<GenreDTO>>(genres);
            return TypedResults.Ok(genresDTO);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(int id, IGenresRepository repository, IMapper mapper)
        {
            var genre = await repository.GetByIdAsync(id);
            if (genre == null)
            {
                return TypedResults.NotFound();
            }

            var genreDTO = mapper.Map<GenreDTO>(genre);
            return TypedResults.Ok(genreDTO);
        }

        static async Task<Created<GenreDTO>> 
            Create(CreateGenreDTO createGenreDTO, IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var genre = mapper.Map<Genre>(createGenreDTO);
            var id = await repository.CreateAsync(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);

            var genreDTO = mapper.Map<GenreDTO>(genre);
            return TypedResults.Created($"/genres/{id}", genreDTO);
        }

        static async Task<Results<NotFound, NoContent>> Update(int id, CreateGenreDTO createGenreDTO, IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }


            var genre = mapper.Map<Genre>(createGenreDTO);
            genre.Id = id;

            await repository.Update(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Delete(int id, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {
            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }
    }
}
