using LIBRISMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Json;

namespace LIBRISMVC.Controllers
{
    [Authorize]
    public class BorrowRecordController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public BorrowRecordController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("LibrisApi");
            var response = await client.GetAsync("BorrowRecord/GetBorrowRecords");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var records = JsonSerializer.Deserialize<IEnumerable<BorrowRecordViewModel>>(jsonString, _jsonOptions);
                return View(records);
            }

            return View(new List<BorrowRecordViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new BorrowRecordViewModel { BorrowedAt = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BorrowRecordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(viewModel);
            }

            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("BorrowRecord/AddBorrowRecord", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while communicating with the API.");
            await PopulateDropdowns();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("LibrisApi");
            var response = await client.GetAsync($"BorrowRecord/GetBorrowRecordsById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var record = JsonSerializer.Deserialize<BorrowRecordViewModel>(jsonString, _jsonOptions);
                await PopulateDropdowns();
                return View(record);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BorrowRecordViewModel viewModel)
        {
            if (id != viewModel.BorrowId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(viewModel);
            }

            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"BorrowRecord/UpdateBorrowRecord/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while updating the record.");
            await PopulateDropdowns();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("LibrisApi");
            var response = await client.DeleteAsync($"BorrowRecord/{id}");

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns()
        {
            var client = _httpClientFactory.CreateClient("LibrisApi");

            var booksResponse = await client.GetAsync("Book/GetBooks");
            if (booksResponse.IsSuccessStatusCode)
            {
                var jsonString = await booksResponse.Content.ReadAsStringAsync();
                var books = JsonSerializer.Deserialize<IEnumerable<BookViewModel>>(jsonString, _jsonOptions);
                ViewBag.Books = new SelectList(books, "BookId", "Title");
            }
            else
            {
                ViewBag.Books = new SelectList(new List<BookViewModel>(), "BookId", "Title");
            }

            var membersResponse = await client.GetAsync("Member/GetMembers");
            if (membersResponse.IsSuccessStatusCode)
            {
                var jsonString = await membersResponse.Content.ReadAsStringAsync();
                var members = JsonSerializer.Deserialize<IEnumerable<MemberViewModel>>(jsonString, _jsonOptions);
                ViewBag.Members = new SelectList(members, "MemberId", "FullName");
            }
            else
            {
                ViewBag.Members = new SelectList(new List<MemberViewModel>(), "MemberId", "FullName");
            }
        }
    }
}