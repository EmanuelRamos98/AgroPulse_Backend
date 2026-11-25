using System;
using System.Collections.Generic;

namespace AgroPulse.Data.EF;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<FieldLot> FieldLots { get; set; } = new List<FieldLot>();
}
