namespace LIBRISMVC.ViewModels
{
    public class AuthorViewModel
    {
        public int AuthorId { get; set; }

        public required string FullName { get; set; }

        public string? Nationality { get; set; }

        public int? BirthYear { get; set; }

        public bool IsActive { get; set; }
    }
}
