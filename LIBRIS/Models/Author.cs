using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRIS.Models
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthorId { get; set; }

        public required string FullName { get; set; }

        public string? Nationality { get; set; }

        public int? BirthYear { get; set; }

        public bool? IsActive { get; set; }

        // Collection Navigation Properties
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
