using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AgroPulse.Data.EF;

public partial class AgroPulseDBContext : DbContext
{
    public AgroPulseDBContext(DbContextOptions<AgroPulseDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AgroAlert> AgroAlerts { get; set; }

    public virtual DbSet<FieldLot> FieldLots { get; set; }

    public virtual DbSet<RainRecord> RainRecords { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AgroAlert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("agro_alerts");

            entity.HasIndex(e => new { e.FieldLotId, e.IsRead }, "idx_alerts_lot_unread");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlertType)
                .HasComment("1:HeladaAgronomica, 2:VentanaPulverizacion, 3:EstresHidrico, 4:SueloFrioSiembra")
                .HasColumnName("alert_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FieldLotId).HasColumnName("field_lot_id");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Message)
                .HasMaxLength(300)
                .HasColumnName("message");
            entity.Property(e => e.Severity)
                .HasComment("1:Info, 2:Warning, 3:Critical")
                .HasColumnName("severity");
            entity.Property(e => e.TargetDate)
                .HasColumnType("datetime")
                .HasColumnName("target_date");

            entity.HasOne(d => d.FieldLot).WithMany(p => p.AgroAlerts)
                .HasForeignKey(d => d.FieldLotId)
                .HasConstraintName("fk_agro_alerts_lots");
        });

        modelBuilder.Entity<FieldLot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("field_lots");

            entity.HasIndex(e => e.UserId, "fk_field_lots_users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CropStage)
                .HasComment("0:PreSiembra, 1:Siembra, 2:Vegetativo, 3:FloracionCritica, 4:LlenadoGrano, 5:Cosecha")
                .HasColumnName("crop_stage");
            entity.Property(e => e.CropType)
                .HasComment("0:Barbecho, 1:Soja, 2:Maiz, 3:Trigo, 4:Girasol, 5:Pastura")
                .HasColumnName("crop_type");
            entity.Property(e => e.Hectares)
                .HasPrecision(10, 2)
                .HasColumnName("hectares");
            entity.Property(e => e.Latitude)
                .HasPrecision(9, 6)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(9, 6)
                .HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.SowingDate).HasColumnName("sowing_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.FieldLots)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_field_lots_users");
        });

        modelBuilder.Entity<RainRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("rain_records");

            entity.HasIndex(e => new { e.FieldLotId, e.RecordDate }, "idx_rain_lot_date");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FieldLotId).HasColumnName("field_lot_id");
            entity.Property(e => e.Millimeters)
                .HasPrecision(6, 2)
                .HasColumnName("millimeters");
            entity.Property(e => e.Notes)
                .HasMaxLength(250)
                .HasColumnName("notes");
            entity.Property(e => e.RecordDate).HasColumnName("record_date");
            entity.Property(e => e.Source)
                .HasComment("1:ApiSatelital, 2:ManualPluviometro")
                .HasColumnName("source");

            entity.HasOne(d => d.FieldLot).WithMany(p => p.RainRecords)
                .HasForeignKey(d => d.FieldLotId)
                .HasConstraintName("fk_rain_records_lots");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
