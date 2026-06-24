namespace LIBRIS.DTOs
{
    public class BookCreateDto
    {
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        public required string Title { get; set; }
        public required string Isbn { get; set; }
    }
}
