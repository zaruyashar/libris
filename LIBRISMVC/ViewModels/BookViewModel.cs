namespace LIBRISMVC.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }

        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public int GenreId { get; set; }
        public string? GenreName { get; set; }
        public required string Title { get; set; }
        public required string Isbn { get; set; }
    }
}
