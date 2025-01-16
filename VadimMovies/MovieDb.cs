using Microsoft.EntityFrameworkCore;

namespace VadimMovies;

public class MovieDb(DbContextOptions<MovieDb> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
}