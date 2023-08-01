using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class UserCredentials
    {
        [Key]
        public required string Login { get; set; }
        public required string Password { get; set; }
        public required User UserData { get; set; }
    }
}
