using Madar.Models;
using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.ManagementVMs.PlantVMs
{
    public class PlantIndexViewModel
    {
        public List<Plant> Plants { get; set; } = new();
        public List<AreaOwner> AreaOwners { get; set; } = new();
        public List<Management> Managements { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalPages { get; set; }
        public int TotalPlants { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class CreatePlantViewModel
    {
        [Required(ErrorMessage = "Plant name is required")]
        [StringLength(150, MinimumLength = 2)]
        [Display(Name = "Plant Name")]
        public string PlantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plant location is required")]
        [StringLength(200, MinimumLength = 5)]
        [Display(Name = "Location")]
        public string PlantLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plant type is required")]
        [StringLength(80, MinimumLength = 2)]
        [Display(Name = "Plant Type")]
        public string PlantType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plant capacity is required")]
        [Range(0, int.MaxValue)] // int in DB
        [Display(Name = "Capacity")]
        public int PlantCapacity { get; set; }

        [Required(ErrorMessage = "Equipment count is required")]
        [Range(0, 10000)]
        [Display(Name = "Equipment Count")]
        public int PlantEquipmentCount { get; set; }

        [Required(ErrorMessage = "Plant status is required")]
        [RegularExpression("^(Active|Inactive|Under_Maintenance|Decommissioned)$")]
        [Display(Name = "Status")]
        public string PlantStatus { get; set; } = "Active";

        [Required(ErrorMessage = "Area Owner is required")]
        [Display(Name = "Area Owner")]
        public long AoId { get; set; } // bigint in DB

        [Required(ErrorMessage = "Management is required")]
        [Display(Name = "Management")]
        public long MgmtId { get; set; } // bigint in DB
    }

    public class UpdatePlantViewModel
    {
        [Required(ErrorMessage = "Plant name is required")]
        [StringLength(150, MinimumLength = 2)]
        [Display(Name = "Plant Name")]
        public string PlantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plant location is required")]
        [StringLength(200, MinimumLength = 5)]
        [Display(Name = "Location")]
        public string PlantLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plant type is required")]
        [StringLength(80, MinimumLength = 2)]
        [Display(Name = "Plant Type")]
        public string PlantType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plant capacity is required")]
        [Range(0, int.MaxValue)]
        [Display(Name = "Capacity")]
        public int PlantCapacity { get; set; }

        [Required(ErrorMessage = "Equipment count is required")]
        [Range(0, 10000)]
        [Display(Name = "Equipment Count")]
        public int PlantEquipmentCount { get; set; }

        [Required(ErrorMessage = "Plant status is required")]
        [RegularExpression("^(Active|Inactive|Under_Maintenance|Decommissioned)$")]
        [Display(Name = "Status")]
        public string PlantStatus { get; set; } = "Active";

        [Required(ErrorMessage = "Area Owner is required")]
        [Display(Name = "Area Owner")]
        public long AoId { get; set; } // bigint in DB

        [Required(ErrorMessage = "Management is required")]
        [Display(Name = "Management")]
        public long MgmtId { get; set; } // bigint in DB
    }

    public class PlantDetailsViewModel
    {
        public long PlantId { get; set; } // bigint in DB
        public string PlantName { get; set; } = string.Empty;
        public string PlantLocation { get; set; } = string.Empty;
        public string PlantType { get; set; } = string.Empty;
        public int PlantCapacity { get; set; } // int in DB
        public int PlantEquipmentCount { get; set; }
        public string PlantStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public long AoId { get; set; } // bigint in DB
        public string AreaOwnerName { get; set; } = string.Empty;
        public string AreaOwnerEmail { get; set; } = string.Empty;
        public string AreaOwnerExtension { get; set; } = string.Empty;

        public long MgmtId { get; set; } // bigint in DB
        public string ManagementName { get; set; } = string.Empty;
        public string ManagementEmail { get; set; } = string.Empty;
        public string ManagementExtension { get; set; } = string.Empty;

        public int TotalAudits { get; set; }
        public int CompletedAudits { get; set; }
        public int PendingAudits { get; set; }
        public double ComplianceRate { get; set; }
    }

    public class PlantStatisticsViewModel
    {
        public int TotalPlants { get; set; }
        public int ActivePlants { get; set; }
        public int InactivePlants { get; set; }
        public int UnderMaintenancePlants { get; set; }
        public int DecommissionedPlants { get; set; }
        public int TotalEquipment { get; set; }
        public double AverageEquipmentPerPlant { get; set; }
        public Dictionary<string, int> PlantsByType { get; set; } = new();
        public Dictionary<string, int> PlantsByStatus { get; set; } = new();
    }
}
