using System;
using System.Collections.Generic;

namespace DbMobileModel.Models.IntermediateDb;

public partial class CambioStato
{
    /// <summary>
    /// PK Cambio Stato
    /// </summary>
    public int CstCambioId { get; set; }

    /// <summary>
    /// FK a Riga Missione
    /// </summary>
    public int CstMissioneRigId { get; set; }

    /// <summary>
    /// Utente che modifica
    /// </summary>
    public string CstUtenteId { get; set; } = null!;

    /// <summary>
    /// Stato precedente
    /// </summary>
    public string CstStatoOld { get; set; } = null!;

    /// <summary>
    /// Stato nuovo
    /// </summary>
    public string CstStatoNew { get; set; } = null!;

    /// <summary>
    /// Data cambio stato
    /// </summary>
    public DateTime CstDataCambio { get; set; }

    public virtual MissioneRig CstMissioneRig { get; set; } = null!;

    public virtual Utente CstUtente { get; set; } = null!;
}
