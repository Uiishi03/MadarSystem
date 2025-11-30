// FILE: Controllers/Management/PerformanceReportController.cs
using Madar.Data;
using Madar.ViewModels.ManagementVMs.PerformanceReportVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madar.Controllers.Management
{
    [Authorize]
    [Route("Management/PerformanceReports")]
    public class PerformanceReportController : Controller
    {
        private readonly MadarDbContext _context;
        private readonly ILogger<PerformanceReportController> _logger;

        public PerformanceReportController(MadarDbContext context, ILogger<PerformanceReportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string period = "monthly", long? plantId = null, long? auditorId = null)
        {
            try
            {
                var auditHistoryQuery = _context.AuditHistories.AsQueryable();
                var auditsQuery = _context.Audits.AsQueryable();

                if (plantId.HasValue)
                {
                    var scheduleIds = await _context.AuditSchedules
                        .Where(s => s.PlantId == plantId.Value)
                        .Select(s => s.AudSchId)
                        .ToListAsync();

                    var auditIds = await _context.Audits
                        .Where(a => scheduleIds.Contains(a.AudSchId))
                        .Select(a => a.AuditId)
                        .ToListAsync();

                    auditHistoryQuery = auditHistoryQuery.Where(ah => auditIds.Contains(ah.AuditId));
                    auditsQuery = auditsQuery.Where(a => auditIds.Contains(a.AuditId));
                }

                var auditHistories = await auditHistoryQuery
                    .OrderByDescending(ah => ah.CreatedAt)
                    .ToListAsync();

                var audits = await auditsQuery
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                var plants = await _context.Plants.ToListAsync();
                var auditors = await _context.Auditors.ToListAsync();

                if (auditorId.HasValue)
                {
                    auditors = auditors.Where(a => a.AudId == auditorId.Value).ToList();
                }

                var complianceRate = CalculateComplianceRate(auditHistories);
                var averageScore = CalculateAverageScore(auditHistories);
                var completionRate = CalculateCompletionRate(audits);
                var openActions = await _context.CorrectiveActions
                    .Where(ca => ca.ActionStatus != "Completed")
                    .CountAsync();
                var overdueActions = await _context.CorrectiveActions
                    .Where(ca => ca.ActionStatus == "Overdue")
                    .CountAsync();

                var plantPerformance = await GetPlantPerformance(plants);
                var auditorPerformance = await GetAuditorPerformance(auditors);
                var timelineData = GetTimelineData(auditHistories, period);

                var viewModel = new PerformanceReportIndexViewModel
                {
                    AuditHistories = auditHistories,
                    Audits = audits,
                    Plants = plants,
                    Auditors = auditors,
                    PlantId = plantId,
                    AuditorId = auditorId,
                    Period = period,
                    ComplianceRate = complianceRate,
                    AverageScore = averageScore,
                    CompletionRate = completionRate,
                    OpenActions = openActions,
                    OverdueActions = overdueActions,
                    PlantPerformance = plantPerformance,
                    AuditorPerformance = auditorPerformance,
                    TimelineData = timelineData
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading performance reports");
                TempData["Error"] = "Failed to load performance reports. Please try again.";
                return View(new PerformanceReportIndexViewModel());
            }
        }

        [HttpPost("GenerateReport")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateReport(string period, long? plantId, long? auditorId)
        {
            try
            {
                // TODO: Implement PDF generation logic
                TempData["Success"] = "PDF report generated successfully!";
                return RedirectToAction(nameof(Index), new { period, plantId, auditorId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF report");
                TempData["Error"] = "Failed to generate report. Please try again.";
                return RedirectToAction(nameof(Index), new { period, plantId, auditorId });
            }
        }

        #region Helper Methods

        private double CalculateComplianceRate(List<Models.AuditHistory> auditHistories)
        {
            if (!auditHistories.Any()) return 0;

            var totalScore = auditHistories.Sum(ah => (double)(ah.AudHistoryScore ?? 0));
            var maxPossibleScore = auditHistories.Count * 100;

            return maxPossibleScore > 0 ? Math.Round(totalScore / maxPossibleScore * 100) : 0;
        }

        private double CalculateAverageScore(List<Models.AuditHistory> auditHistories)
        {
            if (!auditHistories.Any()) return 0;

            return Math.Round((double)auditHistories.Average(ah => ah.AudHistoryScore ?? 0));
        }

        private double CalculateCompletionRate(List<Models.Audit> audits)
        {
            if (!audits.Any()) return 0;

            var completed = audits.Count(a => a.AuditStatus == "Closed");
            return Math.Round((double)completed / audits.Count * 100);
        }

        private async Task<List<PlantPerformanceViewModel>> GetPlantPerformance(List<Models.Plant> plants)
        {
            var performanceList = new List<PlantPerformanceViewModel>();

            foreach (var plant in plants)
            {
                var schedules = await _context.AuditSchedules
                    .Where(s => s.PlantId == plant.PlantId)
                    .ToListAsync();

                var scheduleIds = schedules.Select(s => s.AudSchId).ToList();

                var audits = await _context.Audits
                    .Where(a => scheduleIds.Contains(a.AudSchId))
                    .ToListAsync();

                var auditIds = audits.Select(a => a.AuditId).ToList();

                var histories = await _context.AuditHistories
                    .Where(h => auditIds.Contains(h.AuditId))
                    .ToListAsync();

                var scores = histories.Select(h => (double)(h.AudHistoryScore ?? 0)).ToList();

                var equipmentCount = await _context.Equipment
                    .CountAsync(e => e.PlantId == plant.PlantId);

                performanceList.Add(new PlantPerformanceViewModel
                {
                    Plant = plant,
                    AuditCount = audits.Count,
                    AverageScore = scores.Any() ? Math.Round(scores.Average()) : 0,
                    EquipmentCount = equipmentCount,
                    ComplianceRate = scores.Any() ? Math.Round(scores.Average()) : 0
                });
            }

            return performanceList.OrderByDescending(p => p.AverageScore).ToList();
        }

        private async Task<List<AuditorPerformanceViewModel>> GetAuditorPerformance(List<Models.Auditor> auditors)
        {
            var performanceList = new List<AuditorPerformanceViewModel>();

            foreach (var auditor in auditors)
            {
                var allocations = await _context.AuditorAllocations
                    .Where(aa => aa.AudId == auditor.AudId)
                    .ToListAsync();

                var scheduleIds = allocations.Select(aa => aa.AudSchId).ToList();

                var audits = await _context.Audits
                    .Where(a => scheduleIds.Contains(a.AudSchId))
                    .ToListAsync();

                var auditIds = audits.Select(a => a.AuditId).ToList();

                var histories = await _context.AuditHistories
                    .Where(h => auditIds.Contains(h.AuditId))
                    .ToListAsync();

                var scores = histories.Select(h => (double)(h.AudHistoryScore ?? 0)).ToList();

                var completedAudits = audits.Count(a => a.AuditStatus == "Closed");
                var completionRate = audits.Any()
                    ? Math.Round((double)completedAudits / audits.Count * 100)
                    : 0;

                performanceList.Add(new AuditorPerformanceViewModel
                {
                    Auditor = auditor,
                    AuditCount = audits.Count,
                    AverageScore = scores.Any() ? Math.Round(scores.Average()) : 0,
                    CompletionRate = completionRate
                });
            }

            return performanceList.OrderByDescending(a => a.AverageScore).ToList();
        }

        private List<TimelineDataViewModel> GetTimelineData(List<Models.AuditHistory> auditHistories, string period)
        {
            var data = new List<TimelineDataViewModel>();
            var now = DateTime.Now;

            if (period == "monthly")
            {
                for (int i = 5; i >= 0; i--)
                {
                    var date = now.AddMonths(-i);
                    var monthData = auditHistories
                        .Where(h => h.CreatedAt.Year == date.Year && h.CreatedAt.Month == date.Month)
                        .ToList();

                    data.Add(new TimelineDataViewModel
                    {
                        Period = date.ToString("MMM yyyy"),
                        Score = monthData.Any() ? Math.Round((double)monthData.Average(h => h.AudHistoryScore ?? 0)) : 0,
                        Count = monthData.Count
                    });
                }
            }
            else if (period == "quarterly")
            {
                for (int i = 3; i >= 0; i--)
                {
                    var date = now.AddMonths(-i * 3);
                    var quarter = (date.Month - 1) / 3 + 1;
                    var quarterData = auditHistories
                        .Where(h => h.CreatedAt.Year == date.Year && (h.CreatedAt.Month - 1) / 3 + 1 == quarter)
                        .ToList();

                    data.Add(new TimelineDataViewModel
                    {
                        Period = $"Q{quarter} {date.Year}",
                        Score = quarterData.Any() ? Math.Round((double)quarterData.Average(h => h.AudHistoryScore ?? 0)) : 0,
                        Count = quarterData.Count
                    });
                }
            }
            else // yearly
            {
                for (int i = 2; i >= 0; i--)
                {
                    var date = now.AddYears(-i);
                    var yearData = auditHistories
                        .Where(h => h.CreatedAt.Year == date.Year)
                        .ToList();

                    data.Add(new TimelineDataViewModel
                    {
                        Period = date.Year.ToString(),
                        Score = yearData.Any() ? Math.Round((double)yearData.Average(h => h.AudHistoryScore ?? 0)) : 0,
                        Count = yearData.Count
                    });
                }
            }

            return data;
        }

        #endregion
    }
}