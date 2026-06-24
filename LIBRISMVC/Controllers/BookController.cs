using LIBRISMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Json;

namespace LIBRISMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public BookController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("LibrisApi");
            var response = await client.GetAsync("Book/GetBooks");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var books = JsonSerializer.Deserialize<IEnumerable<BookViewModel>>(jsonString, _jsonOptions);
                return View(books);
            }

            return View(new List<BookViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new BookViewModel { Title = string.Empty, Isbn = string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(viewModel);
            }

            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Book/AddBook", jsonContent);

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
            var response = await client.GetAsync($"Book/GetBooksById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var book = JsonSerializer.Deserialize<BookViewModel>(jsonString, _jsonOptions);
                await PopulateDropdowns();
                return View(book);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookViewModel viewModel)
        {
            if (id != viewModel.BookId)
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

            var response = await client.PutAsync($"Book/UpdateBook/{id}", jsonContent);

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
            var response = await client.DeleteAsync($"Book/{id}");

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns()
        {
            var client = _httpClientFactory.CreateClient("LibrisApi");

            var authorsResponse = await client.GetAsync("Author/GetAuthors");
            if (authorsResponse.IsSuccessStatusCode)
            {
                var jsonString = await authorsResponse.Content.ReadAsStringAsync();
                var authors = JsonSerializer.Deserialize<IEnumerable<AuthorViewModel>>(jsonString, _jsonOptions);
                ViewBag.Authors = new SelectList(authors, "AuthorId", "FullName");
            }
            else
            {
                ViewBag.Authors = new SelectList(new List<AuthorViewModel>(), "AuthorId", "FullName");
            }

            var genresResponse = await client.GetAsync("Genre/GetGenres");
            if (genresResponse.IsSuccessStatusCode)
            {
                var jsonString = await genresResponse.Content.ReadAsStringAsync();
                var genres = JsonSerializer.Deserialize<IEnumerable<GenreViewModel>>(jsonString, _jsonOptions);
                ViewBag.Genres = new SelectList(genres, "GenreId", "Name");
            }
            else
            {
                ViewBag.Genres = new SelectList(new List<GenreViewModel>(), "GenreId", "Name");
            }
        }
    }
}