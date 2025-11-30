using Madar.Data;
using Madar.Models;
using Madar.ViewModels.AuthVMs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Madar.Controllers
{
    public class AuthController : Controller
    {
        private readonly MadarDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(MadarDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Show the login form (GET) - Equivalent to showLoginForm() in PHP
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // If already authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }

            return View(new LoginViewModel());
        }

        /// <summary>
        /// Handle login request (POST) - Equivalent to login() in PHP
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Find user by email - Equivalent to Auth::attempt in Laravel
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty,
                        "The provided credentials do not match our records.");
                    return View(model);
                }

                // Verify password using BCrypt - Equivalent to Hash::check in Laravel
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);

                if (!isPasswordValid)
                {
                    ModelState.AddModelError(string.Empty,
                        "The provided credentials do not match our records.");
                    return View(model);
                }

                // Create claims for the user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.UserType),
                    new Claim("UserType", user.UserType)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                HttpContext.Session.Clear();

                _logger.LogInformation("User {Email} logged in successfully with UserType: {UserType}",
                    user.Email, user.UserType);

                return RedirectToDashboard();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt for {Email}", model.Email);
                ModelState.AddModelError(string.Empty,
                    "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();

                _logger.LogInformation("User {Email} logged out successfully", userEmail);

                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return RedirectToAction(nameof(Login));
            }
        }

        private IActionResult RedirectToDashboard()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction(nameof(Login));
            }

            var userType = User.FindFirst("UserType")?.Value;

            return userType switch
            {
                "Management" => RedirectToAction("Dashboard", "Management"),
                "Auditor" => RedirectToAction("Dashboard", "Auditor"),
                "AreaOwner" => RedirectToAction("Dashboard", "AreaOwner"),
                "ResponsiblePerson" => RedirectToAction("Dashboard", "ResponsiblePerson"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}