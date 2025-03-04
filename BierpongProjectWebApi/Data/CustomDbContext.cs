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
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<MatchHistory> MatchHistories { get; set; }

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

            modelBuilder.Entity<Game>(entity =>
            {
                // Define the primary key
                entity.HasKey(g => g.GameId);

                // Define the foreign keys for player1 and player2
                entity.HasOne(g => g.Player1)
                    .WithMany()
                    .HasForeignKey(g => g.Player1Id)
                    .OnDelete(DeleteBehavior.NoAction);  // No cascading delete, sets Player1Id to NULL if Player1 is deleted

                entity.HasOne(g => g.Player2)
                    .WithMany()
                    .HasForeignKey(g => g.Player2Id)
                    .OnDelete(DeleteBehavior.NoAction);  // No cascading delete, sets Player2Id to NULL if Player2 is deleted

                // Optional: configure other properties like the scoreline
                entity.Property(g => g.Scoreline)
                    .HasMaxLength(1000);  // Example of setting max length for a string
            });

            modelBuilder.Entity<User>()
           .HasOne(u => u.UserProfile)
           .WithOne(up => up.User)
           .HasForeignKey<UserProfile>(up => up.UserId)
           .OnDelete(DeleteBehavior.Cascade); // Optional: Delete UserProfile if User is deleted
        }
    }
}
