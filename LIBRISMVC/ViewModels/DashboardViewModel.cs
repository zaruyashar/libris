namespace LIBRISMVC.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCatalog { get; set; }
        public int ActiveMembers { get; set; }
        public int CurrentlyBorrowed { get; set; }
        public int OverdueReturns { get; set; }

        public List<string> GenreLabels { get; set; } = new();
        public List<int> GenreData { get; set; } = new();

        public int AvailableBooks { get; set; }
        public int BorrowedBooks { get; set; }
    }
}