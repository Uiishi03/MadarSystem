using Madar.Data;
using Madar.Models;
using Madar.ViewModels.AreaOwnerVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madar.Controllers.AreaOwner
{
    [Authorize(Roles = "AreaOwner")]
    [Route("AreaOwner/Equipment")]
    public class EquipmentController : Controller
    {
        private readonly MadarDbContext _context;
        private readonly ILogger<EquipmentController> _logger;

        public EquipmentController(MadarDbContext context, ILogger<EquipmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("Manage")]
        public async Task<IActionResult> ManageEquipment(int pageNumber = 1, int pageSize = 8)
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
                    return RedirectToAction("Dashboard", "AreaOwner");
                }

                var equipmentQuery = _context.Equipment
                    .Where(e => e.PlantId == plant.PlantId)
                    .OrderBy(e => e.EquipName);

                var totalCount = await equipmentQuery.CountAsync();

                var equipment = await equipmentQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => new EquipmentDto
                    {
                        EquipId = (int)e.EquipId,
                        EquipName = e.EquipName ?? string.Empty,
                        EquipType = e.EquipType ?? string.Empty,
                        EquipModel = e.EquipModel ?? string.Empty,
                        EquipStatus = e.EquipStatus ?? string.Empty,
                        EquipLocation = e.EquipLocation ?? string.Empty,
                        EquipCapacity = e.EquipCapacity,
                        EquipMaintenCycle = e.EquipMaintenCycle ?? string.Empty,
                        CreatedAt = e.CreatedAt
                    })
                    .ToListAsync();

                var viewModel = new EquipmentListViewModel
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
                    Equipment = equipment,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                ViewData["PageTitle"] = "Equipment Management";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading equipment");
                TempData["ErrorMessage"] = "An error occurred while loading equipment.";
                return RedirectToAction("Dashboard", "AreaOwner");
            }
        }

        [HttpPost("Store")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StoreEquipment(CreateEquipmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return RedirectToAction(nameof(ManageEquipment));
            }

            try
            {
                //var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var userId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var areaOwner = await _context.AreaOwners
                    .FirstOrDefaultAsync(ao => ao.UserId == userId);

                var plant = await _context.Plants
                    .FirstOrDefaultAsync(p => p.AoId == areaOwner.AoId);

                if (plant == null)
                {
                    TempData["ErrorMessage"] = "No plant assigned to your account.";
                    return RedirectToAction(nameof(ManageEquipment));
                }

                var equipment = new Equipment
                {
                    PlantId = plant.PlantId,
                    EquipName = model.EquipName,
                    EquipType = model.EquipType,
                    EquipModel = model.EquipModel,
                    EquipStatus = model.EquipStatus,
                    EquipLocation = model.EquipLocation,
                    EquipCapacity = model.EquipCapacity,
                    EquipMaintenCycle = model.EquipMaintenCycle,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Equipment.Add(equipment);
                await _context.SaveChangesAsync();

                // Update plant equipment count
                var equipmentCount = await _context.Equipment
                    .Where(e => e.PlantId == plant.PlantId)
                    .CountAsync();

                plant.PlantEquipmentCount = equipmentCount;
                plant.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Equipment added successfully!";
                return RedirectToAction(nameof(ManageEquipment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding equipment");
                TempData["ErrorMessage"] = $"Failed to add equipment: {ex.Message}";
                return RedirectToAction(nameof(ManageEquipment));
            }
        }

        [HttpPost("Update/{equipmentId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEquipment(int equipmentId, UpdateEquipmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return RedirectToAction(nameof(ManageEquipment));
            }

            try
            {
                //var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var userId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                var areaOwner = await _context.AreaOwners
                    .FirstOrDefaultAsync(ao => ao.UserId == userId);

                var plant = await _context.Plants
                    .FirstOrDefaultAsync(p => p.AoId == areaOwner.AoId);

                var equipment = await _context.Equipment
                    .FirstOrDefaultAsync(e => e.EquipId == equipmentId && e.PlantId == plant.PlantId);

                if (equipment == null)
                {
                    TempData["ErrorMessage"] = "Equipment not found or access denied.";
                    return RedirectToAction(nameof(ManageEquipment));
                }

                equipment.EquipName = model.EquipName;
                equipment.EquipType = model.EquipType;
                equipment.EquipModel = model.EquipModel;
                equipment.EquipStatus = model.EquipStatus;
                equipment.EquipLocation = model.EquipLocation;
                equipment.EquipCapacity = model.EquipCapacity;
                equipment.EquipMaintenCycle = model.EquipMaintenCycle;
                equipment.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Equipment updated successfully!";
                return RedirectToAction(nameof(ManageEquipment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating equipment");
                TempData["ErrorMessage"] = $"Failed to update equipment: {ex.Message}";
                return RedirectToAction(nameof(ManageEquipment));
            }
        }

        [HttpPost("Delete/{equipmentId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEquipment(int equipmentId)
        {
            try
            {
                //var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var userId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                var areaOwner = await _context.AreaOwners
                    .FirstOrDefaultAsync(ao => ao.UserId == userId);

                var plant = await _context.Plants
                    .FirstOrDefaultAsync(p => p.AoId == areaOwner.AoId);

                var equipment = await _context.Equipment
                    .FirstOrDefaultAsync(e => e.EquipId == equipmentId && e.PlantId == plant.PlantId);

                if (equipment == null)
                {
                    TempData["ErrorMessage"] = "Equipment not found or access denied.";
                    return RedirectToAction(nameof(ManageEquipment));
                }

                _context.Equipment.Remove(equipment);
                await _context.SaveChangesAsync();

                // Update plant equipment count
                var equipmentCount = await _context.Equipment
                    .Where(e => e.PlantId == plant.PlantId)
                    .CountAsync();

                plant.PlantEquipmentCount = equipmentCount;
                plant.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Equipment deleted successfully!";
                return RedirectToAction(nameof(ManageEquipment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting equipment");
                TempData["ErrorMessage"] = $"Failed to delete equipment: {ex.Message}";
                return RedirectToAction(nameof(ManageEquipment));
            }
        }
    }
}