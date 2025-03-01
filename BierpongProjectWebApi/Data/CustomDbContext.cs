using BierpongProjectWebApi.Domain.Entities;
using BierpongProjectWebApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BierpongProjectWebApi.Data
{
    public class CustomDbContext : DbContext
    {
        public CustomDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
