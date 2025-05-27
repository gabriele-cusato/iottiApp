using System;
using System.Collections.Generic;

namespace DbMobileModel.Models.IntermediateDb;

public partial class Utente
{
    /// <summary>
    /// Codice Utente PK
    /// </summary>
    public string UtnUtenteId { get; set; } = null!;

    /// <summary>
    /// Descrizione Utente
    /// </summary>
    public string? UtnDescription { get; set; }

    /// <summary>
    /// Email Utente
    /// </summary>
    public string? UtnEmail { get; set; }

    public string? UtnUsername { get; set; }

    /// <summary>
    /// Password Utente
    /// </summary>
    public string? UtnPassword { get; set; }

    /// <summary>
    /// Telefono Utente
    /// </summary>
    public string? UtnPhone { get; set; }

    public string? UtnRuolo { get; set; }

    /// <summary>
    /// Data creazione record
    /// </summary>
    public DateTime UtnCreateDt { get; set; }

    /// <summary>
    /// Data ultimo aggiornamento
    /// </summary>
    public DateTime UtnUpdateDt { get; set; }

    public virtual ICollection<CambioStato> CambioStato { get; set; } = new List<CambioStato>();
}
