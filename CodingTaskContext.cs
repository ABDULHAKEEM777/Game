using Cleverbit.CodingTask.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Cleverbit.CodingTask.Data
{
    public class CodingTaskContext : DbContext
    {
        public CodingTaskContext(DbContextOptions<CodingTaskContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Match> Matches { get; set; }

        public DbSet<UserMatch> UserMatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(nameof(User));
            modelBuilder.Entity<Match>().ToTable(nameof(Match));
            modelBuilder.Entity<Match>()
                .HasOne(um => um.User)
                .WithMany()
                .HasForeignKey(um => um.WinnerUserId);                
            modelBuilder.Entity<UserMatch>()
                .HasKey(um => um.Id); 
            modelBuilder.Entity<UserMatch>()
                .HasOne(um => um.User) 
                .WithMany()
                .HasForeignKey(um => um.UserId) 
                .IsRequired(); 
           modelBuilder.Entity<UserMatch>().ToTable(nameof(UserMatch));
        }
    }
}
