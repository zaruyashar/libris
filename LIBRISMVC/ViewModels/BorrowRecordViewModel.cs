namespace LIBRISMVC.ViewModels
{
    public class BorrowRecordViewModel
    {
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public int MemberId { get; set; }
        public string? MemberFullName { get; set; }
        public required DateTime BorrowedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}
