using Madar.Models;
using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.ManagementVMs.CorrectiveActionVMs
{
    public class CorrectiveActionIndexViewModel
    {
        public List<CorrectiveAction> CorrectiveActions { get; set; } = new();
        public List<ResponsiblePerson> ResponsiblePersons { get; set; } = new();
        public List<Audit> Audits { get; set; } = new();
        public List<Management> Managements { get; set; } = new();
        public string? Status { get; set; }
        public string Priority { get; set; } = "overdue";
        public int TotalActions { get; set; }
        public int OverdueActions { get; set; }
        public int CriticalActions { get; set; }
        public double CompletionRate { get; set; }
    }

    public class CreateCorrectiveActionViewModel
    {
        [Required]
        [StringLength(2000, MinimumLength = 10)]
        [Display(Name = "Action Description")]
        public string ActionDescription { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Deadline Date")]
        public DateOnly ActionDeadlineDate { get; set; }

        [Required]
        [Display(Name = "Responsible Person")]
        public long RespId { get; set; } // bigint in DB

        [Required]
        [Display(Name = "Related Audit")]
        public long AuditId { get; set; } // bigint in DB

        [Display(Name = "Management")]
        public long? MgmtId { get; set; } // bigint in DB

        [Display(Name = "Initial Status")]
        public string ActionStatus { get; set; } = "Open";
    }

    public class UpdateCorrectiveActionViewModel
    {
        [Required]
        [StringLength(2000, MinimumLength = 10)]
        [Display(Name = "Action Description")]
        public string ActionDescription { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Deadline Date")]
        public DateOnly ActionDeadlineDate { get; set; }

        [Required]
        [Display(Name = "Responsible Person")]
        public long RespId { get; set; } // bigint in DB
    }

    public class UpdateCorrectiveActionStatusViewModel
    {
        [Required]
        [RegularExpression("^(Open|In_Progress|Completed|Extended|Overdue|Rejected)$")]
        [Display(Name = "Action Status")]
        public string ActionStatus { get; set; } = string.Empty;
    }

    public class CorrectiveActionDetailsViewModel
    {
        public long ActionId { get; set; } // bigint in DB
        public string ActionDescription { get; set; } = string.Empty;
        public DateOnly ActionDeadlineDate { get; set; }
        public string ActionStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public long AuditId { get; set; } // bigint in DB
        public string AuditTitle { get; set; } = string.Empty;
        public string PlantName { get; set; } = string.Empty;

        public long RespId { get; set; } // bigint in DB
        public string ResponsiblePersonName { get; set; } = string.Empty;
        public string ResponsiblePersonEmail { get; set; } = string.Empty;

        public long? MgmtId { get; set; } // bigint in DB
        public string ManagementName { get; set; } = string.Empty;
        public string ManagementEmail { get; set; } = string.Empty;

        public bool IsOverdue => ActionDeadlineDate < DateOnly.FromDateTime(DateTime.Now) && ActionStatus != "Completed";
        public bool IsCritical => ActionDeadlineDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(3)) && ActionStatus != "Completed" && ActionStatus != "Rejected";
        public int DaysRemaining => ActionDeadlineDate.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber;
    }

    public class ExtensionRequestViewModel
    {
        [Required]
        public long ActionId { get; set; } // bigint in DB

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Requested New Deadline")]
        public DateOnly RequestedDeadline { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 20)]
        [Display(Name = "Justification for Extension")]
        public string Justification { get; set; } = string.Empty;
    }

    public class CorrectiveActionStatisticsViewModel
    {
        public int TotalActions { get; set; }
        public int PendingActions { get; set; }
        public int InProgressActions { get; set; }
        public int CompletedActions { get; set; }
        public int OverdueActions { get; set; }
        public int ExtendedActions { get; set; }
        public int RejectedActions { get; set; }
        public int EscalatedActions { get; set; }
        public double CompletionRate { get; set; }
        public double OnTimeCompletionRate { get; set; }
        public int AverageCompletionDays { get; set; }
        public Dictionary<string, int> ActionsByStatus { get; set; } = new();
        public Dictionary<string, int> ActionsByPlant { get; set; } = new();
    }
}
