using HelpDeskProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
         
        
                modelBuilder.Entity<Comment>()
                        .HasOne(c => c.User)
                        .WithMany()
                        .HasForeignKey(c => c.UserID)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                    .HasOne(c => c.Ticket)
                    .WithMany()
                    .HasForeignKey(c => c.TicketID)
                    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                      .HasOne(c => c.CreatedBy)
                      .WithMany()
                      .HasForeignKey(c => c.CreatedById)
                      .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ActivityLog>()
                      .HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserID)
                      .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ErrorLog>()
                    .HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserID)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
