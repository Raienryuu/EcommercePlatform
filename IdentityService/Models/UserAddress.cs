using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityService.Models {
    public class UserAddress : IUserData {
        [Key]
        public int Id {get; set;}
        public string? Name { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public string? PhonePrefix { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ZIPCode { get; set; }
        public string? Country { get; set; }
        public IdentityUser? User { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }

        public static UserAddress CreateFrom(NewUser user, IdentityUser account)
        {
            UserAddress userAddress = new()
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
                UserId = account.Id
            };
            return userAddress;
        }
    }
}
