using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

var builder = WebApplication.CreateBuilder(args);

#region services zone - BEGIN

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("name=DefaultConnection"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["allowedOrigins"]!)
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });

    options.AddPolicy("free", configuration =>
    {
        configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGenresRepository, GenreRepository>();

#endregion services zone - end

var app = builder.Build();

#region Middlewares zone - BEGION

app.UseSwagger();
app.UseSwaggerUI();


app.UseCors();

app.UseOutputCache();

app.MapGet("/", () => "Hello, World");

app.MapGet("/genres", async (IGenresRepository repository) =>
{
    return Results.Ok(await repository.GetAllAsync());
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

app.MapGet("/genres/{id:int}", async (int id, IGenresRepository repository) =>
{
    var genre = await repository.GetByIdAsync(id);
    if (genre == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(genre);
});

app.MapPost("/genres", async (Genre genre, IGenresRepository repository, IOutputCacheStore outputCacheStore) =>
{
    var id = await repository.CreateAsync(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.Created($"/genres/{id}",genre);
});

#endregion Middlewares zone - END


app.Run();
