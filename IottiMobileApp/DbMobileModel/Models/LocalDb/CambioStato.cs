using System;
using System.Collections.Generic;

namespace DbMobileModel.Models.LocalDb;

public partial class CambioStato
{
    public int CstCambioId { get; set; }

    public int CstMissioneRigId { get; set; }

    public string CstUtenteId { get; set; } = null!;

    public string CstStatoOld { get; set; } = null!;

    public string CstStatoNew { get; set; } = null!;

    public DateTime CstDataCambio { get; set; }

    public virtual MissioneRig CstMissioneRig { get; set; } = null!;

    public virtual Utente CstUtente { get; set; } = null!;
}
