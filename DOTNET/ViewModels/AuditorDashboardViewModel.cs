using Madar.Common;
using Madar.Models;
using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.AuditorVMs
{
    // Dashboard ViewModel
    public class AuditorDashboardViewModel
    {
        public Models.Auditor Auditor { get; set; }
        public List<AuditorAllocation> AssignedSchedules { get; set; } = new();
        public List<AuditorAllocation> TodayAudits { get; set; } = new();
        public List<Audit> RecentAudits { get; set; } = new();
        public int TotalAssigned { get; set; }
        public int TodayCount { get; set; }
        public int PendingAudits { get; set; }
    }

    // Execute Audit ViewModel
    public class ExecuteAuditViewModel
    {
        public Audit Audit { get; set; }
        public List<ResponsiblePerson> ResponsiblePersons { get; set; } = new();
    }

    // Attendance ViewModel
    public class AttendanceViewModel
    {
        [Required(ErrorMessage = "Attendance records are required")]
        public List<AttendanceRecordViewModel> Attendances { get; set; } = new();
    }

    public class AttendanceRecordViewModel
    {
        [Required]
        public long AuditorId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeOnly? ArrivalTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeOnly? DepartureTime { get; set; }

        // Validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Constants.ATTENDANCE_STATUSES.Contains(Status))
            {
                yield return new ValidationResult(
                    "Invalid attendance status",
                    new[] { nameof(Status) });
            }

            if (ArrivalTime.HasValue && DepartureTime.HasValue)
            {
                if (DepartureTime <= ArrivalTime)
                {
                    yield return new ValidationResult(
                        "Departure time must be after arrival time",
                        new[] { nameof(DepartureTime) });
                }
            }
        }
    }

    // Evidence ViewModel
    public class EvidenceViewModel
    {
        [Required(ErrorMessage = "Evidence title is required")]
        [StringLength(180, ErrorMessage = "Title cannot exceed 180 characters")]
        [Display(Name = "Evidence Title")]
        public string EvidenceTitle { get; set; }

        [Required(ErrorMessage = "Evidence file is required")]
        [Display(Name = "Evidence File")]
        public IFormFile EvidenceFile { get; set; }

        [Display(Name = "Related Corrective Action")]
        public int? ActionId { get; set; }

        // File validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EvidenceFile != null)
            {
                // Check file size (10MB max)
                if (EvidenceFile.Length > Constants.MAX_FILE_SIZE_BYTES)
                {
                    yield return new ValidationResult(
                        $"File size cannot exceed {Constants.MAX_FILE_SIZE_MB}MB",
                        new[] { nameof(EvidenceFile) });
                }

                // Check file extension
                var extension = Path.GetExtension(EvidenceFile.FileName).ToLower();
                if (!Constants.ALLOWED_FILE_EXTENSIONS.Contains(extension))
                {
                    yield return new ValidationResult(
                        "Invalid file type. Allowed types: " + string.Join(", ", Constants.ALLOWED_FILE_EXTENSIONS),
                        new[] { nameof(EvidenceFile) });
                }
            }
        }
    }

    // Corrective Action ViewModel
    public class CorrectiveActionViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Action description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Action Description")]
        public string ActionDescription { get; set; }

        [Required(ErrorMessage = "Responsible person is required")]
        [Display(Name = "Responsible Person")]
        public long ResponsiblePersonId { get; set; }

        [Required(ErrorMessage = "Deadline date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Deadline Date")]
        public DateTime DeadlineDate { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority Level")]
        public string Priority { get; set; }

        // Validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DeadlineDate <= DateTime.Today)
            {
                yield return new ValidationResult(
                    "Deadline date must be in the future",
                    new[] { nameof(DeadlineDate) });
            }

            if (!Constants.PRIORITY_LEVELS.Contains(Priority))
            {
                yield return new ValidationResult(
                    "Invalid priority level",
                    new[] { nameof(Priority) });
            }
        }
    }

    // Complete Audit ViewModel
    public class CompleteAuditViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Comments are required")]
        [StringLength(2000, ErrorMessage = "Comments cannot exceed 2000 characters")]
        [Display(Name = "Audit Comments")]
        public string Comments { get; set; }

        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
        [Display(Name = "Audit Score")]
        public decimal? Score { get; set; }

        // Validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Score.HasValue)
            {
                if (Score < Constants.MIN_AUDIT_SCORE || Score > Constants.MAX_AUDIT_SCORE)
                {
                    yield return new ValidationResult(
                        $"Score must be between {Constants.MIN_AUDIT_SCORE} and {Constants.MAX_AUDIT_SCORE}",
                        new[] { nameof(Score) });
                }
            }
        }
    }

    // Audit Checklist Step ViewModel
    public class AuditChecklistStepViewModel
    {
        [Required(ErrorMessage = "Step description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string Category { get; set; }

        public bool IsCompliant { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string Notes { get; set; }

        [Range(0, 10, ErrorMessage = "Score must be between 0 and 10")]
        public int? Score { get; set; }
    }

    // Audit Checklist ViewModel
    public class AuditChecklistViewModel
    {
        public long AuditId { get; set; }

        [Required]
        public List<AuditChecklistStepViewModel> Steps { get; set; } = new();

        public decimal CalculateOverallScore()
        {
            if (Steps == null || !Steps.Any())
                return 0;

            var scoredSteps = Steps.Where(s => s.Score.HasValue).ToList();
            if (!scoredSteps.Any())
                return 0;

            return (decimal)scoredSteps.Average(s => s.Score.Value) * 10; // Convert to 0-100 scale
        }
    }

    // Save Notes ViewModel
    public class SaveNotesViewModel
    {
        [Required]
        public long AuditId { get; set; }

        [StringLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters")]
        [Display(Name = "Audit Notes")]
        public string Notes { get; set; }
    }

    // Schedule Filter ViewModel
    public class ScheduleFilterViewModel
    {
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Plant")]
        public long? PlantId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FromDate.HasValue && ToDate.HasValue)
            {
                if (ToDate < FromDate)
                {
                    yield return new ValidationResult(
                        "To Date must be after From Date",
                        new[] { nameof(ToDate) });
                }
            }

            if (!string.IsNullOrEmpty(Status) && !Constants.AUDIT_SCHEDULE_STATUSES.Contains(Status))
            {
                yield return new ValidationResult(
                    "Invalid status value",
                    new[] { nameof(Status) });
            }
        }
    }

    // Audit Statistics ViewModel
    public class AuditStatisticsViewModel
    {
        public int TotalAudits { get; set; }
        public int CompletedAudits { get; set; }
        public int InProgressAudits { get; set; }
        public int ScheduledAudits { get; set; }
        public decimal AverageScore { get; set; }
        public int TotalCorrectiveActions { get; set; }
        public int PendingActions { get; set; }
        public int CompletedActions { get; set; }
        public int OverdueActions { get; set; }

        public decimal CompletionRate => TotalAudits > 0
            ? (decimal)CompletedAudits / TotalAudits * 100
            : 0;

        public decimal ActionCompletionRate => TotalCorrectiveActions > 0
            ? (decimal)CompletedActions / TotalCorrectiveActions * 100
            : 0;
    }

    // Evidence List Item ViewModel
    public class EvidenceListItemViewModel
    {
        public long EvidenceId { get; set; }
        public string EvidenceTitle { get; set; }
        public string EvidenceUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ActionId { get; set; }
        public string ActionDescription { get; set; }
    }

    // Corrective Action List Item ViewModel
    public class CorrectiveActionListItemViewModel
    {
        public long ActionId { get; set; }
        public string ActionDescription { get; set; }
        public string ActionStatus { get; set; }
        public DateOnly DeadlineDate { get; set; }
        public string ResponsiblePersonName { get; set; }
        public bool IsOverdue => DeadlineDate < DateOnly.FromDateTime(DateTime.Today)
            && ActionStatus != Constants.ACTION_STATUS_COMPLETED;
    }
}
