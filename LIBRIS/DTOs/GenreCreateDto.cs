namespace LIBRIS.DTOs
{
    public class GenreCreateDto
    {
        public required string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
