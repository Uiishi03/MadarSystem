using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class AreaOwner
{
    public long AoId { get; set; }

    public long UserId { get; set; }

    public string AoFname { get; set; } = null!;

    public string AoLname { get; set; } = null!;

    public string AoEmail { get; set; } = null!;

    public string? AoExtension { get; set; }

    public string? AoRole { get; set; }

    public string AoPassword { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
