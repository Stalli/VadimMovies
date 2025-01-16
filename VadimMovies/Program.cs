using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.EntityFrameworkCore;
using VadimMovies;

var builder = WebApplication.CreateBuilder(args);
var corsPolicyName = "MoviesOriginPolicy";
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MovieDb>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Azure")));
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
                      policy  =>
                      {
                          policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                      });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);

app.MapGet("/", SearchMovie);

app.Run();
return;

async Task<List<MovieDto>> SearchMovie(MovieDb db, string movieTitle)
{
    var result = new List<MovieDto>();
    var movies = await db.Movies.Where(m => m.Title == movieTitle).ToListAsync();
    
    var share = new ShareClient(builder.Configuration.GetConnectionString("AzureFileShare"), "vadimmoviewfileshare");
    var directory = share.GetDirectoryClient("");
    foreach (var movie in movies)
    {
        var file = directory.GetFileClient(movie.ImagePath);

        ShareFileDownloadInfo imageFile = await file.DownloadAsync();
        using var ms = new MemoryStream();
        await imageFile.Content.CopyToAsync(ms);
        var imageBytes = ms.ToArray();
        result.Add(new MovieDto
        {
            Id = movie.Id,
            Title = movieTitle,
            Year = movie.Year,
            Image = imageBytes
        });
    }
    
    return result;
}