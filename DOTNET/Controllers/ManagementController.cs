using Madar.Data;
using Madar.Models;
using Madar.ViewModels.ManagementVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Madar.Controllers.Management
{
    [Authorize]
    [Route("Management")]
    public class ManagementController : Controller
    {
        private readonly MadarDbContext _context;
        private readonly ILogger<ManagementController> _logger;

        public ManagementController(MadarDbContext context, ILogger<ManagementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var totalPlants = await _context.Plants.CountAsync();
                var pendingAudits = await _context.Audits.CountAsync(a => a.AuditStatus == "Draft");
                var overdueActions = await _context.CorrectiveActions.CountAsync(ca => ca.ActionStatus == "Overdue");

                var totalAudits = await _context.Audits.CountAsync();
                var completedAudits = await _context.Audits.CountAsync(a => a.AuditStatus == "Closed");
                var completionRate = totalAudits > 0 ? Math.Round((double)completedAudits / totalAudits * 100) : 0;

                var totalActions = await _context.CorrectiveActions.CountAsync();
                var completedActions = await _context.CorrectiveActions.CountAsync(ca => ca.ActionStatus == "Completed");
                var actionCompletionRate = totalActions > 0 ? Math.Round((double)completedActions / totalActions * 100) : 0;

                var startOfWeek = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7);
                var actionsDueThisWeek = await _context.CorrectiveActions
                    .Where(ca => ca.ActionDeadlineDate >= DateOnly.FromDateTime(startOfWeek)
                        && ca.ActionDeadlineDate <= DateOnly.FromDateTime(endOfWeek)
                        && ca.ActionStatus != "Completed")
                    .CountAsync();

                var criticalDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
                var criticalIssues = await _context.CorrectiveActions
                    .Where(ca => ca.ActionDeadlineDate <= criticalDate
                        && ca.ActionStatus != "Completed")
                    .CountAsync();

                var sevenDaysAgo = DateTime.Now.AddDays(-7);
                var recentUsers = await _context.Users
                    .Where(u => u.CreatedAt >= sevenDaysAgo)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                var today = DateOnly.FromDateTime(DateTime.Now);
                var nextWeek = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
                var upcomingAudits = await _context.AuditSchedules
                    .Where(s => s.AudSchDate >= today && s.AudSchDate <= nextWeek && s.AudSchStatus == "Scheduled")
                    .OrderBy(s => s.AudSchDate)
                    .Take(5)
                    .ToListAsync();

                var viewModel = new DashboardViewModel
                {
                    TotalUsers = totalUsers,
                    TotalPlants = totalPlants,
                    PendingAudits = pendingAudits,
                    OverdueActions = overdueActions,
                    CompletionRate = completionRate,
                    ActionCompletionRate = actionCompletionRate,
                    ActionsDueThisWeek = actionsDueThisWeek,
                    CriticalIssues = criticalIssues,
                    RecentUsers = recentUsers,
                    UpcomingAudits = upcomingAudits,
                    TotalAudits = totalAudits,
                    CompletedAudits = completedAudits,
                    TotalActions = totalActions,
                    CompletedActions = completedActions
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["Error"] = "Failed to load dashboard. Please try again.";
                return View(new DashboardViewModel());
            }
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Dashboard");
                }

                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Dashboard");
                }

                var viewModel = new ProfileViewModel
                {
                    UserId = user.UserId,
                    Name = user.Name ?? "",
                    Email = user.Email ?? "",
                    UserType = user.UserType ?? ""
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile");
                TempData["Error"] = "Failed to load profile. Please try again.";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost("Profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid form data. Please check all fields.";
                return RedirectToAction("Profile");
            }

            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Profile");
                }

                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Profile");
                }

                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == model.Email && u.UserId != userId.Value);

                if (emailExists)
                {
                    TempData["Error"] = "Email is already taken.";
                    return RedirectToAction("Profile");
                }

                user.Name = model.Name;
                user.Email = model.Email;
                user.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    if (string.IsNullOrEmpty(model.CurrentPassword))
                    {
                        TempData["Error"] = "Current password is required to set a new password.";
                        return RedirectToAction("Profile");
                    }
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                TempData["Error"] = "Failed to update profile. Please try again.";
                return RedirectToAction("Profile");
            }
        }

        [HttpGet("Notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var notifications = new List<NotificationViewModel>();

                var overdueActions = await _context.CorrectiveActions.CountAsync(ca => ca.ActionStatus == "Overdue");
                if (overdueActions > 0)
                {
                    notifications.Add(new NotificationViewModel
                    {
                        Type = "danger",
                        Icon = "exclamation-triangle",
                        Message = $"{overdueActions} overdue corrective actions",
                        Link = "/Management/CorrectiveActions?priority=overdue"
                    });
                }

                var today = DateOnly.FromDateTime(DateTime.Now);
                var todayAudits = await _context.AuditSchedules
                    .CountAsync(s => s.AudSchDate == today && s.AudSchStatus == "Scheduled");
                if (todayAudits > 0)
                {
                    notifications.Add(new NotificationViewModel
                    {
                        Type = "info",
                        Icon = "calendar-day",
                        Message = $"{todayAudits} audits scheduled for today",
                        Link = "/Management/AuditSchedules?view=list"
                    });
                }

                var criticalDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
                var criticalActions = await _context.CorrectiveActions
                    .CountAsync(ca => ca.ActionDeadlineDate <= criticalDate && ca.ActionStatus != "Completed");
                if (criticalActions > 0)
                {
                    notifications.Add(new NotificationViewModel
                    {
                        Type = "warning",
                        Icon = "clock",
                        Message = $"{criticalActions} actions due in 3 days",
                        Link = "/Management/CorrectiveActions?priority=critical"
                    });
                }

                return Json(new { success = true, notifications });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications");
                return Json(new { success = false, message = "Failed to load notifications" });
            }
        }

        private long? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                return null;
            }
            return userId;
        }
    }
}