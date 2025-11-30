using Madar.Common;
using Madar.Data;
using Madar.Models;
using Madar.ViewModels.AuditorVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Madar.Controllers.Auditor
{
    [Authorize]
    [Route("Auditor")]
    public class AuditorController : Controller
    {
        private readonly MadarDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AuditorController> _logger;

        public AuditorController(
            MadarDbContext context,
            IWebHostEnvironment environment,
            ILogger<AuditorController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        private long GetCurrentAuditorId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims");
            }

            // Find auditor by USER_ID (this is the correct way)
            var auditor = _context.Auditors
                .FirstOrDefault(a => a.UserId == long.Parse(userId));

            if (auditor == null)
            {
                throw new UnauthorizedAccessException("Auditor not found for current user");
            }

            return auditor.AudId;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var auditorId = GetCurrentAuditorId();

                var auditor = await _context.Auditors
                    .FirstOrDefaultAsync(a => a.AudId == auditorId);

                if (auditor == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var today = DateTime.Today;

                // Get assigned schedules
                var assignedSchedules = await _context.AuditorAllocations
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Plant)
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Audits)
                    .Where(aa => aa.AudId == auditorId && aa.Schedule.AudSchDate >= DateOnly.FromDateTime(today))
                    .OrderByDescending(aa => aa.AssignedDate)
                    .ToListAsync();

                // Get today's audits
                var todayAudits = await _context.AuditorAllocations
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Plant)
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Audits)
                    .Where(aa => aa.AudId == auditorId
                        && aa.Schedule.AudSchDate == DateOnly.FromDateTime(today)
                        && aa.Schedule.AudSchStatus == Constants.AUDIT_SCHEDULE_SCHEDULED)
                    .ToListAsync();

                // Get recent audits
                var recentAudits = await _context.Audits
                    .Include(a => a.Schedule)
                    .Where(a => a.Schedule.Allocations.Any(aa => aa.AudId == auditorId)
                        && a.AuditStatus == Constants.AUDIT_STATUS_CLOSED)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                // Get pending audits
                var pendingAudits = await _context.Audits
                    .Where(a => a.Schedule.Allocations.Any(aa => aa.AudId == auditorId)
                        && (a.AuditStatus == Constants.AUDIT_STATUS_DRAFT
                            || a.AuditStatus == Constants.AUDIT_STATUS_IN_PROGRESS))
                    .CountAsync();

                var viewModel = new AuditorDashboardViewModel
                {
                    Auditor = auditor,
                    AssignedSchedules = assignedSchedules,
                    TodayAudits = todayAudits,
                    RecentAudits = recentAudits,
                    TotalAssigned = assignedSchedules.Count,
                    TodayCount = todayAudits.Count,
                    PendingAudits = pendingAudits
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading auditor dashboard");
                TempData["Error"] = "Failed to load dashboard";
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpGet("AssignedSchedules")]
        public async Task<IActionResult> AssignedSchedules()
        {
            try
            {
                var auditorId = GetCurrentAuditorId();
                var sevenDaysAgo = DateTime.Today.AddDays(-7);

                var allocations = await _context.AuditorAllocations
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Plant)
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Audits)
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Allocations)
                            .ThenInclude(a => a.Auditor)
                    .Where(aa => aa.AudId == auditorId
                        && aa.Schedule.AudSchDate >= DateOnly.FromDateTime(sevenDaysAgo))
                    .OrderByDescending(aa => aa.AssignedDate)
                    .ToListAsync();

                return View(allocations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assigned schedules");
                TempData["Error"] = "Failed to load assigned schedules";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet("Schedule/{scheduleId}")]
        public async Task<IActionResult> ViewSchedule(long scheduleId)
        {
            try
            {
                var auditorId = GetCurrentAuditorId();

                var allocation = await _context.AuditorAllocations
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Plant)
                            .ThenInclude(p => p.Equipment)
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Audits)
                            .ThenInclude(a => a.Evidences)
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Allocations)
                            .ThenInclude(a => a.Auditor)
                    .FirstOrDefaultAsync(aa => aa.AudId == auditorId
                        && aa.AudSchId == scheduleId);

                if (allocation == null)
                {
                    TempData["Error"] = "Schedule not found or access denied";
                    return RedirectToAction("AssignedSchedules");
                }

                return View(allocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing schedule {ScheduleId}", scheduleId);
                TempData["Error"] = "Failed to load schedule details";
                return RedirectToAction("AssignedSchedules");
            }
        }

        [HttpPost("StartAudit/{scheduleId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartAudit(long scheduleId)
        {
            try
            {
                var auditorId = GetCurrentAuditorId();

                var allocation = await _context.AuditorAllocations
                    .Include(aa => aa.Schedule)
                        .ThenInclude(s => s.Plant)
                    .FirstOrDefaultAsync(aa => aa.AudId == auditorId
                        && aa.AudSchId == scheduleId);

                if (allocation == null)
                {
                    TempData["Error"] = "Schedule not found or access denied";
                    return RedirectToAction("AssignedSchedules");
                }

                var schedule = allocation.Schedule;

                var audit = new Models.Audit
                {
                    AudSchId = scheduleId,
                    AuditTitle = $"Safety Audit - {schedule.Plant.PlantName} - {DateTime.Now:MMM dd, yyyy}",
                    AuditDescription = "Routine safety audit conducted as per schedule",
                    AuditStatus = Constants.AUDIT_STATUS_IN_PROGRESS
                };

                _context.Audits.Add(audit);
                schedule.AudSchStatus = Constants.AUDIT_SCHEDULE_SCHEDULED;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Audit started successfully!";
                return RedirectToAction("ExecuteAudit", new { auditId = audit.AuditId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting audit for schedule {ScheduleId}", scheduleId);
                TempData["Error"] = "Failed to start audit";
                return RedirectToAction("ViewSchedule", new { scheduleId });
            }
        }

        [HttpGet("Audit/Execute/{auditId}")]
        public async Task<IActionResult> ExecuteAudit(long auditId)
        {
            try
            {
                var auditorId = GetCurrentAuditorId();

                var audit = await _context.Audits
                    .Include(a => a.Schedule)
                        .ThenInclude(s => s.Plant)
                    .Include(a => a.Evidences)
                    .Include(a => a.CorrectiveActions)
                    .FirstOrDefaultAsync(a => a.AuditId == auditId);

                if (audit == null)
                {
                    TempData["Error"] = "Audit not found";
                    return RedirectToAction("Dashboard");
                }

                var allocation = await _context.AuditorAllocations
                    .FirstOrDefaultAsync(aa => aa.AudId == auditorId
                        && aa.AudSchId == audit.AudSchId);

                if (allocation == null)
                {
                    TempData["Error"] = "Access denied";
                    return RedirectToAction("Dashboard");
                }

                var responsiblePersons = await _context.ResponsiblePeople
                    .Include(rp => rp.AreaOwner)
                        .ThenInclude(ao => ao.Plants)
                    .Where(rp => rp.AreaOwner.Plants.Any(p => p.PlantId == audit.Schedule.PlantId))
                    .ToListAsync();

                var viewModel = new ExecuteAuditViewModel
                {
                    Audit = audit,
                    ResponsiblePersons = responsiblePersons
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing audit {AuditId}", auditId);
                TempData["Error"] = "Failed to load audit";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet("Audit/{auditId}/Attendance")]
        public async Task<IActionResult> RecordAttendance(long auditId)
        {
            try
            {
                var auditorId = GetCurrentAuditorId();

                var audit = await _context.Audits
                    .Include(a => a.Schedule)
                        .ThenInclude(s => s.Allocations)
                            .ThenInclude(aa => aa.Auditor)
                    .Include(a => a.Schedule)
                        .ThenInclude(s => s.Plant)
                    .FirstOrDefaultAsync(a => a.AuditId == auditId);

                if (audit == null)
                {
                    TempData["Error"] = "Audit not found";
                    return RedirectToAction("Dashboard");
                }

                var allocation = await _context.AuditorAllocations
                    .FirstOrDefaultAsync(aa => aa.AudId == auditorId
                        && aa.AudSchId == audit.AudSchId);

                if (allocation == null)
                {
                    TempData["Error"] = "Access denied";
                    return RedirectToAction("Dashboard");
                }

                return View(audit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading attendance for audit {AuditId}", auditId);
                TempData["Error"] = "Failed to load attendance page";
                return RedirectToAction("ExecuteAudit", new { auditId });
            }
        }

        [HttpPost("Audit/{auditId}/Attendance")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAttendance(long auditId, [FromForm] AttendanceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid attendance data";
                return RedirectToAction("RecordAttendance", new { auditId });
            }

            try
            {
                var audit = await _context.Audits.FindAsync(auditId);
                if (audit == null)
                {
                    TempData["Error"] = "Audit not found";
                    return RedirectToAction("Dashboard");
                }

                foreach (var attendance in model.Attendances)
                {
                    var existingAttendance = await _context.AuditAttendances
                        .FirstOrDefaultAsync(aa => aa.AuditId == auditId
                            && aa.AudId == attendance.AuditorId);

                    if (existingAttendance != null)
                    {
                        existingAttendance.AudAttendStatus = attendance.Status;
                        existingAttendance.AudArrivalTime = attendance.ArrivalTime;
                        existingAttendance.AudDepartTime = attendance.DepartureTime;
                        existingAttendance.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        _context.AuditAttendances.Add(new AuditAttendance
                        {
                            AuditId = auditId,
                            AudId = attendance.AuditorId,
                            AudAttendDate = DateOnly.FromDateTime(DateTime.Today),
                            AudAttendStatus = attendance.Status,
                            AudArrivalTime = attendance.ArrivalTime,
                            AudDepartTime = attendance.DepartureTime,
                            RespId = null
                        });
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Attendance records saved successfully!";
                return RedirectToAction("ExecuteAudit", new { auditId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving attendance for audit {AuditId}", auditId);
                TempData["Error"] = "Failed to save attendance records";
                return RedirectToAction("RecordAttendance", new { auditId });
            }
        }

        [HttpGet("Audit/{auditId}/Evidence")]
        public async Task<IActionResult> CollectEvidence(long auditId)
        {
            try
            {
                var auditorId = GetCurrentAuditorId();

                var audit = await _context.Audits
                    .Include(a => a.Schedule)
                        .ThenInclude(s => s.Plant)
                    .Include(a => a.Evidences)
                    .Include(a => a.CorrectiveActions)
                        .ThenInclude(ca => ca.ResponsiblePerson)
                    .FirstOrDefaultAsync(a => a.AuditId == auditId);

                if (audit == null)
                {
                    TempData["Error"] = "Audit not found";
                    return RedirectToAction("Dashboard");
                }

                var allocation = await _context.AuditorAllocations
                    .FirstOrDefaultAsync(aa => aa.AudId == auditorId
                        && aa.AudSchId == audit.AudSchId);

                if (allocation == null)
                {
                    TempData["Error"] = "Access denied";
                    return RedirectToAction("Dashboard");
                }

                return View(audit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading evidence collection for audit {AuditId}", auditId);
                TempData["Error"] = "Failed to load evidence collection page";
                return RedirectToAction("ExecuteAudit", new { auditId });
            }
        }

        [HttpPost("Audit/{auditId}/Evidence")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StoreEvidence(long auditId, [FromForm] EvidenceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid evidence data";
                return RedirectToAction("CollectEvidence", new { auditId });
            }

            try
            {
                var auditorId = GetCurrentAuditorId();

                var audit = await _context.Audits.FindAsync(auditId);
                if (audit == null)
                {
                    TempData["Error"] = "Audit not found";
                    return RedirectToAction("Dashboard");
                }

                var allocation = await _context.AuditorAllocations
                    .FirstOrDefaultAsync(aa => aa.AudId == auditorId
                        && aa.AudSchId == audit.AudSchId);

                if (allocation == null)
                {
                    TempData["Error"] = "Access denied";
                    return RedirectToAction("Dashboard");
                }

                string filePath = await UploadFileAsync(
                    model.EvidenceFile,
                    "evidence",
                    model.EvidenceTitle);

                var evidence = new Evidence
                {
                    AuditId = auditId,
                    EvidenceTitle = model.EvidenceTitle,
                    EvidenceUrl = filePath,
                    ActionId = model.ActionId
                };

                _context.Evidences.Add(evidence);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Evidence uploaded successfully!";
                return RedirectToAction("CollectEvidence", new { auditId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing evidence for audit {AuditId}", auditId);
                TempData["Error"] = "Failed to upload evidence";
                return RedirectToAction("CollectEvidence", new { auditId });
            }
        }

        [HttpPost("Audit/{auditId}/CorrectiveAction")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCorrectiveAction(long auditId, [FromForm] CorrectiveActionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid corrective action data";
                return RedirectToAction("ExecuteAudit", new { auditId });
            }

            try
            {
                var auditorId = GetCurrentAuditorId();

                var audit = await _context.Audits.FindAsync(auditId);
                if (audit == null)
                {
                    TempData["Error"] = "Audit not found";
                    return RedirectToAction("Dashboard");
                }

                var allocation = await _context.AuditorAllocations
                    .FirstOrDefaultAsync(aa => aa.AudId == auditorId
                        && aa.AudSchId == audit.AudSchId);

                if (allocation == null)
                {
                    TempData["Error"] = "Access denied";
                    return RedirectToAction("Dashboard");
                }

                var correctiveAction = new CorrectiveAction
                {
                    RespId = model.ResponsiblePersonId,
                    AuditId = auditId,
                    MgmtId = null,
                    ActionDescription = model.ActionDescription,
                    ActionDeadlineDate = DateOnly.FromDateTime(model.DeadlineDate),
                    ActionStatus = Constants.ACTION_STATUS_PENDING
                };

                _context.CorrectiveActions.Add(correctiveAction);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Corrective action created successfully!";
                return RedirectToAction("ExecuteAudit", new { auditId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating corrective action for audit {AuditId}", auditId);
                TempData["Error"] = "Failed to create corrective action";
                return RedirectToAction("ExecuteAudit", new { auditId });
            }
        }

        [HttpPost("Audit/{auditId}/Complete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteAudit(long auditId, [FromForm] CompleteAuditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid completion data";
                return RedirectToAction("ExecuteAudit", new { auditId });
            }

            try
            {
                var auditorId = GetCurrentAuditorId();

                var audit = await _context.Audits
                    .Include(a => a.Schedule)
                        .ThenInclude(s => s.Plant)
                    .FirstOrDefaultAsync(a => a.AuditId == auditId);

                if (audit == null)
                {
                    TempData["Error"] = "Audit not found";
                    return RedirectToAction("Dashboard");
                }

                var allocation = await _context.AuditorAllocations
                    .FirstOrDefaultAsync(aa => aa.AudId == auditorId
                        && aa.AudSchId == audit.AudSchId);

                if (allocation == null)
                {
                    TempData["Error"] = "Access denied";
                    return RedirectToAction("Dashboard");
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Update audit status
                    audit.AuditStatus = Constants.AUDIT_STATUS_UNDER_REVIEW;
                    audit.UpdatedAt = DateTime.UtcNow;

                    // Update schedule status
                    audit.Schedule.AudSchStatus = Constants.AUDIT_SCHEDULE_COMPLETED;
                    audit.Schedule.UpdatedAt = DateTime.UtcNow;

                    // Create audit history record
                    var auditHistory = new AuditHistory
                    {
                        AuditId = audit.AuditId,
                        AoId = audit.Schedule.Plant.AoId,
                        MgmtId = audit.Schedule.Plant.MgmtId,
                        AudHistoryTitle = $"Audit Completed - {audit.AuditTitle}",
                        AudHistoryStatus = Constants.AUDIT_HISTORY_STATUS_PENDING,
                        AudHistoryScore = model.Score,
                        AudHistoryComments = model.Comments,
                        AudHistoryEscalationLevel = DetermineEscalationLevel(model.Score)
                    };

                    _context.AuditHistories.Add(auditHistory);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Audit completed successfully and submitted for review!";
                    return RedirectToAction("Dashboard");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing audit {AuditId}", auditId);
                TempData["Error"] = "Failed to complete audit";
                return RedirectToAction("ExecuteAudit", new { auditId });
            }
        }

        [HttpPost("Evidence/{evidenceId}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvidence(long evidenceId)
        {
            try
            {
                var auditorId = GetCurrentAuditorId();

                var evidence = await _context.Evidences
                    .Include(e => e.Audit)
                        .ThenInclude(a => a.Schedule)
                    .FirstOrDefaultAsync(e => e.EvidenceId == evidenceId);

                if (evidence == null)
                {
                    TempData["Error"] = "Evidence not found";
                    return RedirectToAction("Dashboard");
                }

                var allocation = await _context.AuditorAllocations
                    .FirstOrDefaultAsync(aa => aa.AudId == auditorId
                        && aa.AudSchId == evidence.Audit.AudSchId);

                if (allocation == null)
                {
                    TempData["Error"] = "Access denied";
                    return RedirectToAction("Dashboard");
                }

                // Delete file from storage
                await RemoveFileAsync(evidence.EvidenceUrl);

                _context.Evidences.Remove(evidence);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Evidence deleted successfully!";
                return RedirectToAction("CollectEvidence", new { auditId = evidence.AuditId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting evidence {EvidenceId}", evidenceId);
                TempData["Error"] = "Failed to delete evidence";
                return RedirectToAction("Dashboard");
            }
        }

        private string DetermineEscalationLevel(decimal? score)
        {
            if (!score.HasValue) return Constants.ESCALATION_LEVEL_LOW;

            if (score >= Constants.EXCELLENT_AUDIT_SCORE)
                return Constants.ESCALATION_LEVEL_LOW;
            else if (score >= Constants.PASSING_AUDIT_SCORE)
                return Constants.ESCALATION_LEVEL_MEDIUM;
            else if (score >= 40)
                return Constants.ESCALATION_LEVEL_HIGH;
            else
                return Constants.ESCALATION_LEVEL_CRITICAL;
        }

        #region File Upload Methods

        private async Task<string> UploadFileAsync(IFormFile file, string path = "uploads", string slug = "dummy-slug")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;

                // Create slug-safe filename
                slug = slug.Replace(" ", "-").ToLower();
                var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{slug}-{currentDate}-{Guid.NewGuid()}{extension}";

                // Create directory if it doesn't exist
                var uploadPath = Path.Combine(_environment.WebRootPath, path);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/{path}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                throw;
            }
        }

        private async Task<bool> RemoveFileAsync(string path)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, path.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    await Task.Run(() => System.IO.File.Delete(fullPath));
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing file");
                return false;
            }
        }

        #endregion
    }
}