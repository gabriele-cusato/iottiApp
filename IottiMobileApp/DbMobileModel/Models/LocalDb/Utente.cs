using System;
using System.Collections.Generic;

namespace DbMobileModel.Models.LocalDb;

public partial class Utente
{
    public string UtnUtenteId { get; set; } = null!;

    public string? UtnDescription { get; set; }

    public string? UtnEmail { get; set; }

    public string? UtnUsername { get; set; }

    public string? UtnPassword { get; set; }

    public string? UtnPhone { get; set; }

    public string? UtnRuolo { get; set; }

    public DateTime UtnCreateDt { get; set; }

    public DateTime UtnUpdateDt { get; set; }

    public virtual ICollection<CambioStato> CambioStato { get; set; } = new List<CambioStato>();
}
