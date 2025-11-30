using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class Management
{
    public long MgmtId { get; set; }

    public long UserId { get; set; }

    public string MgmtFname { get; set; } = null!;

    public string MgmtLname { get; set; } = null!;

    public string MgmtEmail { get; set; } = null!;

    public string? MgmtExtension { get; set; }

    public string? MgmtRole { get; set; }

    public string MgmtPassword { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
