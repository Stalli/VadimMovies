namespace VadimMovies;

public class MovieDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int Year { get; set; }
    public byte[]? Image { get; set; }
}