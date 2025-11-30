using Madar.Data;
using Madar.Models;
using Madar.ViewModels.AreaOwnerVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madar.Controllers.AreaOwner
{
    [Authorize(Roles = "AreaOwner")]
    [Route("AreaOwner/Profile")]
    public class ProfileController : Controller
    {
        private readonly MadarDbContext _context;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(MadarDbContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                //var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var userId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                var areaOwner = await _context.AreaOwners
                    .FirstOrDefaultAsync(ao => ao.UserId == userId);

                if (areaOwner == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var viewModel = new ProfileViewModel
                {
                    AreaOwner = new AreaOwnerProfileDto
                    {
                        AoId = areaOwner.AoId,
                        AoFname = areaOwner.AoFname ?? string.Empty,
                        AoLname = areaOwner.AoLname ?? string.Empty,
                        AoEmail = areaOwner.AoEmail ?? string.Empty,
                        AoExtension = areaOwner.AoExtension ?? string.Empty,
                        AoRole = areaOwner.AoRole ?? string.Empty,
                        CreatedAt = areaOwner.CreatedAt
                    }
                };

                ViewData["PageTitle"] = "My Profile";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile");
                TempData["ErrorMessage"] = "An error occurred while loading profile.";
                return RedirectToAction("Dashboard", "AreaOwner");
            }
        }

        [HttpPost("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                //var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var userId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                var areaOwner = await _context.AreaOwners
                    .FirstOrDefaultAsync(ao => ao.UserId == userId);

                if (areaOwner == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                // Check if email is already taken by another user
                var emailExists = await _context.AreaOwners
                    .AnyAsync(ao => ao.AoEmail == model.AoEmail && ao.AoId != areaOwner.AoId);

                if (emailExists)
                {
                    ModelState.AddModelError("AoEmail", "Email is already taken.");
                    TempData["ErrorMessage"] = "Email is already taken.";
                    return RedirectToAction(nameof(Index));
                }

                var userEmailExists = await _context.Users
                    .AnyAsync(u => u.Email == model.AoEmail && u.UserId != userId);

                if (userEmailExists)
                {
                    ModelState.AddModelError("AoEmail", "Email is already taken.");
                    TempData["ErrorMessage"] = "Email is already taken.";
                    return RedirectToAction(nameof(Index));
                }

                // Update Area Owner record
                areaOwner.AoFname = model.AoFname;
                areaOwner.AoLname = model.AoLname;
                areaOwner.AoEmail = model.AoEmail;
                areaOwner.AoExtension = model.AoExtension;
                areaOwner.UpdatedAt = DateTime.Now;

                // Update User record
                user.Name = $"{model.AoFname} {model.AoLname}";
                user.Email = model.AoEmail;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                TempData["ErrorMessage"] = $"Failed to update profile: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("UpdatePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                //var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var userId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Check current password
                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
                {
                    TempData["ErrorMessage"] = "Current password is incorrect.";
                    return RedirectToAction(nameof(Index));
                }

                var areaOwner = await _context.AreaOwners
                    .FirstOrDefaultAsync(ao => ao.UserId == userId);

                // Update password in User table
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.UpdatedAt = DateTime.Now;

                // Update password in AreaOwner table
                areaOwner.AoPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                areaOwner.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Password updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password");
                TempData["ErrorMessage"] = $"Failed to update password: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}