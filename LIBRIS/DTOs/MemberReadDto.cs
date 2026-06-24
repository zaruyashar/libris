namespace LIBRIS.DTOs
{
    public class MemberReadDto
    {
        public int MemberId { get; set; }

        public required string FullName { get; set; }

        public required string Email { get; set; }

        public DateTime MemberSince { get; set; }

        public required bool IsActive { get; set; }
    }
}
