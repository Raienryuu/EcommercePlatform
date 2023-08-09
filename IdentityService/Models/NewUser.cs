namespace IdentityService.Models
{
    public class NewUser
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }
        public required string PhonePrefix { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string ZIPCode { get; set; }
        public required string Country { get; set; }
    }
}
