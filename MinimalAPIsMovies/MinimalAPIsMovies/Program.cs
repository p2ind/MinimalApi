using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies;
using MinimalAPIsMovies.Endpoints;
using MinimalAPIsMovies.Repositories;
using MinimalAPIsMovies.Services;

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
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();

//builder.Services.AddTransient<IFileStorage, AzureFileStorage>();

builder.Services.AddTransient<IFileStorage, LocalFileStorage>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

#endregion services zone - end

var app = builder.Build();

#region Middlewares zone - BEGION

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();


app.UseCors();

app.UseOutputCache();

app.MapGet("/", () => "Hello, World");

app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movies/{movieId:int}/comments").MapComments();

#endregion Middlewares zone - END


app.Run();