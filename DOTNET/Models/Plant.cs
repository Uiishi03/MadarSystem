using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class Plant
{
    public long PlantId { get; set; }

    public long MgmtId { get; set; }

    public long AoId { get; set; }

    public string PlantName { get; set; } = null!;

    public string? PlantLocation { get; set; }

    public string PlantStatus { get; set; } = null!;

    public string? PlantType { get; set; }

    public int? PlantCapacity { get; set; }

    public int? PlantEquipmentCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
