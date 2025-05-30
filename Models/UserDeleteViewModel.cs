using System.ComponentModel.DataAnnotations;

namespace FSSA.Models
{
    public class UserDeleteViewModel
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        [Display(Name = "Admin Override Confirmation")]
        [Required(ErrorMessage = "Incorrect override key. Try Again.")]
        public string AdminConfirmation { get; set; }
    }
}
