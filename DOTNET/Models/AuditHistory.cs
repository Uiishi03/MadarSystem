using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class AuditHistory
{
    public long AudHistoryId { get; set; }

    public long AuditId { get; set; }

    public long AoId { get; set; }

    public long MgmtId { get; set; }

    public string? AudHistoryTitle { get; set; }

    public string AudHistoryStatus { get; set; } = null!;

    public decimal? AudHistoryScore { get; set; }

    public string? AudHistoryComments { get; set; }

    public string? AudHistoryEscalationLevel { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
