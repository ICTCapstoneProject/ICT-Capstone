using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FSSA.Models
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "At least one role must be selected.")]
        [MinLength(1, ErrorMessage = "Please select at least one role.")]
        public List<int> SelectedRoles { get; set; } = new();
    }

}
