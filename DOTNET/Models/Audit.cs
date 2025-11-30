using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class Audit
{
    public long AuditId { get; set; }

    public long AudSchId { get; set; }

    public string AuditTitle { get; set; } = null!;

    public string? AuditDescription { get; set; }

    public string? AuditChecklistSteps { get; set; }

    public string AuditStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
