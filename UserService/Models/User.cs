using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class User
    {
        public required string Name { get; set; }
        public required string Lastname { get; set; }
        [Key]
        public required string Email { get; set; }
        public required string PhonePrefix { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string ZIPCode { get; set; }
        public required string Country { get; set; }

        public static User GenerateFrom(NewUser user)
        {
            return new User()
            {
                Name = user.Name,
                Lastname = user.Lastname,
                Email = user.Email,
                PhonePrefix = user.PhonePrefix,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                ZIPCode = user.ZIPCode,
                Country = user.Country,
            };
        }
    }
}
               