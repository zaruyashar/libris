namespace LIBRISMVC.ViewModels
{
    public class GenreViewModel
    {
        public int GenreId { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
