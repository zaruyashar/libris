namespace LIBRIS.DTOs
{
    public class BorrowRecordCreateDto
    {
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public required DateTime BorrowedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}
