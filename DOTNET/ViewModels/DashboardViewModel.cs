using Madar.Models;
using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.ManagementVMs
{
    /// <summary>
    /// Dashboard view model
    /// </summary>
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalPlants { get; set; }
        public int PendingAudits { get; set; }
        public int OverdueActions { get; set; }
        public double CompletionRate { get; set; }
        public double ActionCompletionRate { get; set; }
        public int ActionsDueThisWeek { get; set; }
        public int CriticalIssues { get; set; }
        public List<User> RecentUsers { get; set; } = new();
        public List<AuditSchedule> UpcomingAudits { get; set; } = new();
        public int TotalAudits { get; set; }
        public int CompletedAudits { get; set; }
        public int TotalActions { get; set; }
        public int CompletedActions { get; set; }
    }

    /// <summary>
    /// Profile view model
    /// </summary>
    public class ProfileViewModel
    {
        public long UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Update profile view model
    /// </summary>
    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(150, MinimumLength = 2)]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [StringLength(190)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Notification view model
    /// </summary>
    public class NotificationViewModel
    {
        public string Type { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}
