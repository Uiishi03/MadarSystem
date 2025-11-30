using System;
using System.Collections.Generic;
using Madar.Models;
using Microsoft.EntityFrameworkCore;

namespace Madar.Data;

public partial class MadarDbContext : DbContext
{
    public MadarDbContext()
    {
    }

    public MadarDbContext(DbContextOptions<MadarDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AreaOwner> AreaOwners { get; set; }

    public virtual DbSet<Audit> Audits { get; set; }

    public virtual DbSet<AuditAttendance> AuditAttendances { get; set; }

    public virtual DbSet<AuditEquipment> AuditEquipments { get; set; }

    public virtual DbSet<AuditHistory> AuditHistories { get; set; }

    public virtual DbSet<AuditSchedule> AuditSchedules { get; set; }

    public virtual DbSet<Auditor> Auditors { get; set; }

    public virtual DbSet<AuditorAllocation> AuditorAllocations { get; set; }

    public virtual DbSet<CorrectiveAction> CorrectiveActions { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<Evidence> Evidences { get; set; }

    public virtual DbSet<Management> Managements { get; set; }

    public virtual DbSet<Plant> Plants { get; set; }

    public virtual DbSet<ResponsiblePerson> ResponsiblePeople { get; set; }

    public virtual DbSet<User> Users { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AreaOwner>(entity =>
        {
            entity.HasKey(e => e.AoId).HasName("PK__AreaOwne__4EB9FCAC4C7EC606");

            entity.ToTable("AreaOwner");

            entity.HasIndex(e => e.AoEmail, "UQ__AreaOwne__A6D297131E849D03").IsUnique();

            entity.Property(e => e.AoId).HasColumnName("AO_ID");
            entity.Property(e => e.AoEmail)
                .HasMaxLength(190)
                .HasColumnName("AO_EMAIL");
            entity.Property(e => e.AoExtension)
                .HasMaxLength(20)
                .HasColumnName("AO_EXTENSION");
            entity.Property(e => e.AoFname)
                .HasMaxLength(100)
                .HasColumnName("AO_FNAME");
            entity.Property(e => e.AoLname)
                .HasMaxLength(100)
                .HasColumnName("AO_LNAME");
            entity.Property(e => e.AoPassword)
                .HasMaxLength(255)
                .HasColumnName("AO_PASSWORD");
            entity.Property(e => e.AoRole)
                .HasMaxLength(80)
                .HasColumnName("AO_ROLE");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__Audit__CDFE7087AE6C8155");

            entity.ToTable("Audit");

            entity.Property(e => e.AuditId).HasColumnName("AUDIT_ID");
            entity.Property(e => e.AudSchId).HasColumnName("AUD_SCH_ID");
            entity.Property(e => e.AuditChecklistSteps).HasColumnName("AUDIT_CHECKLIST_STEPS");
            entity.Property(e => e.AuditDescription).HasColumnName("AUDIT_DESCRIPTION");
            entity.Property(e => e.AuditStatus)
                .HasMaxLength(30)
                .HasColumnName("AUDIT_STATUS");
            entity.Property(e => e.AuditTitle)
                .HasMaxLength(180)
                .HasColumnName("AUDIT_TITLE");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<AuditAttendance>(entity =>
        {
            entity.HasKey(e => e.AudAttendId).HasName("PK__AuditAtt__79B2DB4377ADA31C");

            entity.ToTable("AuditAttendance");

            entity.Property(e => e.AudAttendId).HasColumnName("AUD_ATTEND_ID");
            entity.Property(e => e.AudArrivalTime)
                .HasPrecision(0)
                .HasColumnName("AUD_ARRIVAL_TIME");
            entity.Property(e => e.AudAttendDate).HasColumnName("AUD_ATTEND_DATE");
            entity.Property(e => e.AudAttendStatus)
                .HasMaxLength(30)
                .HasColumnName("AUD_ATTEND_STATUS");
            entity.Property(e => e.AudDepartTime)
                .HasPrecision(0)
                .HasColumnName("AUD_DEPART_TIME");
            entity.Property(e => e.AudId).HasColumnName("AUD_ID");
            entity.Property(e => e.AuditId).HasColumnName("AUDIT_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.RespId).HasColumnName("RESP_ID");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<AuditEquipment>(entity =>
        {
            entity.HasKey(e => new { e.AuditId, e.EquipId }).HasName("PK__AuditEqu__E5372ADC4BFF3FE1");

            entity.ToTable("AuditEquipment");

            entity.Property(e => e.AuditId).HasColumnName("AUDIT_ID");
            entity.Property(e => e.EquipId).HasColumnName("EQUIP_ID");
            entity.Property(e => e.AeDate).HasColumnName("AE_DATE");
            entity.Property(e => e.AeTime)
                .HasPrecision(0)
                .HasColumnName("AE_TIME");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<AuditHistory>(entity =>
        {
            entity.HasKey(e => e.AudHistoryId).HasName("PK__AuditHis__BDCC553EA6995BFE");

            entity.ToTable("AuditHistory");

            entity.Property(e => e.AudHistoryId).HasColumnName("AUD_HISTORY_ID");
            entity.Property(e => e.AoId).HasColumnName("AO_ID");
            entity.Property(e => e.AudHistoryComments).HasColumnName("AUD_HISTORY_COMMENTS");
            entity.Property(e => e.AudHistoryEscalationLevel)
                .HasMaxLength(50)
                .HasColumnName("AUD_HISTORY_ESCALATION_LEVEL");
            entity.Property(e => e.AudHistoryScore)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("AUD_HISTORY_SCORE");
            entity.Property(e => e.AudHistoryStatus)
                .HasMaxLength(30)
                .HasColumnName("AUD_HISTORY_STATUS");
            entity.Property(e => e.AudHistoryTitle)
                .HasMaxLength(180)
                .HasColumnName("AUD_HISTORY_TITLE");
            entity.Property(e => e.AuditId).HasColumnName("AUDIT_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.MgmtId).HasColumnName("MGMT_ID");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<AuditSchedule>(entity =>
        {
            entity.HasKey(e => e.AudSchId).HasName("PK__AuditSch__1F3C45B0C8BB77D6");

            entity.ToTable("AuditSchedule");

            entity.Property(e => e.AudSchId).HasColumnName("AUD_SCH_ID");
            entity.Property(e => e.AudSchDate).HasColumnName("AUD_SCH_DATE");
            entity.Property(e => e.AudSchDuration).HasColumnName("AUD_SCH_DURATION");
            entity.Property(e => e.AudSchStatus)
                .HasMaxLength(30)
                .HasColumnName("AUD_SCH_STATUS");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.PlantId).HasColumnName("PLANT_ID");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<Auditor>(entity =>
        {
            entity.HasKey(e => e.AudId).HasName("PK__Auditor__C12C9F2E31AA1481");

            entity.ToTable("Auditor");

            entity.HasIndex(e => e.AudEmail, "UQ__Auditor__B27815670E12B33A").IsUnique();

            entity.Property(e => e.AudId)
                .ValueGeneratedNever()
                .HasColumnName("AUD_ID");
            entity.Property(e => e.AudEmail)
                .HasMaxLength(190)
                .HasColumnName("AUD_EMAIL");
            entity.Property(e => e.AudExtension)
                .HasMaxLength(20)
                .HasColumnName("AUD_EXTENSION");
            entity.Property(e => e.AudFname)
                .HasMaxLength(100)
                .HasColumnName("AUD_FNAME");
            entity.Property(e => e.AudLname)
                .HasMaxLength(100)
                .HasColumnName("AUD_LNAME");
            entity.Property(e => e.AudPassword)
                .HasMaxLength(255)
                .HasColumnName("AUD_PASSWORD");
            entity.Property(e => e.AudRole)
                .HasMaxLength(80)
                .HasColumnName("AUD_ROLE");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
        });

        modelBuilder.Entity<AuditorAllocation>(entity =>
        {
            entity.HasKey(e => new { e.AudId, e.AudSchId }).HasName("PK__AuditorA__D0DF5B75F00DD9D0");

            entity.ToTable("AuditorAllocation");

            entity.Property(e => e.AudId).HasColumnName("AUD_ID");
            entity.Property(e => e.AudSchId).HasColumnName("AUD_SCH_ID");
            entity.Property(e => e.AssignedDate).HasColumnName("ASSIGNED_DATE");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.RoleType)
                .HasMaxLength(40)
                .HasColumnName("ROLE_TYPE");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<CorrectiveAction>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__Correcti__A7EC837116B6787E");

            entity.ToTable("CorrectiveAction");

            entity.Property(e => e.ActionId).HasColumnName("ACTION_ID");
            entity.Property(e => e.ActionDeadlineDate).HasColumnName("ACTION_DEADLINE_DATE");
            entity.Property(e => e.ActionDescription).HasColumnName("ACTION_DESCRIPTION");
            entity.Property(e => e.ActionStatus)
                .HasMaxLength(30)
                .HasColumnName("ACTION_STATUS");
            entity.Property(e => e.AuditId).HasColumnName("AUDIT_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.MgmtId).HasColumnName("MGMT_ID");
            entity.Property(e => e.RespId).HasColumnName("RESP_ID");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipId).HasName("PK__Equipmen__8C95A5B35298CEB2");

            entity.Property(e => e.EquipId).HasColumnName("EQUIP_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.EquipCapacity).HasColumnName("EQUIP_CAPACITY");
            entity.Property(e => e.EquipLocation)
                .HasMaxLength(200)
                .HasColumnName("EQUIP_LOCATION");
            entity.Property(e => e.EquipMaintenCycle)
                .HasMaxLength(50)
                .HasColumnName("EQUIP_MAINTEN_CYCLE");
            entity.Property(e => e.EquipModel)
                .HasMaxLength(100)
                .HasColumnName("EQUIP_MODEL");
            entity.Property(e => e.EquipName)
                .HasMaxLength(150)
                .HasColumnName("EQUIP_NAME");
            entity.Property(e => e.EquipStatus)
                .HasMaxLength(30)
                .HasColumnName("EQUIP_STATUS");
            entity.Property(e => e.EquipType)
                .HasMaxLength(100)
                .HasColumnName("EQUIP_TYPE");
            entity.Property(e => e.PlantId).HasColumnName("PLANT_ID");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<Evidence>(entity =>
        {
            entity.HasKey(e => e.EvidenceId).HasName("PK__Evidence__B9272A42AC0D457F");

            entity.ToTable("Evidence");

            entity.Property(e => e.EvidenceId).HasColumnName("EVIDENCE_ID");
            entity.Property(e => e.ActionId).HasColumnName("ACTION_ID");
            entity.Property(e => e.AuditId).HasColumnName("AUDIT_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.EvidenceTitle)
                .HasMaxLength(180)
                .HasColumnName("EVIDENCE_TITLE");
            entity.Property(e => e.EvidenceUrl)
                .HasMaxLength(500)
                .HasColumnName("EVIDENCE_URL");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<Management>(entity =>
        {
            entity.HasKey(e => e.MgmtId).HasName("PK__Manageme__6F578EC9D5F9E888");

            entity.ToTable("Management");

            entity.HasIndex(e => e.MgmtEmail, "UQ__Manageme__3349984633ADE28C").IsUnique();

            entity.Property(e => e.MgmtId)
                .ValueGeneratedNever()
                .HasColumnName("MGMT_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.MgmtEmail)
                .HasMaxLength(190)
                .HasColumnName("MGMT_EMAIL");
            entity.Property(e => e.MgmtExtension)
                .HasMaxLength(20)
                .HasColumnName("MGMT_EXTENSION");
            entity.Property(e => e.MgmtFname)
                .HasMaxLength(100)
                .HasColumnName("MGMT_FNAME");
            entity.Property(e => e.MgmtLname)
                .HasMaxLength(100)
                .HasColumnName("MGMT_LNAME");
            entity.Property(e => e.MgmtPassword)
                .HasMaxLength(255)
                .HasColumnName("MGMT_PASSWORD");
            entity.Property(e => e.MgmtRole)
                .HasMaxLength(80)
                .HasColumnName("MGMT_ROLE");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
        });

        modelBuilder.Entity<Plant>(entity =>
        {
            entity.HasKey(e => e.PlantId).HasName("PK__Plant__6CC7CE6DF4DA8588");

            entity.ToTable("Plant");

            entity.Property(e => e.PlantId).HasColumnName("PLANT_ID");
            entity.Property(e => e.AoId).HasColumnName("AO_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.MgmtId).HasColumnName("MGMT_ID");
            entity.Property(e => e.PlantCapacity).HasColumnName("PLANT_CAPACITY");
            entity.Property(e => e.PlantEquipmentCount)
                .HasDefaultValue(0)
                .HasColumnName("PLANT_EQUIPMENT_COUNT");
            entity.Property(e => e.PlantLocation)
                .HasMaxLength(200)
                .HasColumnName("PLANT_LOCATION");
            entity.Property(e => e.PlantName)
                .HasMaxLength(150)
                .HasColumnName("PLANT_NAME");
            entity.Property(e => e.PlantStatus)
                .HasMaxLength(30)
                .HasColumnName("PLANT_STATUS");
            entity.Property(e => e.PlantType)
                .HasMaxLength(80)
                .HasColumnName("PLANT_TYPE");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<ResponsiblePerson>(entity =>
        {
            entity.HasKey(e => e.RespId).HasName("PK__Responsi__112D4FBD4D58D115");

            entity.ToTable("ResponsiblePerson");

            entity.HasIndex(e => e.RespEmail, "UQ__Responsi__0678342EFF291BDF").IsUnique();

            entity.Property(e => e.RespId).HasColumnName("RESP_ID");
            entity.Property(e => e.AoId).HasColumnName("AO_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.RespEmail)
                .HasMaxLength(190)
                .HasColumnName("RESP_EMAIL");
            entity.Property(e => e.RespExtension)
                .HasMaxLength(20)
                .HasColumnName("RESP_EXTENSION");
            entity.Property(e => e.RespFname)
                .HasMaxLength(100)
                .HasColumnName("RESP_FNAME");
            entity.Property(e => e.RespLname)
                .HasMaxLength(100)
                .HasColumnName("RESP_LNAME");
            entity.Property(e => e.RespPassword)
                .HasMaxLength(255)
                .HasColumnName("RESP_PASSWORD");
            entity.Property(e => e.RespRole)
                .HasMaxLength(80)
                .HasColumnName("RESP_ROLE");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__F3BEEBFF50FDD073");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__161CF72442667540").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("USER_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.Email)
                .HasMaxLength(190)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("NAME");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("UPDATED_AT");
            entity.Property(e => e.UserType)
                .HasMaxLength(30)
                .HasColumnName("USER_TYPE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
