using Madar.Data;
using Madar.Models;
using Madar.ViewModels.AreaOwnerVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madar.Controllers.AreaOwner
{
    [Authorize(Roles = "AreaOwner")]
    [Route("AreaOwner")]
    public class AreaOwnerController : Controller
    {
        private readonly MadarDbContext _context;
        private readonly ILogger<AreaOwnerController> _logger;

        public AreaOwnerController(MadarDbContext context, ILogger<AreaOwnerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
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

                var plant = await _context.Plants
                    .FirstOrDefaultAsync(p => p.AoId == areaOwner.AoId);

                if (plant == null)
                {
                    TempData["ErrorMessage"] = "No plant assigned to your account.";
                    return View(new DashboardViewModel());
                }

                // Get statistics
                var equipmentCount = await _context.Equipment
                    .Where(e => e.PlantId == plant.PlantId)
                    .CountAsync();

                var upcomingAudits = await _context.AuditSchedules
                    .Where(a => a.PlantId == plant.PlantId && a.AudSchStatus == "Scheduled")
                    .CountAsync();

                var openActions = await (from ca in _context.CorrectiveActions
                                         join a in _context.Audits on ca.AuditId equals a.AuditId
                                         join sch in _context.AuditSchedules on a.AudSchId equals sch.AudSchId
                                         where sch.PlantId == plant.PlantId && ca.ActionStatus == "Open"
                                         select ca).CountAsync();

                var teamMembers = await _context.ResponsiblePeople
                    .Where(rp => rp.AoId == areaOwner.AoId)
                    .CountAsync();

                var equipmentMaintenance = await _context.Equipment
                    .Where(e => e.PlantId == plant.PlantId && e.EquipStatus == "Under_Maintenance")
                    .CountAsync();

                var today = DateOnly.FromDateTime(DateTime.Now);
                var overdueActions = await (from ca in _context.CorrectiveActions
                                            join a in _context.Audits on ca.AuditId equals a.AuditId
                                            join sch in _context.AuditSchedules on a.AudSchId equals sch.AudSchId
                                            where sch.PlantId == plant.PlantId
                                                 && ca.ActionDeadlineDate < today
                                                 && (ca.ActionStatus == "Open" || ca.ActionStatus == "In_Progress")
                                            select ca).CountAsync();

                var stats = new DashboardStats
                {
                    EquipmentCount = equipmentCount,
                    UpcomingAudits = upcomingAudits,
                    OpenActions = openActions,
                    TeamMembers = teamMembers,
                    EquipmentMaintenance = equipmentMaintenance,
                    OverdueActions = overdueActions
                };

                // Get recent audits
                var recentAudits = await (from sch in _context.AuditSchedules
                                          join p in _context.Plants on sch.PlantId equals p.PlantId
                                          where sch.PlantId == plant.PlantId
                                          orderby sch.AudSchDate descending
                                          select new RecentAuditDto
                                          {
                                              AudSchId = sch.AudSchId,
                                              AudSchDate = sch.AudSchDate.ToDateTime(TimeOnly.MinValue),
                                              AudSchStatus = sch.AudSchStatus ?? string.Empty,
                                              AudSchDuration = sch.AudSchDuration ?? 0,
                                              PlantName = p.PlantName ?? string.Empty
                                          }).Take(5).ToListAsync();

                // Get pending actions
                var pendingActions = await (from ca in _context.CorrectiveActions
                                            join rp in _context.ResponsiblePeople on ca.RespId equals rp.RespId
                                            join a in _context.Audits on ca.AuditId equals a.AuditId
                                            join sch in _context.AuditSchedules on a.AudSchId equals sch.AudSchId
                                            where sch.PlantId == plant.PlantId
                                                 && (ca.ActionStatus == "Open" || ca.ActionStatus == "In_Progress")
                                            orderby ca.ActionDeadlineDate
                                            select new PendingActionDto
                                            {
                                                //ActionId = (int)ca.ActionId,
                                                ActionId = ca.ActionId,
                                                ActionDescription = ca.ActionDescription ?? string.Empty,
                                                ActionDeadlineDate = ca.ActionDeadlineDate.ToDateTime(TimeOnly.MinValue),
                                                ActionStatus = ca.ActionStatus ?? string.Empty,
                                                ResponsiblePersonName = (rp.RespFname ?? "") + " " + (rp.RespLname ?? ""),
                                                AuditTitle = a.AuditTitle ?? string.Empty
                                            }).Take(5).ToListAsync();

                var viewModel = new DashboardViewModel
                {
                    AreaOwner = new AreaOwnerInfo
                    {
                        AoId = areaOwner.AoId,
                        AoFname = areaOwner.AoFname ?? string.Empty,
                        AoLname = areaOwner.AoLname ?? string.Empty,
                        AoEmail = areaOwner.AoEmail ?? string.Empty
                    },
                    Plant = new PlantInfo
                    {
                        //PlantId = (int)plant.PlantId,
                        PlantId = plant.PlantId,
                        PlantName = plant.PlantName ?? string.Empty,
                        PlantLocation = plant.PlantLocation ?? string.Empty,
                        PlantStatus = plant.PlantStatus ?? string.Empty,
                        PlantType = plant.PlantType ?? string.Empty
                    },
                    Stats = stats,
                    RecentAudits = recentAudits,
                    PendingActions = pendingActions
                };

                ViewData["PageTitle"] = "Dashboard";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
                return View(new DashboardViewModel());
            }
        }

        [HttpGet("ManagePlants")]
        public async Task<IActionResult> ManagePlants()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                var areaOwner = await _context.AreaOwners
                    .FirstOrDefaultAsync(ao => ao.UserId == userId);

                if (areaOwner == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var plant = await _context.Plants
                    .FirstOrDefaultAsync(p => p.AoId == areaOwner.AoId);

                if (plant == null)
                {
                    TempData["ErrorMessage"] = "No plant assigned to your account.";
                    return RedirectToAction(nameof(Dashboard));
                }

                var equipmentCount = await _context.Equipment
                    .Where(e => e.PlantId == plant.PlantId)
                    .CountAsync();

                var activeEquipment = await _context.Equipment
                    .Where(e => e.PlantId == plant.PlantId && e.EquipStatus == "In_Service")
                    .CountAsync();

                var viewModel = new PlantViewModel
                {
                    AreaOwner = new AreaOwnerInfo
                    {
                        AoId = areaOwner.AoId,
                        AoFname = areaOwner.AoFname ?? string.Empty,
                        AoLname = areaOwner.AoLname ?? string.Empty,
                        AoEmail = areaOwner.AoEmail ?? string.Empty
                    },
                    Plant = new PlantDetailsViewModel
                    {
                        PlantId = plant.PlantId,
                        PlantName = plant.PlantName ?? string.Empty,
                        PlantLocation = plant.PlantLocation ?? string.Empty,
                        PlantStatus = plant.PlantStatus ?? string.Empty,
                        PlantType = plant.PlantType ?? string.Empty,
                        PlantCapacity = plant.PlantCapacity,
                        PlantEquipmentCount = plant.PlantEquipmentCount ?? 0,
                        AoId = plant.AoId,
                        MgmtId = plant.MgmtId,
                        CreatedAt = plant.CreatedAt,
                        UpdatedAt = plant.UpdatedAt
                    },
                    EquipmentCount = equipmentCount,
                    ActiveEquipment = activeEquipment
                };

                ViewData["PageTitle"] = "Plant Management";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading plant management");
                TempData["ErrorMessage"] = "An error occurred while loading plant information.";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        [HttpPost("UpdatePlant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePlant(PlantDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return RedirectToAction(nameof(ManagePlants));
            }

            try
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                var areaOwner = await _context.AreaOwners
                    .FirstOrDefaultAsync(ao => ao.UserId == userId);

                var plant = await _context.Plants
                    .FirstOrDefaultAsync(p => p.PlantId == model.PlantId && p.AoId == areaOwner.AoId);

                if (plant == null)
                {
                    TempData["ErrorMessage"] = "Plant not found or access denied.";
                    return RedirectToAction(nameof(ManagePlants));
                }

                plant.PlantName = model.PlantName;
                plant.PlantLocation = model.PlantLocation;
                plant.PlantStatus = model.PlantStatus;
                plant.PlantType = model.PlantType;
                plant.PlantCapacity = model.PlantCapacity;
                plant.PlantEquipmentCount = model.PlantEquipmentCount;
                plant.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Plant information updated successfully!";
                return RedirectToAction(nameof(ManagePlants));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plant");
                TempData["ErrorMessage"] = $"Failed to update plant information: {ex.Message}";
                return RedirectToAction(nameof(ManagePlants));
            }
        }

        [HttpGet("GetPlantStats")]
        public async Task<IActionResult> GetPlantStats()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                var areaOwner = await _context.AreaOwners
                    .FirstOrDefaultAsync(ao => ao.UserId == userId);

                var plant = await _context.Plants
                    .FirstOrDefaultAsync(p => p.AoId == areaOwner.AoId);

                if (plant == null)
                {
                    return Json(new { error = "No plant found" });
                }

                var stats = new PlantStatsViewModel
                {
                    TotalEquipment = await _context.Equipment
                        .Where(e => e.PlantId == plant.PlantId)
                        .CountAsync(),

                    ActiveEquipment = await _context.Equipment
                        .Where(e => e.PlantId == plant.PlantId && e.EquipStatus == "In_Service")
                        .CountAsync(),

                    MaintenanceEquipment = await _context.Equipment
                        .Where(e => e.PlantId == plant.PlantId && e.EquipStatus == "Under_Maintenance")
                        .CountAsync(),

                    OutOfService = await _context.Equipment
                        .Where(e => e.PlantId == plant.PlantId && e.EquipStatus == "Out_of_Service")
                        .CountAsync()
                };

                return Json(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting plant stats");
                return Json(new { error = ex.Message });
            }
        }
    }
}