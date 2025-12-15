using Madar.Models;
using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.ManagementVMs.PerformanceReportVMs
{
    public class PerformanceReportIndexViewModel
    {
        public List<AuditHistory> AuditHistories { get; set; } = new();
        public List<Audit> Audits { get; set; } = new();
        public List<Plant> Plants { get; set; } = new();
        public List<Auditor> Auditors { get; set; } = new();
        public long? PlantId { get; set; } // bigint in DB
        public long? AuditorId { get; set; } // bigint in DB
        public string Period { get; set; } = "monthly";
        public double ComplianceRate { get; set; }
        public double AverageScore { get; set; }
        public double CompletionRate { get; set; }
        public int OpenActions { get; set; }
        public int OverdueActions { get; set; }
        public List<PlantPerformanceViewModel> PlantPerformance { get; set; } = new();
        public List<AuditorPerformanceViewModel> AuditorPerformance { get; set; } = new();
        public List<TimelineDataViewModel> TimelineData { get; set; } = new();
    }

    public class PlantPerformanceViewModel
    {
        public Plant Plant { get; set; } = null!;
        public int AuditCount { get; set; }
        public double AverageScore { get; set; }
        public int EquipmentCount { get; set; }
        public double ComplianceRate { get; set; }

        public string PerformanceLevel
        {
            get
            {
                if (AverageScore >= 90) return "Excellent";
                if (AverageScore >= 75) return "Good";
                if (AverageScore >= 60) return "Satisfactory";
                if (AverageScore >= 50) return "Needs Improvement";
                return "Poor";
            }
        }

        public string PerformanceColor
        {
            get
            {
                if (AverageScore >= 90) return "success";
                if (AverageScore >= 75) return "primary";
                if (AverageScore >= 60) return "info";
                if (AverageScore >= 50) return "warning";
                return "danger";
            }
        }
    }

    public class AuditorPerformanceViewModel
    {
        public Auditor Auditor { get; set; } = null!;
        public int AuditCount { get; set; }
        public double AverageScore { get; set; }
        public double CompletionRate { get; set; }

        public string FullName => $"{Auditor.AudFname} {Auditor.AudLname}".Trim();

        public string PerformanceLevel
        {
            get
            {
                if (AverageScore >= 90 && CompletionRate >= 90) return "Outstanding";
                if (AverageScore >= 80 && CompletionRate >= 80) return "Excellent";
                if (AverageScore >= 70 && CompletionRate >= 70) return "Good";
                if (AverageScore >= 60 && CompletionRate >= 60) return "Satisfactory";
                return "Needs Improvement";
            }
        }

        public string PerformanceColor
        {
            get
            {
                if (AverageScore >= 90 && CompletionRate >= 90) return "success";
                if (AverageScore >= 80 && CompletionRate >= 80) return "primary";
                if (AverageScore >= 70 && CompletionRate >= 70) return "info";
                if (AverageScore >= 60 && CompletionRate >= 60) return "warning";
                return "danger";
            }
        }
    }

    public class TimelineDataViewModel
    {
        public string Period { get; set; } = string.Empty;
        public double Score { get; set; }
        public int Count { get; set; }
    }

    public class GenerateReportViewModel
    {
        [Required]
        [RegularExpression("^(monthly|quarterly|yearly)$")]
        [Display(Name = "Report Period")]
        public string Period { get; set; } = "monthly";

        [Display(Name = "Plant")]
        public long? PlantId { get; set; } // bigint in DB

        [Display(Name = "Auditor")]
        public long? AuditorId { get; set; } // bigint in DB

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Report Format")]
        [Required]
        [RegularExpression("^(PDF|Excel|CSV)$")]
        public string Format { get; set; } = "PDF";

        [Display(Name = "Include Charts")]
        public bool IncludeCharts { get; set; } = true;

        [Display(Name = "Include Detailed Breakdown")]
        public bool IncludeDetailedBreakdown { get; set; } = true;
    }
}
