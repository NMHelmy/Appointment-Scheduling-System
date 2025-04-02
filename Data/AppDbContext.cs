using AppointmentScheduling.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduling.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _config;

        // Constructor for dependency injection (used at runtime)
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Service> Services { get; set; }

        // Fallback configuration if context isn't configured via DI
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    _config.GetConnectionString("DefaultConnection"),
                    options => options.EnableRetryOnFailure()
                );
            }
        }

        // Configure database model relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(50);

                entity.HasIndex(u => u.Email)
                .IsUnique();

                entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

                entity.Property<string>(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);
            });

            modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent user deletion if appointments exist

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasIndex(s => s.Name).IsUnique();
            });

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
