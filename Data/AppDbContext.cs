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
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }

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
            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

                entity.HasIndex(u => u.Email)
                .IsUnique();

                entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

                entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

                entity.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Client");

                entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

                // Relationships
                entity.HasMany(u => u.Appointments)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // Appointment Configuration
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.AppointmentId);

                entity.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(100);

                entity.Property(a => a.Description)
                .HasMaxLength(500);

                entity.Property(a => a.AppointmentDate)
                .IsRequired()
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

                // Relationships
                entity.HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(a => a.Payments)
                .WithOne(p => p.Appointment)
                .HasForeignKey(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(a => a.StatusHistory)
                .WithOne(s => s.Appointment)
                .HasForeignKey(s => s.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Review)
                .WithOne(r => r.Appointment)
                .HasForeignKey<Review>(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(a => new { a.UserId, a.AppointmentDate });
            });

            // Service Configuration
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasIndex(s => s.Name)
                .IsUnique();

                entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

                entity.Property(s => s.Description)
                .HasMaxLength(500);
            });

            // Payment Configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(p => p.Amount)
                .HasPrecision(10, 2);

                entity.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

                entity.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

                entity.Property(p => p.TransactionId)
                .HasMaxLength(100);

                // Indexes
                entity.HasIndex(p => new { p.AppointmentId, p.Status });
                entity.HasIndex(p => p.TransactionId)
                .IsUnique()
                .HasFilter("[TransactionId] IS NOT NULL");
            });

            // Notification Configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

                entity.Property(n => n.NotificationType)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

                entity.Property(n => n.Status)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

                // Indexes
                entity.HasIndex(n => n.Status);
            });

            // AppointmentStatus Configuration
            modelBuilder.Entity<AppointmentStatus>(entity =>
            {
                entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

                entity.HasOne(s => s.Appointment)
                .WithMany(a => a.StatusHistory)
                .HasForeignKey(s => s.AppointmentId)
                .IsRequired(false)  // This makes the FK optional
                .OnDelete(DeleteBehavior.Cascade);

                entity.Property(s => s.Description)
                .HasMaxLength(200);

                // Indexes
                entity.HasIndex(s => s.Name)
                .IsUnique();

                // Seed initial AppointmentStatuses
                entity.HasData(
                new
                {
                    AppointmentStatusId = 1,
                    Name = "Booked",
                    Description = "Appointment is scheduled"
                },
                new
                {
                    AppointmentStatusId = 2,
                    Name = "Completed",
                    Description = "Appointment is fulfilled"
                },
                new
                {
                    AppointmentStatusId = 3,
                    Name = "Cancelled",
                    Description = "Appointment is cancelled"
                }
            );
            });

            // Review Configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(r => r.Rating)
                .IsRequired();

                entity.Property(r => r.Comment)
                .HasMaxLength(500);

                entity.Property(r => r.ReviewDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
