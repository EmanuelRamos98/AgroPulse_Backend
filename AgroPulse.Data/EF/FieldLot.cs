using System;
using System.Collections.Generic;

namespace AgroPulse.Data.EF;

public partial class FieldLot
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public decimal Hectares { get; set; }

    /// <summary>
    /// 0:Barbecho, 1:Soja, 2:Maiz, 3:Trigo, 4:Girasol, 5:Pastura
    /// </summary>
    public sbyte CropType { get; set; }

    /// <summary>
    /// 0:PreSiembra, 1:Siembra, 2:Vegetativo, 3:FloracionCritica, 4:LlenadoGrano, 5:Cosecha
    /// </summary>
    public sbyte CropStage { get; set; }

    public DateOnly? SowingDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AgroAlert> AgroAlerts { get; set; } = new List<AgroAlert>();

    public virtual ICollection<RainRecord> RainRecords { get; set; } = new List<RainRecord>();

    public virtual User User { get; set; } = null!;
}
