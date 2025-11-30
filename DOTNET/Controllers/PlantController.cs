using Madar.Data;
using Madar.Models;
using Madar.ViewModels.ManagementVMs.PlantVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madar.Controllers.Management
{
    [Authorize]
    [Route("Management/Plants")]
    public class PlantController : Controller
    {
        private readonly MadarDbContext _context;
        private readonly ILogger<PlantController> _logger;

        public PlantController(MadarDbContext context, ILogger<PlantController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Display plant management page with pagination
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 9)
        {
            try
            {
                var totalPlants = await _context.Plants.CountAsync();
                var totalPages = (int)Math.Ceiling(totalPlants / (double)pageSize);

                var plants = await _context.Plants
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var areaOwners = await _context.AreaOwners.ToListAsync();
                var managements = await _context.Managements.ToListAsync();

                var viewModel = new PlantIndexViewModel
                {
                    Plants = plants,
                    AreaOwners = areaOwners,
                    Managements = managements,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalPlants = totalPlants
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading plants");
                TempData["Error"] = "Failed to load plants. Please try again.";
                return View(new PlantIndexViewModel());
            }
        }

        /// <summary>
        /// Store new plant
        /// </summary>
        [HttpPost("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(CreatePlantViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid form data. Please check all fields.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var areaOwnerExists = await _context.AreaOwners.AnyAsync(ao => ao.AoId == model.AoId);
                if (!areaOwnerExists)
                {
                    TempData["Error"] = "Selected Area Owner does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                var managementExists = await _context.Managements.AnyAsync(m => m.MgmtId == model.MgmtId);
                if (!managementExists)
                {
                    TempData["Error"] = "Selected Management does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                var plant = new Plant
                {
                    PlantName = model.PlantName,
                    PlantLocation = model.PlantLocation,
                    PlantType = model.PlantType,
                    PlantCapacity = model.PlantCapacity, // Now int as per DB
                    PlantEquipmentCount = model.PlantEquipmentCount,
                    PlantStatus = model.PlantStatus,
                    AoId = model.AoId, // Now long as per DB
                    MgmtId = model.MgmtId, // Now long as per DB
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Plants.Add(plant);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Plant created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating plant");
                TempData["Error"] = "Failed to create plant. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Update existing plant
        /// </summary>
        [HttpPost("Update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(long id, UpdatePlantViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid form data. Please check all fields.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var plant = await _context.Plants.FindAsync(id);
                if (plant == null)
                {
                    TempData["Error"] = "Plant not found.";
                    return RedirectToAction(nameof(Index));
                }

                var areaOwnerExists = await _context.AreaOwners.AnyAsync(ao => ao.AoId == model.AoId);
                if (!areaOwnerExists)
                {
                    TempData["Error"] = "Selected Area Owner does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                var managementExists = await _context.Managements.AnyAsync(m => m.MgmtId == model.MgmtId);
                if (!managementExists)
                {
                    TempData["Error"] = "Selected Management does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                plant.PlantName = model.PlantName;
                plant.PlantLocation = model.PlantLocation;
                plant.PlantType = model.PlantType;
                plant.PlantCapacity = model.PlantCapacity;
                plant.PlantEquipmentCount = model.PlantEquipmentCount;
                plant.PlantStatus = model.PlantStatus;
                plant.AoId = model.AoId;
                plant.MgmtId = model.MgmtId;
                plant.UpdatedAt = DateTime.UtcNow;

                _context.Plants.Update(plant);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Plant updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plant");
                TempData["Error"] = "Failed to update plant. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Delete plant
        /// </summary>
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var plant = await _context.Plants.FindAsync(id);
                if (plant == null)
                {
                    TempData["Error"] = "Plant not found.";
                    return RedirectToAction(nameof(Index));
                }

                var hasEquipment = await _context.Equipment.AnyAsync(e => e.PlantId == id);
                if (hasEquipment)
                {
                    TempData["Error"] = "Cannot delete plant with associated equipment. Please remove equipment first.";
                    return RedirectToAction(nameof(Index));
                }

                var hasSchedules = await _context.AuditSchedules.AnyAsync(s => s.PlantId == id);
                if (hasSchedules)
                {
                    TempData["Error"] = "Cannot delete plant with associated audit schedules. Please remove schedules first.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Plants.Remove(plant);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Plant deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting plant");
                TempData["Error"] = "Failed to delete plant. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Get plant details (for AJAX/Edit modal)
        /// </summary>
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var plant = await _context.Plants
                    .Include(p => p.AreaOwner)
                        .ThenInclude(ao => ao.User)
                    .Include(p => p.Management)
                        .ThenInclude(m => m.User)
                    .FirstOrDefaultAsync(p => p.PlantId == id);

                if (plant == null)
                {
                    return NotFound(new { success = false, message = "Plant not found" });
                }

                var result = new
                {
                    success = true,
                    data = new
                    {
                        plantId = plant.PlantId,
                        plantName = plant.PlantName,
                        plantLocation = plant.PlantLocation,
                        plantType = plant.PlantType,
                        plantCapacity = plant.PlantCapacity,
                        plantEquipmentCount = plant.PlantEquipmentCount,
                        plantStatus = plant.PlantStatus,
                        aoId = plant.AoId,
                        mgmtId = plant.MgmtId,
                        areaOwner = plant.AreaOwner != null ? new
                        {
                            id = plant.AreaOwner.AoId,
                            name = $"{plant.AreaOwner.AoFname} {plant.AreaOwner.AoLname}".Trim(),
                            email = plant.AreaOwner.AoEmail
                        } : null,
                        management = plant.Management != null ? new
                        {
                            id = plant.Management.MgmtId,
                            name = $"{plant.Management.MgmtFname} {plant.Management.MgmtLname}".Trim(),
                            email = plant.Management.MgmtEmail
                        } : null
                    }
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching plant details");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}