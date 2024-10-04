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
        public DbSet<User> Users { get; set; } = default!;
    }
}

