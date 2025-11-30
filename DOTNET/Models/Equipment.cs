using System;
using System.Collections.Generic;

namespace Madar.Models;

public partial class Equipment
{
    public long EquipId { get; set; }

    public long PlantId { get; set; }

    public string EquipName { get; set; } = null!;

    public string? EquipType { get; set; }

    public string? EquipModel { get; set; }

    public string EquipStatus { get; set; } = null!;

    public string? EquipLocation { get; set; }

    public int? EquipCapacity { get; set; }

    public string? EquipMaintenCycle { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
