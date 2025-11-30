using System.ComponentModel.DataAnnotations.Schema;

namespace Madar.Models
{
    // Audit.cs - Add navigation properties
    public partial class Audit
    {
        // Navigation Properties
        [ForeignKey("AudSchId")]
        public virtual AuditSchedule? Schedule { get; set; }

        public virtual ICollection<Evidence> Evidences { get; set; } = new List<Evidence>();
        public virtual ICollection<CorrectiveAction> CorrectiveActions { get; set; } = new List<CorrectiveAction>();
        public virtual ICollection<AuditAttendance> Attendances { get; set; } = new List<AuditAttendance>();
        public virtual ICollection<AuditHistory> Histories { get; set; } = new List<AuditHistory>();
        public virtual ICollection<AuditEquipment> AuditEquipments { get; set; } = new List<AuditEquipment>();
    }

    // AuditSchedule.cs - Add navigation properties
    public partial class AuditSchedule
    {
        // Navigation Properties
        [ForeignKey("PlantId")]
        public virtual Plant? Plant { get; set; }

        public virtual ICollection<Audit> Audits { get; set; } = new List<Audit>();
        public virtual ICollection<AuditorAllocation> Allocations { get; set; } = new List<AuditorAllocation>();
    }

    // AuditorAllocation.cs - Add navigation properties
    public partial class AuditorAllocation
    {
        // Navigation Properties
        [ForeignKey("AudId")]
        public virtual Auditor? Auditor { get; set; }

        [ForeignKey("AudSchId")]
        public virtual AuditSchedule? Schedule { get; set; }
    }

    // Plant.cs - Add navigation properties
    public partial class Plant
    {
        // Navigation Properties
        [ForeignKey("MgmtId")]
        public virtual Management? Management { get; set; }

        [ForeignKey("AoId")]
        public virtual AreaOwner? AreaOwner { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
        public virtual ICollection<AuditSchedule> AuditSchedules { get; set; } = new List<AuditSchedule>();
    }

    // Equipment.cs - Add navigation properties
    public partial class Equipment
    {
        // Navigation Properties
        [ForeignKey("PlantId")]
        public virtual Plant? Plant { get; set; }

        public virtual ICollection<AuditEquipment> AuditEquipments { get; set; } = new List<AuditEquipment>();
    }

    // CorrectiveAction.cs - Add navigation properties
    public partial class CorrectiveAction
    {
        // Navigation Properties
        [ForeignKey("RespId")]
        public virtual ResponsiblePerson? ResponsiblePerson { get; set; }

        [ForeignKey("AuditId")]
        public virtual Audit? Audit { get; set; }

        [ForeignKey("MgmtId")]
        public virtual Management? Management { get; set; }

        public virtual ICollection<Evidence> Evidences { get; set; } = new List<Evidence>();
    }

    // Evidence.cs - Add navigation properties
    public partial class Evidence
    {
        // Navigation Properties
        [ForeignKey("AuditId")]
        public virtual Audit? Audit { get; set; }

        [ForeignKey("ActionId")]
        public virtual CorrectiveAction? CorrectiveAction { get; set; }
    }

    // AuditAttendance.cs - Add navigation properties
    public partial class AuditAttendance
    {
        // Navigation Properties
        [ForeignKey("AuditId")]
        public virtual Audit? Audit { get; set; }

        [ForeignKey("AudId")]
        public virtual Auditor? Auditor { get; set; }

        [ForeignKey("RespId")]
        public virtual ResponsiblePerson? ResponsiblePerson { get; set; }
    }

    // AuditHistory.cs - Add navigation properties
    public partial class AuditHistory
    {
        // Navigation Properties
        [ForeignKey("AuditId")]
        public virtual Audit? Audit { get; set; }

        [ForeignKey("AoId")]
        public virtual AreaOwner? AreaOwner { get; set; }

        [ForeignKey("MgmtId")]
        public virtual Management? Management { get; set; }
    }

    // AuditEquipment.cs - Add navigation properties
    public partial class AuditEquipment
    {
        // Navigation Properties
        [ForeignKey("AuditId")]
        public virtual Audit? Audit { get; set; }

        [ForeignKey("EquipId")]
        public virtual Equipment? Equipment { get; set; }
    }

    // ResponsiblePerson.cs - Add navigation properties
    public partial class ResponsiblePerson
    {
        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("AoId")]
        public virtual AreaOwner? AreaOwner { get; set; }

        public virtual ICollection<CorrectiveAction> CorrectiveActions { get; set; } = new List<CorrectiveAction>();
        public virtual ICollection<AuditAttendance> Attendances { get; set; } = new List<AuditAttendance>();
    }

    // AreaOwner.cs - Add navigation properties
    public partial class AreaOwner
    {
        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();
        public virtual ICollection<ResponsiblePerson> ResponsiblePeople { get; set; } = new List<ResponsiblePerson>();
        public virtual ICollection<AuditHistory> AuditHistories { get; set; } = new List<AuditHistory>();
    }

    // Management.cs - Add navigation properties
    public partial class Management
    {
        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();
        public virtual ICollection<CorrectiveAction> CorrectiveActions { get; set; } = new List<CorrectiveAction>();
        public virtual ICollection<AuditHistory> AuditHistories { get; set; } = new List<AuditHistory>();
    }

    // Auditor.cs - Add navigation properties
    public partial class Auditor
    {
        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<AuditorAllocation> Allocations { get; set; } = new List<AuditorAllocation>();
        public virtual ICollection<AuditAttendance> Attendances { get; set; } = new List<AuditAttendance>();
    }

    // User.cs - Add navigation properties
    public partial class User
    {
        // Navigation Properties
        public virtual Auditor? Auditor { get; set; }
        public virtual Management? Management { get; set; }
        public virtual AreaOwner? AreaOwner { get; set; }
        public virtual ResponsiblePerson? ResponsiblePerson { get; set; }
    }
}