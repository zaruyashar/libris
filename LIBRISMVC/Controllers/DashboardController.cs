using LIBRISMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LIBRISMVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("LibrisApi");
            var dashboardData = new DashboardViewModel();

            var booksResponse = await client.GetAsync("Book/GetBooks");
            var membersResponse = await client.GetAsync("Member/GetMembers");
            var borrowsResponse = await client.GetAsync("BorrowRecord/GetBorrowRecords");

            IEnumerable<BookViewModel> books = new List<BookViewModel>();
            if (booksResponse.IsSuccessStatusCode)
            {
                var booksJson = await booksResponse.Content.ReadAsStringAsync();
                books = JsonSerializer.Deserialize<IEnumerable<BookViewModel>>(booksJson, _jsonOptions) ?? new List<BookViewModel>();
            }

            IEnumerable<MemberViewModel> members = new List<MemberViewModel>();
            if (membersResponse.IsSuccessStatusCode)
            {
                var membersJson = await membersResponse.Content.ReadAsStringAsync();
                members = JsonSerializer.Deserialize<IEnumerable<MemberViewModel>>(membersJson, _jsonOptions) ?? new List<MemberViewModel>();
            }

            IEnumerable<BorrowRecordViewModel> borrows = new List<BorrowRecordViewModel>();
            if (borrowsResponse.IsSuccessStatusCode)
            {
                var borrowsJson = await borrowsResponse.Content.ReadAsStringAsync();
                borrows = JsonSerializer.Deserialize<IEnumerable<BorrowRecordViewModel>>(borrowsJson, _jsonOptions) ?? new List<BorrowRecordViewModel>();
            }

            dashboardData.TotalCatalog = books.Count();
            dashboardData.ActiveMembers = members.Count(m => m.IsActive);

            var activeBorrows = borrows.Where(b => b.ReturnedAt == null).ToList();
            dashboardData.CurrentlyBorrowed = activeBorrows.Count;

            dashboardData.OverdueReturns = activeBorrows.Count(b => b.BorrowedAt.AddDays(14) < DateTime.Now);

            var genreGroups = books.GroupBy(b => b.GenreName)
                                   .Where(g => !string.IsNullOrEmpty(g.Key))
                                   .OrderByDescending(g => g.Count())
                                   .Take(6)
                                   .ToList();

            dashboardData.GenreLabels = genreGroups.Select(g => g.Key).ToList()!;
            dashboardData.GenreData = genreGroups.Select(g => g.Count()).ToList();

            dashboardData.BorrowedBooks = dashboardData.CurrentlyBorrowed;
            dashboardData.AvailableBooks = dashboardData.TotalCatalog - dashboardData.BorrowedBooks;

            return View(dashboardData);
        }
    }
}