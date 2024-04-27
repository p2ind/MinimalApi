using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Repositories;
using MinimalAPIsMovies.Services;

namespace MinimalAPIsMovies.Endpoints
{
    public static class CommetnsEndpoints
    {
        public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
        {
            group.MapGet("/",GetAll).CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("comments-get"));
            group.MapGet("/{id:int}", GetById);

            group.MapPost("/", Create)
                .AddEndpointFilter<ValidationFilter<CreateCommentDTO>>()
                .RequireAuthorization();

            group.MapPut("/{id:int}", Update)
                .AddEndpointFilter<ValidationFilter<CreateCommentDTO>>()
                .RequireAuthorization();

            group.MapDelete("/{id:int}", Delete).RequireAuthorization();
            return group;
        }

        static async Task<Results<Ok<List<CommentDTO>>, NotFound>> GetAll(int movieId, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var comments = await commentsRepository.GetAll(movieId);
            var commentDTO = mapper.Map<List<CommentDTO>>(comments);
            return TypedResults.Ok(commentDTO);
        }

        static async Task<Results<Ok<CommentDTO>, NotFound>> GetById(int movieId, int id, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var comment = await commentsRepository.GetById(id);

            if(comment is null)
            {
                return TypedResults.NotFound();
            }

            var commentDTO = mapper.Map<CommentDTO>(comment);
            return TypedResults.Ok(commentDTO);
        }

        static async Task<Results<Created<CommentDTO>, NotFound, BadRequest<string>>> Create(int movieId, CreateCommentDTO createCommentDTO, ICommentsRepository repository, IMoviesRepository moviesRepository, IMapper mapper, IOutputCacheStore outputCacheStore, IUserService userService)
        {
            if(!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var user = await userService.GetUser();

            if(user is null)
            {
                return TypedResults.BadRequest("user not found");
            }

            var comments = mapper.Map<Comment>(createCommentDTO);
            comments.MovieId = movieId;
            comments.UserId = user.Id;

            var id = await repository.Create(comments);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            var commentDTO = mapper.Map<CommentDTO>(comments);
            return TypedResults.Created($"/comments/{id}", commentDTO);
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Update(int movieId, int id,CreateCommentDTO createCommentDTO, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper, IOutputCacheStore outputCacheStore,IUserService userService)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var commentFromDB = await commentsRepository.GetById(id);

            if(commentFromDB is null)
            {
                return TypedResults.NotFound();
            }
            
            var user = await userService.GetUser();

            if(user is null)
            {
                return TypedResults.NotFound();
            }

            if(commentFromDB.UserId != user.Id)
            {
                return TypedResults.Forbid();
            }

            commentFromDB.Body = createCommentDTO.Body;

            await commentsRepository.Update(commentFromDB);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Delete(int movieId, int id, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository,  IOutputCacheStore outputCacheStore, IUserService userService)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var commentFromDB = await commentsRepository.GetById(id);

            if (commentFromDB is null)
            {
                return TypedResults.NotFound();
            }

            var user = await userService.GetUser();

            if (user is null)
            {
                return TypedResults.NotFound();
            }

            if (commentFromDB.UserId != user.Id)
            {
                return TypedResults.Forbid();
            }

            await commentsRepository.Delete(id);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }
    }
}
