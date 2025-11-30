using Madar.Data;
using Madar.Models;
using Madar.ViewModels.ManagementVMs.CorrectiveActionVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madar.Controllers.Management
{
    [Authorize]
    [Route("Management/CorrectiveActions")]
    public class CorrectiveActionController : Controller
    {
        private readonly MadarDbContext _context;
        private readonly ILogger<CorrectiveActionController> _logger;

        public CorrectiveActionController(MadarDbContext context, ILogger<CorrectiveActionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string status = null, string priority = "overdue")
        {
            try
            {
                var query = _context.CorrectiveActions
                               .Include(ca => ca.Audit)
                                   .ThenInclude(a => a.Schedule)
                                       .ThenInclude(s => s.Plant)
                               .Include(ca => ca.ResponsiblePerson)
                               .Include(ca => ca.Management)
                               .AsQueryable();

                if (!string.IsNullOrEmpty(status) && IsValidStatus(status))
                {
                    query = query.Where(ca => ca.ActionStatus == status);
                }

                if (!string.IsNullOrEmpty(priority))
                {
                    var today = DateOnly.FromDateTime(DateTime.Now);
                    if (priority == "overdue")
                    {
                        query = query.Where(ca => ca.ActionDeadlineDate < today && ca.ActionStatus != "Completed");
                    }
                    else if (priority == "critical")
                    {
                        var criticalDate = today.AddDays(3);
                        query = query.Where(ca => ca.ActionDeadlineDate <= criticalDate
                            && ca.ActionDeadlineDate >= today
                            && ca.ActionStatus != "Completed"
                            && ca.ActionStatus != "Rejected");
                    }
                }


                var correctiveActions = await query
                    .OrderBy(ca => ca.ActionDeadlineDate)
                    .ToListAsync();

                var responsiblePersons = await _context.ResponsiblePeople.ToListAsync();
                var audits = await _context.Audits.ToListAsync();
                var managements = await _context.Managements.ToListAsync();

                var totalActions = await _context.CorrectiveActions.CountAsync();
                var overdueActions = await _context.CorrectiveActions
                    .Where(ca => ca.ActionStatus != "Completed" && ca.ActionDeadlineDate < DateOnly.FromDateTime(DateTime.Now))
                    .CountAsync();

                var criticalActions = await _context.CorrectiveActions
                    .Where(ca => ca.ActionDeadlineDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(3))
                        && ca.ActionStatus != "Completed"
                        && ca.ActionStatus != "Rejected")
                    .CountAsync();

                var completedCount = await _context.CorrectiveActions
                    .Where(ca => ca.ActionStatus == "Completed")
                    .CountAsync();

                var completionRate = totalActions > 0
                    ? Math.Round((double)completedCount / totalActions * 100)
                    : 0;

                var viewModel = new CorrectiveActionIndexViewModel
                {
                    CorrectiveActions = correctiveActions,
                    ResponsiblePersons = responsiblePersons,
                    Audits = audits,
                    Managements = managements,
                    Status = status,
                    Priority = priority,
                    TotalActions = totalActions,
                    OverdueActions = overdueActions,
                    CriticalActions = criticalActions,
                    CompletionRate = completionRate
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading corrective actions");
                TempData["Error"] = "Failed to load corrective actions. Please try again.";
                return View(new CorrectiveActionIndexViewModel());
            }
        }

        [HttpPost("UpdateStatus/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(long id, UpdateCorrectiveActionStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid status value.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var action = await _context.CorrectiveActions.FindAsync(id);
                if (action == null)
                {
                    TempData["Error"] = "Corrective action not found.";
                    return RedirectToAction(nameof(Index));
                }

                action.ActionStatus = model.ActionStatus;
                action.UpdatedAt = DateTime.UtcNow;

                if (model.ActionStatus == "Completed")
                {
                    var currentUser = await GetCurrentManagementIdAsync();
                    if (currentUser.HasValue)
                    {
                        action.MgmtId = currentUser.Value;
                    }
                }

                _context.CorrectiveActions.Update(action);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Action marked as {model.ActionStatus}!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating corrective action status");
                TempData["Error"] = "Failed to update status. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("ApproveExtension/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveExtension(long id)
        {
            try
            {
                var action = await _context.CorrectiveActions.FindAsync(id);
                if (action == null)
                {
                    TempData["Error"] = "Corrective action not found.";
                    return RedirectToAction(nameof(Index));
                }

                action.ActionStatus = "Extended";
                action.UpdatedAt = DateTime.UtcNow;

                var currentUser = await GetCurrentManagementIdAsync();
                if (currentUser.HasValue)
                {
                    action.MgmtId = currentUser.Value;
                }

                _context.CorrectiveActions.Update(action);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Extension approved successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving extension");
                TempData["Error"] = "Failed to approve extension. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("RejectExtension/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectExtension(long id)
        {
            try
            {
                var action = await _context.CorrectiveActions.FindAsync(id);
                if (action == null)
                {
                    TempData["Error"] = "Corrective action not found.";
                    return RedirectToAction(nameof(Index));
                }

                action.ActionStatus = "Overdue";
                action.UpdatedAt = DateTime.UtcNow;

                var currentUser = await GetCurrentManagementIdAsync();
                if (currentUser.HasValue)
                {
                    action.MgmtId = currentUser.Value;
                }

                _context.CorrectiveActions.Update(action);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Extension rejected and marked as overdue!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting extension");
                TempData["Error"] = "Failed to reject extension. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Escalate/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Escalate(long id)
        {
            try
            {
                var action = await _context.CorrectiveActions.FindAsync(id);
                if (action == null)
                {
                    TempData["Error"] = "Corrective action not found.";
                    return RedirectToAction(nameof(Index));
                }

                action.UpdatedAt = DateTime.UtcNow;

                _context.CorrectiveActions.Update(action);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Action escalated to higher management!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error escalating action");
                TempData["Error"] = "Failed to escalate action. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(long id, UpdateCorrectiveActionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid form data. Please check all fields.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var action = await _context.CorrectiveActions.FindAsync(id);
                if (action == null)
                {
                    TempData["Error"] = "Corrective action not found.";
                    return RedirectToAction(nameof(Index));
                }

                var respExists = await _context.ResponsiblePeople.AnyAsync(rp => rp.RespId == model.RespId);
                if (!respExists)
                {
                    TempData["Error"] = "Selected responsible person does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                action.ActionDescription = model.ActionDescription;
                action.ActionDeadlineDate = model.ActionDeadlineDate;
                action.RespId = model.RespId;
                action.UpdatedAt = DateTime.UtcNow;

                _context.CorrectiveActions.Update(action);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Action updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating corrective action");
                TempData["Error"] = "Failed to update action. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool IsValidStatus(string status)
        {
            var validStatuses = new[] { "Open", "In_Progress", "Completed", "Overdue", "Extended", "Rejected" };
            return validStatuses.Contains(status);
        }

        private async Task<long?> GetCurrentManagementIdAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long userIdLong))
            {
                return null;
            }

            var management = await _context.Managements
                .FirstOrDefaultAsync(m => m.UserId == userIdLong);

            return management?.MgmtId;
        }
    }
}