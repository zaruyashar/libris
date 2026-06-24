namespace LIBRIS.DTOs
{
    public class BorrowRecordReadDto
    {
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public int MemberId { get; set; }
        public string? MemberFullName { get; set; }
        public required DateTime BorrowedAt { get; set; }
        public DateTime? ReturnedAt { get; set; } // When testing using the Swagger UI, either use ' null ' without quotes, or delete the ReturnedAt line entirely to avoid Json payload issues.
    }
}
