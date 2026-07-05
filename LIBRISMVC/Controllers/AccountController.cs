using LIBRISMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace LIBRISMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(new
            {
                model.Email,
                model.Password
            }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);

                if (authResult != null && authResult.IsSuccess)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, authResult.UserId ?? string.Empty),
                        new Claim(ClaimTypes.Name, authResult.FullName ?? string.Empty),
                        new Claim(ClaimTypes.Email, authResult.Email ?? string.Empty)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                    // Determine the target redirect URL
                    string redirectUrl = Url.Action("Index", "Dashboard") ?? "/";
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        redirectUrl = model.ReturnUrl;
                    }

                    // Render the intermediate view that uses window.open to launch the dashboard in a new tab
                    return View("LoginSuccess", new LoginSuccessViewModel { RedirectUrl = redirectUrl });
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var authResult = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);
                    ModelState.AddModelError(string.Empty, authResult?.Message ?? "Invalid email or password.");
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred during sign-in. Please try again later.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(new
            {
                model.FullName,
                model.Email,
                model.Password
            }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction(nameof(Login));
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                var authResult = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);
                ModelState.AddModelError(string.Empty, authResult?.Message ?? "Registration failed.");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while communicating with the API.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View(new VerifyEmailViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(new { model.Email }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/forgot-password", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ForgotPasswordResponse>(responseContent, _jsonOptions);
                if (result != null)
                {
                    model.Token = result.Token;
                    model.RequestSent = true;
                    model.Message = result.Message;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email not found or API communication error.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword(string email, string token)
        {
            return View(new ChangePasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(new
            {
                model.Email,
                model.Token,
                model.NewPassword
            }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/reset-password", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully. Please log in.";
                return RedirectToAction(nameof(Login));
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                var result = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);
                ModelState.AddModelError(string.Empty, result?.Message ?? "Failed to reset password.");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred.");
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ManageChangePassword()
        {
            return View(new ManageChangePasswordViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageChangePassword(ManageChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(Login));
            }

            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(new
            {
                Email = email,
                model.CurrentPassword,
                model.NewPassword
            }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/change-password", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Index", "Dashboard");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                var result = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);
                ModelState.AddModelError(string.Empty, result?.Message ?? "Failed to change password.");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred.");
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AccountDetails()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var name = User.Identity?.Name ?? string.Empty;

            var model = new AccountDetailsViewModel
            {
                CurrentEmail = email,
                NewEmail = email,
                NewFullName = name
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountDetails(AccountDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = _httpClientFactory.CreateClient("LibrisApi");
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/update-profile", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // Update claims dynamically in the session
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, currentUserId ?? string.Empty),
                    new Claim(ClaimTypes.Name, model.NewFullName),
                    new Claim(ClaimTypes.Email, model.NewEmail)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(AccountDetails));
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                var result = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);
                ModelState.AddModelError(string.Empty, result?.Message ?? "Failed to update profile.");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating profile.");
            }

            return View(model);
        }

        // Helper classes for deserialization
        private class ForgotPasswordResponse
        {
            public string Token { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }

        private class AuthResponseDto
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? FullName { get; set; }
            public string? UserId { get; set; }
        }
    }
}
