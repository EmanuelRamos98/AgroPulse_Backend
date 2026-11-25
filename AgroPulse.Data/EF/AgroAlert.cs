using System;
using System.Collections.Generic;

namespace AgroPulse.Data.EF;

public partial class AgroAlert
{
    public int Id { get; set; }

    public int FieldLotId { get; set; }

    /// <summary>
    /// 1:HeladaAgronomica, 2:VentanaPulverizacion, 3:EstresHidrico, 4:SueloFrioSiembra
    /// </summary>
    public sbyte AlertType { get; set; }

    /// <summary>
    /// 1:Info, 2:Warning, 3:Critical
    /// </summary>
    public sbyte Severity { get; set; }

    public string Message { get; set; } = null!;

    public DateTime TargetDate { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual FieldLot FieldLot { get; set; } = null!;
}
