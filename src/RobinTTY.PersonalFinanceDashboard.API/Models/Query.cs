namespace RobinTTY.PersonalFinanceDashboard.API.Models;

public record Book(string Title, Author Author);
public record Author(string Name);

/// <summary>
/// GraphQL root type for query operations.
/// </summary>
public class Query
{
    private readonly List<Book> _books = new()
    {
        new Book("C# in Depth: Fourth Edition", new Author("Jon Skeet")),
        new Book("Learning GraphQL: Declarative Data Fetching for Modern Web Apps", new Author("Eve Porcello and Alex Banks")),
        new Book("Code: The Hidden Language of Computer Hardware and Software", new Author("Charles Petzold"))
    };

    public List<Book> GetBooks() => _books;
    public Book? GetBook(string title) => _books.FirstOrDefault(x => x.Title == title);

    public Author? GetAuthor(string name) => _books.FirstOrDefault(x => x.Author.Name == name)?.Author;
}
