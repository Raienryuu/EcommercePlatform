using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserCredentials> UserCredentials { get; set; }

        
    }
}
