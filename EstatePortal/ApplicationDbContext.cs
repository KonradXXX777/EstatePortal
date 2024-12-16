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

			// Relation between `EmployeeInvitation` and `User`
			modelBuilder.Entity<EmployeeInvitation>()
                .HasOne(ei => ei.Employer)
                .WithMany(u => u.EmployeeInvitations)
                .HasForeignKey(ei => ei.EmployerId);

			// Relation between `Announcement` and `AnnouncementFeature`
			modelBuilder.Entity<Announcement>()
				.HasOne(a => a.Features)
				.WithOne(f => f.Announcement)
				.HasForeignKey<AnnouncementFeature>(f => f.AnnouncementId)
				.OnDelete(DeleteBehavior.Cascade);

			// Relation between `Announcement` and `AnnouncementPhoto`
			modelBuilder.Entity<AnnouncementPhoto>()
				.HasOne(ap => ap.Announcement)
				.WithMany(a => a.Photos)
				.HasForeignKey(ap => ap.AnnouncementId)
				.OnDelete(DeleteBehavior.Cascade);
		}
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<EmployeeInvitation> EmployeeInvitations { get; set; }=default!;
		public DbSet<Announcement> Announcements { get; set; } = default!;
		public DbSet<AnnouncementFeature> AnnouncementFeatures { get; set; } = default!;
		public DbSet<AnnouncementPhoto> AnnouncementPhotos { get; set; } = default!;
        public DbSet<Chat> Chats { get; set; } = default!;
        public DbSet<Message> Messages { get; set; } = default!;

    }
}