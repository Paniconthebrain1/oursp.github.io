using System.ComponentModel.DataAnnotations;

namespace OurSunday.ViewModel
{
    public class ResetPasswordVM
    {
        public string? id { get; set; }
        public string? Username { get; set; }
        [Required]
        public string? Newpassword { get; set; }
        [Required]
        [Compare(nameof(Newpassword))]
        public string? ConfirmPassword { get; set; }

    }
}
