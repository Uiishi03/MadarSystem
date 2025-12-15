using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.AreaOwnerVMs
{
    public class EquipmentListViewModel
    {
        public AreaOwnerInfo AreaOwner { get; set; }
        public PlantInfo Plant { get; set; }
        public List<EquipmentDto> Equipment { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class EquipmentDto
    {
        public long EquipId { get; set; }
        public string EquipName { get; set; }
        public string EquipType { get; set; }
        public string EquipModel { get; set; }
        public string EquipStatus { get; set; }
        public string EquipLocation { get; set; }
        public int? EquipCapacity { get; set; }
        public string EquipMaintenCycle { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateEquipmentViewModel
    {
        [Required(ErrorMessage = "Equipment name is required")]
        [StringLength(255)]
        [Display(Name = "Equipment Name")]
        public string EquipName { get; set; }

        [Required(ErrorMessage = "Equipment type is required")]
        [StringLength(255)]
        [Display(Name = "Equipment Type")]
        public string EquipType { get; set; }

        [StringLength(255)]
        [Display(Name = "Model")]
        public string EquipModel { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string EquipStatus { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(500)]
        [Display(Name = "Location")]
        public string EquipLocation { get; set; }

        [StringLength(100)]
        [Display(Name = "Capacity")]
        public int? EquipCapacity { get; set; }

        [StringLength(100)]
        [Display(Name = "Maintenance Cycle")]
        public string EquipMaintenCycle { get; set; }
    }

    public class UpdateEquipmentViewModel : CreateEquipmentViewModel
    {
        public long EquipId { get; set; }
    }
}
