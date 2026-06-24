namespace LIBRIS.DTOs
{
    public class BookReadDto
    {
        public int BookId { get; set; }

        public int AuthorId { get; set; }
        public string? AuthorName { get; set; } // Flattened data from the Author table

        public int GenreId { get; set; }
        public string? GenreName { get; set; }  // Flattened data from the Genre table

        public required string Title { get; set; }
        public required string Isbn { get; set; }
    }
}
