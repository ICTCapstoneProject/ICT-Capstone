using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FSSA.Models
{
    public class UserEditViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string? PasswordHash { get; set; }

        public string ExistingPasswordHash { get; set; }

        [Required]
        public List<int> SelectedRoles { get; set; }

        [Required]
        [Display(Name = "Admin Override Confirmation")]
        public string AdminConfirmation { get; set; }
    }
}