using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Models.Login
{
    public class ForgetPasswordModel
    {
        [Required]
        public string email { get; set; }

    }
}
