using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class AuditEquipment
{
    public long AuditId { get; set; }

    public long EquipId { get; set; }

    public DateOnly? AeDate { get; set; }

    public TimeOnly? AeTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
