using Madar.Models;
using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.ManagementVMs.AuditScheduleVMs
{
    /// <summary>
    /// Main view model for audit schedule index page
    /// </summary>
    public class AuditScheduleIndexViewModel
    {
        public List<AuditSchedule> Schedules { get; set; } = new();
        public List<Plant> Plants { get; set; } = new();
        public List<Auditor> Auditors { get; set; } = new();
        public string View { get; set; } = "list";
        public double CompletionRate { get; set; }
        public int TotalSchedules { get; set; }
        public int CompletedSchedules { get; set; }
        public List<AuditScheduleMapViewModel> Maps { get; set; } = new();
    }

    /// <summary>
    /// View model for auditor allocation in schedule
    /// </summary>
    public class AuditorAllocationViewModel
    {
        public long AudId { get; set; } // bigint in DB
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleType { get; set; } = string.Empty;
        public DateOnly AssignedDate { get; set; }
    }

    /// <summary>
    /// View model for audit schedule details
    /// </summary>
    public class AuditScheduleDetailsViewModel
    {
        public long AudSchId { get; set; } // bigint in DB
        public long PlantId { get; set; } // bigint in DB
        public string PlantName { get; set; } = string.Empty;
        public string PlantLocation { get; set; } = string.Empty;
        public DateOnly AudSchDate { get; set; }
        public int AudSchDuration { get; set; }
        public string AudSchStatus { get; set; } = string.Empty;
        public List<AuditorAllocationViewModel> Auditors { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// View model for audit schedule mapping (calendar/list display)
    /// </summary>
    public class AuditScheduleMapViewModel
    {
        public long Id { get; set; } // bigint in DB
        public string Title { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> Auditors { get; set; } = new();
    }


    /// <summary>
    /// View model for creating new audit schedule
    /// </summary>
    public class CreateAuditScheduleViewModel
    {
        [Required(ErrorMessage = "Plant is required")]
        [Display(Name = "Plant")]
        public long PlantId { get; set; } // bigint in DB

        [Required(ErrorMessage = "Audit date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Audit Date")]
        public DateOnly AudSchDate { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, int.MaxValue)]
        [Display(Name = "Duration (hours)")]
        public int AudSchDuration { get; set; }

        [Required(ErrorMessage = "At least one auditor is required")]
        [MinLength(1)]
        [Display(Name = "Auditors")]
        public List<long> Auditors { get; set; } = new(); // bigint in DB
    }

    /// <summary>
    /// View model for updating audit schedule status
    /// </summary>
    public class UpdateAuditScheduleStatusViewModel
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Scheduled|Postponed|Completed|Cancelled|In_Progress)$")]
        [Display(Name = "Status")]
        public string AudSchStatus { get; set; } = string.Empty;
    }
}
