using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using IotControlService.DTO;
using IotControlService.Models;

namespace IotControlService.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountApiController(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(new { Message = "User registered successfully" });
                }
                return BadRequest(new { Message = "Failed to create user" });
            }
            return BadRequest(new { Message = "Invalid data" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccountDTO model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "Login successful" });
                }
                return Unauthorized(new { Message = "Invalid login attempt" });
            }
            return BadRequest(new { Message = "Invalid data" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Logout successful" });
        }
    }
}
