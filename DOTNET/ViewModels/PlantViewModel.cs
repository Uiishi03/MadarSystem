using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.AreaOwnerVMs
{
    public class PlantViewModel
    {
        public AreaOwnerInfo AreaOwner { get; set; }
        public PlantDetailsViewModel Plant { get; set; }
        public int EquipmentCount { get; set; }
        public int ActiveEquipment { get; set; }
    }

    public class PlantDetailsViewModel
    {
        public long PlantId { get; set; } 

        [Required(ErrorMessage = "Plant name is required")]
        [StringLength(255)]
        [Display(Name = "Plant Name")]
        public string PlantName { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(500)]
        [Display(Name = "Location")]
        public string PlantLocation { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string PlantStatus { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [StringLength(255)]
        [Display(Name = "Plant Type")]
        public string PlantType { get; set; }

        [Display(Name = "Capacity")]
        public int? PlantCapacity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Equipment Count")]
        public int? PlantEquipmentCount { get; set; }

        public long AoId { get; set; }
        public long? MgmtId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PlantStatsViewModel
    {
        public int TotalEquipment { get; set; }
        public int ActiveEquipment { get; set; }
        public int MaintenanceEquipment { get; set; }
        public int OutOfService { get; set; }
    }
}