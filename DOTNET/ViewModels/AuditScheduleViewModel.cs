using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.AreaOwnerVMs
{
    public class AuditScheduleListViewModel
    {
        public AreaOwnerInfo AreaOwner { get; set; }
        public PlantInfo Plant { get; set; }
        public List<AuditScheduleDto> Schedules { get; set; }
        public List<AuditorDto> Auditors { get; set; }
        public AuditScheduleFilters Filters { get; set; }
    }

    public class AuditScheduleDto
    {
        public long AudSchId { get; set; } 
        //public DateTime AudSchDate { get; set; }
        public DateOnly AudSchDate { get; set; }
        public int? AudSchDuration { get; set; }
        public string AudSchStatus { get; set; }
        public string PlantName { get; set; }
        public List<AllocatedAuditor> AllocatedAuditors { get; set; }
    }

    public class AllocatedAuditor
    {
        public long AudId { get; set; } 
        public string AuditorName { get; set; }
        public string RoleType { get; set; }
    }

    public class AuditorDto
    {
        public long AudId { get; set; } 
        public string AudFname { get; set; }
        public string AudLname { get; set; }
        public string FullName => $"{AudFname} {AudLname}";
        public string AudEmail { get; set; }
    }

    public class AuditScheduleFilters
    {
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? AuditorId { get; set; } 
    }

    public class CreateAuditScheduleViewModel
    {
        [Required(ErrorMessage = "Audit date is required")]
        [Display(Name = "Audit Date")]
        [DataType(DataType.Date)]
        public DateOnly AudSchDate { get; set; } 

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 24, ErrorMessage = "Duration must be between 1 and 24 hours")]
        [Display(Name = "Duration (hours)")]
        public int AudSchDuration { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string AudSchStatus { get; set; }

        [Required(ErrorMessage = "At least one auditor is required")]
        [MinLength(1, ErrorMessage = "At least one auditor must be assigned")]
        public List<AuditorAllocationDto> Auditors { get; set; }
    }

    public class AuditorAllocationDto
    {
        [Required]
        public long AudId { get; set; } 

        [Required]
        [StringLength(100)]
        public string RoleType { get; set; }
    }

    public class UpdateAuditScheduleViewModel
    {
        public long AudSchId { get; set; } 

        [Required(ErrorMessage = "Audit date is required")]
        [Display(Name = "Audit Date")]
        [DataType(DataType.Date)]
        public DateOnly AudSchDate { get; set; } 

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 24)]
        [Display(Name = "Duration (hours)")]
        public int AudSchDuration { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string AudSchStatus { get; set; }
    }
}