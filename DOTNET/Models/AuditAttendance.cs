using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class AuditAttendance
{
    public long AudAttendId { get; set; }

    public long AuditId { get; set; }

    public long AudId { get; set; }

    public long? RespId { get; set; }

    public DateOnly AudAttendDate { get; set; }

    public string AudAttendStatus { get; set; } = null!;

    public TimeOnly? AudArrivalTime { get; set; }

    public TimeOnly? AudDepartTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
