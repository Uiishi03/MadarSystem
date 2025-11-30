using Madar.Models;
using Microsoft.EntityFrameworkCore;

namespace Madar.Data
{
    /// <summary>
    /// Partial class to extend the scaffolded DbContext with relationship configurations
    /// This file configures all foreign key relationships and navigation properties
    /// </summary>
    public partial class MadarDbContext
    {
        /// <summary>
        /// This method is called after the scaffolded OnModelCreating
        /// </summary>
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            ConfigureRelationships(modelBuilder);
        }

        /// <summary>
        /// Configures all entity relationships matching the database foreign keys
        /// </summary>
        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // ====================================
            // User Relationships (One-to-One)
            // CASCADE DELETE - When User is deleted, role entity is deleted
            // ====================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.Auditor)
                    .WithOne(a => a.User)
                    .HasForeignKey<Auditor>(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.Management)
                    .WithOne(m => m.User)
                    .HasForeignKey<Management>(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.AreaOwner)
                    .WithOne(ao => ao.User)
                    .HasForeignKey<AreaOwner>(ao => ao.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.ResponsiblePerson)
                    .WithOne(rp => rp.User)
                    .HasForeignKey<ResponsiblePerson>(rp => rp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================================
            // AreaOwner Relationships
            // RESTRICT - Prevent deletion if dependent records exist
            // ====================================
            modelBuilder.Entity<AreaOwner>(entity =>
            {
                entity.HasMany(ao => ao.Plants)
                    .WithOne(p => p.AreaOwner)
                    .HasForeignKey(p => p.AoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(ao => ao.ResponsiblePeople)
                    .WithOne(rp => rp.AreaOwner)
                    .HasForeignKey(rp => rp.AoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(ao => ao.AuditHistories)
                    .WithOne(ah => ah.AreaOwner)
                    .HasForeignKey(ah => ah.AoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================================
            // Management Relationships
            // RESTRICT - Prevent deletion if dependent records exist
            // ====================================
            modelBuilder.Entity<Management>(entity =>
            {
                entity.HasMany(m => m.Plants)
                    .WithOne(p => p.Management)
                    .HasForeignKey(p => p.MgmtId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(m => m.CorrectiveActions)
                    .WithOne(ca => ca.Management)
                    .HasForeignKey(ca => ca.MgmtId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(m => m.AuditHistories)
                    .WithOne(ah => ah.Management)
                    .HasForeignKey(ah => ah.MgmtId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================================
            // ResponsiblePerson Relationships
            // ====================================
            modelBuilder.Entity<ResponsiblePerson>(entity =>
            {
                entity.HasOne(rp => rp.AreaOwner)
                    .WithMany(ao => ao.ResponsiblePeople)
                    .HasForeignKey(rp => rp.AoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(rp => rp.CorrectiveActions)
                    .WithOne(ca => ca.ResponsiblePerson)
                    .HasForeignKey(ca => ca.RespId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(rp => rp.Attendances)
                    .WithOne(aa => aa.ResponsiblePerson)
                    .HasForeignKey(aa => aa.RespId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================================
            // Auditor Relationships
            // ====================================
            modelBuilder.Entity<Auditor>(entity =>
            {
                entity.HasMany(a => a.Allocations)
                    .WithOne(aa => aa.Auditor)
                    .HasForeignKey(aa => aa.AudId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(a => a.Attendances)
                    .WithOne(aa => aa.Auditor)
                    .HasForeignKey(aa => aa.AudId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================================
            // Plant Relationships
            // ====================================
            modelBuilder.Entity<Plant>(entity =>
            {
                entity.HasOne(p => p.Management)
                    .WithMany(m => m.Plants)
                    .HasForeignKey(p => p.MgmtId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.AreaOwner)
                    .WithMany(ao => ao.Plants)
                    .HasForeignKey(p => p.AoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // CASCADE - Equipment is deleted when Plant is deleted
                entity.HasMany(p => p.Equipment)
                    .WithOne(e => e.Plant)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.AuditSchedules)
                    .WithOne(a => a.Plant)
                    .HasForeignKey(a => a.PlantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================================
            // Equipment Relationships
            // ====================================
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasOne(e => e.Plant)
                    .WithMany(p => p.Equipment)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.AuditEquipments)
                    .WithOne(ae => ae.Equipment)
                    .HasForeignKey(ae => ae.EquipId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================================
            // AuditSchedule Relationships
            // ====================================
            modelBuilder.Entity<AuditSchedule>(entity =>
            {
                entity.HasOne(a => a.Plant)
                    .WithMany(p => p.AuditSchedules)
                    .HasForeignKey(a => a.PlantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(a => a.Audits)
                    .WithOne(a => a.Schedule)
                    .HasForeignKey(a => a.AudSchId)
                    .OnDelete(DeleteBehavior.Restrict);

                // CASCADE - Allocations deleted when schedule is deleted
                entity.HasMany(a => a.Allocations)
                    .WithOne(aa => aa.Schedule)
                    .HasForeignKey(aa => aa.AudSchId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================================
            // AuditorAllocation Relationships
            // ====================================
            modelBuilder.Entity<AuditorAllocation>(entity =>
            {
                entity.HasOne(aa => aa.Auditor)
                    .WithMany(a => a.Allocations)
                    .HasForeignKey(aa => aa.AudId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(aa => aa.Schedule)
                    .WithMany(a => a.Allocations)
                    .HasForeignKey(aa => aa.AudSchId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================================
            // Audit Relationships
            // ====================================
            modelBuilder.Entity<Audit>(entity =>
            {
                entity.HasOne(a => a.Schedule)
                    .WithMany(s => s.Audits)
                    .HasForeignKey(a => a.AudSchId)
                    .OnDelete(DeleteBehavior.Restrict);

                // CASCADE - Evidence deleted when Audit is deleted
                entity.HasMany(a => a.Evidences)
                    .WithOne(e => e.Audit)
                    .HasForeignKey(e => e.AuditId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(a => a.CorrectiveActions)
                    .WithOne(ca => ca.Audit)
                    .HasForeignKey(ca => ca.AuditId)
                    .OnDelete(DeleteBehavior.Restrict);

                // CASCADE - Attendance deleted when Audit is deleted
                entity.HasMany(a => a.Attendances)
                    .WithOne(aa => aa.Audit)
                    .HasForeignKey(aa => aa.AuditId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(a => a.Histories)
                    .WithOne(ah => ah.Audit)
                    .HasForeignKey(ah => ah.AuditId)
                    .OnDelete(DeleteBehavior.Restrict);

                // CASCADE - AuditEquipments deleted when Audit is deleted
                entity.HasMany(a => a.AuditEquipments)
                    .WithOne(ae => ae.Audit)
                    .HasForeignKey(ae => ae.AuditId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================================
            // AuditEquipment Relationships (Junction Table)
            // ====================================
            modelBuilder.Entity<AuditEquipment>(entity =>
            {
                entity.HasOne(ae => ae.Audit)
                    .WithMany(a => a.AuditEquipments)
                    .HasForeignKey(ae => ae.AuditId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ae => ae.Equipment)
                    .WithMany(e => e.AuditEquipments)
                    .HasForeignKey(ae => ae.EquipId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================================
            // AuditAttendance Relationships
            // ====================================
            modelBuilder.Entity<AuditAttendance>(entity =>
            {
                entity.HasOne(aa => aa.Audit)
                    .WithMany(a => a.Attendances)
                    .HasForeignKey(aa => aa.AuditId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(aa => aa.Auditor)
                    .WithMany(a => a.Attendances)
                    .HasForeignKey(aa => aa.AudId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(aa => aa.ResponsiblePerson)
                    .WithMany(rp => rp.Attendances)
                    .HasForeignKey(aa => aa.RespId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================================
            // AuditHistory Relationships
            // ====================================
            modelBuilder.Entity<AuditHistory>(entity =>
            {
                entity.HasOne(ah => ah.Audit)
                    .WithMany(a => a.Histories)
                    .HasForeignKey(ah => ah.AuditId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ah => ah.AreaOwner)
                    .WithMany(ao => ao.AuditHistories)
                    .HasForeignKey(ah => ah.AoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ah => ah.Management)
                    .WithMany(m => m.AuditHistories)
                    .HasForeignKey(ah => ah.MgmtId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================================
            // CorrectiveAction Relationships
            // ====================================
            modelBuilder.Entity<CorrectiveAction>(entity =>
            {
                entity.HasOne(ca => ca.ResponsiblePerson)
                    .WithMany(rp => rp.CorrectiveActions)
                    .HasForeignKey(ca => ca.RespId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ca => ca.Audit)
                    .WithMany(a => a.CorrectiveActions)
                    .HasForeignKey(ca => ca.AuditId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ca => ca.Management)
                    .WithMany(m => m.CorrectiveActions)
                    .HasForeignKey(ca => ca.MgmtId)
                    .OnDelete(DeleteBehavior.Restrict);

                // SET NULL - Evidence remains but link to action is removed
                entity.HasMany(ca => ca.Evidences)
                    .WithOne(e => e.CorrectiveAction)
                    .HasForeignKey(e => e.ActionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ====================================
            // Evidence Relationships
            // ====================================
            modelBuilder.Entity<Evidence>(entity =>
            {
                entity.HasOne(e => e.Audit)
                    .WithMany(a => a.Evidences)
                    .HasForeignKey(e => e.AuditId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CorrectiveAction)
                    .WithMany(ca => ca.Evidences)
                    .HasForeignKey(e => e.ActionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}