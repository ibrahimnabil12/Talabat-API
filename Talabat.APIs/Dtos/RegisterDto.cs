using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
    public class RegisterDto // this is for sending the data 
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]

        public string Password { get; set; }
    }
}
