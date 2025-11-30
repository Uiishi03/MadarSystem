using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class Evidence
{
    public long EvidenceId { get; set; }

    public long? AuditId { get; set; }

    public string? EvidenceTitle { get; set; }

    public string EvidenceUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? ActionId { get; set; }
}
