using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class AuditorAllocation
{
    public long AudId { get; set; }

    public long AudSchId { get; set; }

    public DateOnly? AssignedDate { get; set; }

    public string? RoleType { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
