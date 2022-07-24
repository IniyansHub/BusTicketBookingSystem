using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AuthenticationService.Models
{
    public partial class busticketdbContext : DbContext
    {
        public busticketdbContext()
        {
        }

        public busticketdbContext(DbContextOptions<busticketdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Busdatum> Busdata { get; set; } = null!;
        public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; } = null!;
        public virtual DbSet<Ticketdatum> Ticketdata { get; set; } = null!;
        public virtual DbSet<Userdatum> Userdata { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=password;database=busticketdb", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.29-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Busdatum>(entity =>
            {
                entity.HasKey(e => e.BusId)
                    .HasName("PRIMARY");

                entity.ToTable("busdata");

                entity.Property(e => e.BusId).HasColumnName("busId");

                entity.Property(e => e.ArrivalTime)
                    .HasColumnType("datetime")
                    .HasColumnName("arrivalTime");

                entity.Property(e => e.BusName)
                    .HasMaxLength(45)
                    .HasColumnName("busName");

                entity.Property(e => e.BusRoute)
                    .HasMaxLength(100)
                    .HasColumnName("busRoute");

                entity.Property(e => e.BusType)
                    .HasMaxLength(5)
                    .HasColumnName("busType");

                entity.Property(e => e.TicketCount)
                    .HasColumnName("ticketCount")
                    .HasDefaultValueSql("'50'");
            });

            modelBuilder.Entity<Efmigrationshistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__efmigrationshistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ProductVersion).HasMaxLength(32);
            });

            modelBuilder.Entity<Ticketdatum>(entity =>
            {
                entity.HasKey(e => e.TicketId)
                    .HasName("PRIMARY");

                entity.ToTable("ticketdata");

                entity.Property(e => e.TicketId).HasColumnName("ticketId");

                entity.Property(e => e.BusId).HasColumnName("busId");

                entity.Property(e => e.TicketCount).HasColumnName("ticketCount");

                entity.Property(e => e.UserId).HasColumnName("userId");
            });

            modelBuilder.Entity<Userdatum>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("userdata");

                entity.HasIndex(e => e.EmailId, "emailId_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.EmailId)
                    .HasMaxLength(100)
                    .HasColumnName("emailId");

                entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

                entity.Property(e => e.Password)
                    .HasMaxLength(500)
                    .HasColumnName("password");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
