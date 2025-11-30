using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class CorrectiveAction
{
    public long ActionId { get; set; }

    public long? RespId { get; set; }

    public long AuditId { get; set; }

    public long? MgmtId { get; set; }

    public string ActionDescription { get; set; } = null!;

    public DateOnly ActionDeadlineDate { get; set; }

    public string ActionStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
