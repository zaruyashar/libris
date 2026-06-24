namespace LIBRIS.DTOs
{
    public class AuthorCreateDto
    {
        public required string FullName { get; set; }

        public string? Nationality { get; set; }

        public int? BirthYear { get; set; }

        public bool? IsActive { get; set; }
    }
}
