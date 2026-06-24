namespace LIBRIS.DTOs
{
    public class AuthorReadDto
    {
        public int AuthorId { get; set; }

        public required string FullName { get; set; }

        public string? Nationality { get; set; }

        public int? BirthYear { get; set; }

        public bool IsActive { get; set; }
    }
}
