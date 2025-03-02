using BierpongProjectWebApi.Domain.Entities;
using BierpongProjectWebApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BierpongProjectWebApi.Data
{
    public class CustomDbContext : DbContext
    {
        public CustomDbContext()
        {
            
        }
        public CustomDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Friendship table and columns via Fluent API
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.ToTable("Friendships");

                entity.HasKey(f => f.FriendshipId); // Primary key

                // Set up the relationships
                entity.HasOne(f => f.User)
                    .WithMany(u => u.Friendships)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

                entity.HasOne(f => f.FriendUser)
                    .WithMany()
                    .HasForeignKey(f => f.FriendUserId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
            });


            modelBuilder.Entity<User>()
           .HasOne(u => u.UserProfile)
           .WithOne(up => up.User)
           .HasForeignKey<UserProfile>(up => up.UserId)
           .OnDelete(DeleteBehavior.Cascade); // Optional: Delete UserProfile if User is deleted
        }
    }
}
