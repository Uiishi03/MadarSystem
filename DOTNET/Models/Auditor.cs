using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class Auditor
{
    public long AudId { get; set; }

    public long UserId { get; set; }

    public string AudFname { get; set; } = null!;

    public string AudLname { get; set; } = null!;

    public string AudEmail { get; set; } = null!;

    public string? AudExtension { get; set; }

    public string? AudRole { get; set; }

    public string AudPassword { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
