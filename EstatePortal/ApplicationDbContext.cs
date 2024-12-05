using Microsoft.EntityFrameworkCore;
using EstatePortal.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EstatePortal
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EmployeeInvitation>()
                .HasOne(ei => ei.Employer)
                .WithMany(u => u.EmployeeInvitations)
                .HasForeignKey(ei => ei.EmployerId);
        }
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<EmployeeInvitation> EmployeeInvitations { get; set; }=default!;
    }
}

