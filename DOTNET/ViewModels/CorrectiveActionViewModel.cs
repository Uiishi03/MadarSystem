using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.AreaOwnerVMs
{
    public class CorrectiveActionListViewModel
    {
        public AreaOwnerInfo AreaOwner { get; set; }
        public PlantInfo Plant { get; set; }
        public List<CorrectiveActionDto> Actions { get; set; }
        public List<ResponsiblePersonDto> ResponsiblePersons { get; set; }
        public List<AuditDto> Audits { get; set; }
        public ActionStats Stats { get; set; }
        public ActionFilters Filters { get; set; }
    }

    public class CorrectiveActionDto
    {
        public long ActionId { get; set; } 
        public string ActionDescription { get; set; }
        public DateTime ActionDeadlineDate { get; set; }
        public string ActionStatus { get; set; }
        public string Priority { get; set; }
        public string ResponsiblePersonName { get; set; }
        public string AuditTitle { get; set; }
        public int EvidenceCount { get; set; }
        public bool IsOverdue => ActionDeadlineDate < DateTime.Now &&
                                 (ActionStatus == "Open" || ActionStatus == "In_Progress");
        public int DaysUntilDeadline => (ActionDeadlineDate - DateTime.Now).Days;
    }

    public class ResponsiblePersonDto
    {
        public long RespId { get; set; } 
        public string RespFname { get; set; }
        public string RespLname { get; set; }
        public string FullName => $"{RespFname} {RespLname}";
        public string RespEmail { get; set; }
    }

    public class AuditDto
    {
        public long AuditId { get; set; } 
        public string AuditTitle { get; set; }
        public string AuditStatus { get; set; }
    }

    public class ActionStats
    {
        public int Total { get; set; }
        public int Open { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int Overdue { get; set; }
    }

    public class ActionFilters
    {
        public string Status { get; set; }
        public string Priority { get; set; }
        public bool? Overdue { get; set; }
    }

    public class AssignActionViewModel
    {
        [Required(ErrorMessage = "Responsible person is required")]
        [Display(Name = "Responsible Person")]
        public long RespId { get; set; } 

        [Required(ErrorMessage = "Audit is required")]
        [Display(Name = "Audit")]
        public long AuditId { get; set; } 

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        [Display(Name = "Action Description")]
        public string ActionDescription { get; set; }

        [Required(ErrorMessage = "Deadline is required")]
        [Display(Name = "Deadline")]
        [DataType(DataType.Date)]
        public DateOnly ActionDeadlineDate { get; set; } 

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public string Priority { get; set; }
    }

    public class UpdateActionStatusViewModel
    {
        public long ActionId { get; set; } 

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Action Status")]
        public string ActionStatus { get; set; }
    }

    public class ExtendDeadlineViewModel
    {
        public long ActionId { get; set; } 

        [Required(ErrorMessage = "New deadline is required")]
        [Display(Name = "New Deadline")]
        [DataType(DataType.Date)]
        public DateOnly NewDeadline { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(500)]
        [Display(Name = "Extension Reason")]
        public string ExtensionReason { get; set; }
    }
}