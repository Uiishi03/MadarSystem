using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class AuditSchedule
{
    public long AudSchId { get; set; }

    public long PlantId { get; set; }

    public DateOnly AudSchDate { get; set; }

    public string AudSchStatus { get; set; } = null!;

    public int? AudSchDuration { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
