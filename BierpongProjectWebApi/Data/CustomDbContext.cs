using BierpongProjectWebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BierpongProjectWebApi.Data
{
    public class CustomDbContext : DbContext
    {
        public CustomDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
