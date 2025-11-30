using Madar.Data;
using Madar.Models;
using Madar.ViewModels.ManagementVMs.AuditScheduleVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madar.Controllers.Management
{
    [Authorize]
    [Route("Management/AuditSchedules")]
    public class AuditScheduleController : Controller
    {
        private readonly MadarDbContext _context;
        private readonly ILogger<AuditScheduleController> _logger;

        public AuditScheduleController(MadarDbContext context, ILogger<AuditScheduleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string view = "list")
        {
            try
            {
                var schedules = await _context.AuditSchedules
                    .OrderByDescending(s => s.AudSchDate)
                    .ToListAsync();

                var plants = await _context.Plants
                    .Where(p => p.PlantStatus == "Active")
                    .ToListAsync();

                var auditors = await _context.Auditors.ToListAsync();

                var totalSchedules = schedules.Count;
                var completedSchedules = schedules.Count(s => s.AudSchStatus == "Completed");
                var completionRate = totalSchedules > 0
                    ? Math.Round((double)completedSchedules / totalSchedules * 100)
                    : 0;

                var maps = schedules.Select(schedule => new AuditScheduleMapViewModel
                {
                    Id = schedule.AudSchId,
                    Title = _context.Plants.Find(schedule.PlantId)?.PlantName ?? "Unknown Plant",
                    Date = schedule.AudSchDate,
                    Status = schedule.AudSchStatus ?? "Scheduled",
                    Duration = schedule.AudSchDuration ?? 0,
                    Location = _context.Plants.Find(schedule.PlantId)?.PlantLocation ?? "",
                    Auditors = _context.AuditorAllocations
                        .Where(aa => aa.AudSchId == schedule.AudSchId)
                        .Join(_context.Auditors, aa => aa.AudId, a => a.AudId, (aa, a) => $"{a.AudFname} {a.AudLname}")
                        .ToList()
                }).ToList();

                var viewModel = new AuditScheduleIndexViewModel
                {
                    Schedules = schedules,
                    Plants = plants,
                    Auditors = auditors,
                    View = view,
                    CompletionRate = completionRate,
                    TotalSchedules = totalSchedules,
                    CompletedSchedules = completedSchedules,
                    Maps = maps
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading audit schedules");
                TempData["Error"] = "Failed to load audit schedules. Please try again.";
                return View(new AuditScheduleIndexViewModel());
            }
        }

        [HttpPost("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(CreateAuditScheduleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid form data. Please check all fields.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var plantExists = await _context.Plants.AnyAsync(p => p.PlantId == model.PlantId);
                if (!plantExists)
                {
                    TempData["Error"] = "Selected plant does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                var schedule = new AuditSchedule
                {
                    PlantId = model.PlantId,
                    AudSchDate = model.AudSchDate,
                    AudSchDuration = model.AudSchDuration,
                    AudSchStatus = "Scheduled",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.AuditSchedules.Add(schedule);
                await _context.SaveChangesAsync();

                foreach (var auditorId in model.Auditors)
                {
                    var allocation = new AuditorAllocation
                    {
                        AudId = auditorId,
                        AudSchId = schedule.AudSchId,
                        AssignedDate = DateOnly.FromDateTime(DateTime.Now),
                        RoleType = "Auditor",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.AuditorAllocations.Add(allocation);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Audit schedule created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit schedule");
                TempData["Error"] = "Failed to create audit schedule. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("UpdateStatus/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(long id, UpdateAuditScheduleStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid status value.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var schedule = await _context.AuditSchedules.FindAsync(id);
                if (schedule == null)
                {
                    TempData["Error"] = "Audit schedule not found.";
                    return RedirectToAction(nameof(Index));
                }

                schedule.AudSchStatus = model.AudSchStatus;
                schedule.UpdatedAt = DateTime.UtcNow;

                _context.AuditSchedules.Update(schedule);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Audit schedule marked as {model.AudSchStatus}!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating audit schedule status");
                TempData["Error"] = "Failed to update status. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var schedule = await _context.AuditSchedules.FindAsync(id);
                if (schedule == null)
                {
                    TempData["Error"] = "Audit schedule not found.";
                    return RedirectToAction(nameof(Index));
                }

                var allocations = await _context.AuditorAllocations
                    .Where(aa => aa.AudSchId == id)
                    .ToListAsync();
                _context.AuditorAllocations.RemoveRange(allocations);

                _context.AuditSchedules.Remove(schedule);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Audit schedule deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting audit schedule");
                TempData["Error"] = "Failed to delete audit schedule. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            try
            {
                var schedule = await _context.AuditSchedules.FindAsync(id);
                if (schedule == null)
                {
                    return NotFound(new { success = false, message = "Schedule not found" });
                }

                var plant = await _context.Plants.FindAsync(schedule.PlantId);
                var allocations = await _context.AuditorAllocations
                    .Where(aa => aa.AudSchId == id)
                    .ToListAsync();

                var auditorDetails = new List<object>();
                foreach (var alloc in allocations)
                {
                    var auditor = await _context.Auditors.FindAsync(alloc.AudId);
                    if (auditor != null)
                    {
                        auditorDetails.Add(new
                        {
                            id = auditor.AudId,
                            name = $"{auditor.AudFname} {auditor.AudLname}".Trim()
                        });
                    }
                }

                var result = new
                {
                    success = true,
                    data = new
                    {
                        id = schedule.AudSchId,
                        plantId = schedule.PlantId,
                        plantName = plant?.PlantName,
                        date = schedule.AudSchDate.ToString("yyyy-MM-dd"),
                        duration = schedule.AudSchDuration,
                        status = schedule.AudSchStatus,
                        auditors = auditorDetails
                    }
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching schedule details");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}