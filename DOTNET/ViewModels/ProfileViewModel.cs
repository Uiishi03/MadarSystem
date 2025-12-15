using System.ComponentModel.DataAnnotations;

namespace Madar.ViewModels.AreaOwnerVMs
{
    public class ProfileViewModel
    {
        public AreaOwnerProfileDto AreaOwner { get; set; }
    }

    public class AreaOwnerProfileDto
    {
        public long AoId { get; set; } 
        public string AoFname { get; set; }
        public string AoLname { get; set; }
        public string AoEmail { get; set; }
        public string AoExtension { get; set; }
        public string AoRole { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string AoFname { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string AoLname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(190)]
        [Display(Name = "Email")]
        public string AoEmail { get; set; }

        [StringLength(20)]
        [Display(Name = "Extension")]
        public string AoExtension { get; set; }
    }

    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm New Password")]
        public string NewPasswordConfirmation { get; set; }
    }
}