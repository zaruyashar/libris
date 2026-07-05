using LIBRIS.DTOs;
using LIBRIS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LIBRIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid input data."
                });
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email already registered."
                });
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "User registered successfully.",
                    Email = user.Email,
                    FullName = user.FullName,
                    UserId = user.Id
                });
            }

            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return BadRequest(new AuthResponseDto
            {
                IsSuccess = false,
                Message = $"Registration failed: {errors}"
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid credentials format."
                });
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid email or password."
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Login successful.",
                    Email = user.Email,
                    FullName = user.FullName,
                    UserId = user.Id
                });
            }

            if (result.IsLockedOut)
            {
                return StatusCode(StatusCodes.Status423Locked, new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Account locked out. Please try again in 5 minutes."
                });
            }

            return Unauthorized(new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Invalid email or password."
            });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<object>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Email is required." });
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                // To prevent user enumeration, we could return success, but in a dev/homework project,
                // and to return the reset token directly, we must handle it.
                return NotFound(new { Message = "User not found with this email." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(new
            {
                Token = token,
                Email = user.Email,
                Message = "Reset token generated successfully. Copy it to proceed."
            });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<AuthResponseDto>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "Invalid input details." });
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new AuthResponseDto { IsSuccess = false, Message = "User not found." });
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto { IsSuccess = true, Message = "Password reset successfully." });
            }

            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return BadRequest(new AuthResponseDto { IsSuccess = false, Message = $"Failed to reset password: {errors}" });
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<AuthResponseDto>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "Invalid input details." });
            }

            var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);
            if (user == null)
            {
                return NotFound(new AuthResponseDto { IsSuccess = false, Message = "User not found." });
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto { IsSuccess = true, Message = "Password changed successfully." });
            }

            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return BadRequest(new AuthResponseDto { IsSuccess = false, Message = $"Failed to change password: {errors}" });
        }

        [HttpPost("update-profile")]
        public async Task<ActionResult<AuthResponseDto>> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "Invalid profile data." });
            }

            var user = await _userManager.FindByEmailAsync(updateProfileDto.CurrentEmail);
            if (user == null)
            {
                return NotFound(new AuthResponseDto { IsSuccess = false, Message = "User not found." });
            }

            user.FullName = updateProfileDto.NewFullName;

            if (!string.Equals(updateProfileDto.CurrentEmail, updateProfileDto.NewEmail, StringComparison.OrdinalIgnoreCase))
            {
                var existingUserWithNewEmail = await _userManager.FindByEmailAsync(updateProfileDto.NewEmail);
                if (existingUserWithNewEmail != null)
                {
                    return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "Email address is already in use." });
                }

                var setUserNameResult = await _userManager.SetUserNameAsync(user, updateProfileDto.NewEmail);
                if (!setUserNameResult.Succeeded)
                {
                    var userNameErrors = string.Join(" ", setUserNameResult.Errors.Select(e => e.Description));
                    return BadRequest(new AuthResponseDto { IsSuccess = false, Message = $"Failed to set username: {userNameErrors}" });
                }

                var setEmailResult = await _userManager.SetEmailAsync(user, updateProfileDto.NewEmail);
                if (!setEmailResult.Succeeded)
                {
                    var emailErrors = string.Join(" ", setEmailResult.Errors.Select(e => e.Description));
                    return BadRequest(new AuthResponseDto { IsSuccess = false, Message = $"Failed to set email: {emailErrors}" });
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Profile updated successfully.",
                    Email = user.Email,
                    FullName = user.FullName,
                    UserId = user.Id
                });
            }

            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return BadRequest(new AuthResponseDto { IsSuccess = false, Message = $"Failed to update profile: {errors}" });
        }
    }
}
