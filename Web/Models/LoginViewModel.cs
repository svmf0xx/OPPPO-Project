using System.ComponentModel.DataAnnotations;
namespace Shcheduler.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

    }
}