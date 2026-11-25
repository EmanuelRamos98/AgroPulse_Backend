using System;
using System.Collections.Generic;

namespace AgroPulse.Data.EF;

public partial class RainRecord
{
    public int Id { get; set; }

    public int FieldLotId { get; set; }

    public DateOnly RecordDate { get; set; }

    public decimal Millimeters { get; set; }

    /// <summary>
    /// 1:ApiSatelital, 2:ManualPluviometro
    /// </summary>
    public sbyte Source { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual FieldLot FieldLot { get; set; } = null!;
}
