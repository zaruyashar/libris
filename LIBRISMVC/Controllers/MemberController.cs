using LIBRISMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace LIBRISMVC.Controllers
{
    public class MemberController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public MemberController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("LibrisApi");
            var response = await client.GetAsync("Member/GetMembers");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var members = JsonSerializer.Deserialize<IEnumerable<MemberViewModel>>(jsonString, _jsonOptions);
                return View(members);
            }

            return View(new List<MemberViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new MemberViewModel { FullName = string.Empty, Email = string.Empty, MemberSince = DateTime.Now, IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Member/AddMember", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while communicating with the API.");
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("LibrisApi");
            var response = await client.GetAsync($"Member/GetMembersById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var member = JsonSerializer.Deserialize<MemberViewModel>(jsonString, _jsonOptions);
                return View(member);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MemberViewModel viewModel)
        {
            if (id != viewModel.MemberId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"Member/UpdateMember/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while updating the record.");
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("LibrisApi");
            var response = await client.DeleteAsync($"Member/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}