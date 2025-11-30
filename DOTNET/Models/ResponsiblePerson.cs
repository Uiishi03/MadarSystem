using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class ResponsiblePerson
{
    public long RespId { get; set; }

    public long UserId { get; set; }

    public long AoId { get; set; }

    public string RespFname { get; set; } = null!;

    public string RespLname { get; set; } = null!;

    public string? RespRole { get; set; }

    public string RespEmail { get; set; } = null!;

    public string? RespExtension { get; set; }

    public string RespPassword { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
