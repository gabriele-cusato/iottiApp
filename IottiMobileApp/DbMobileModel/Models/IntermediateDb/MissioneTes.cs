using System;
using System.Collections.Generic;

namespace DbMobileModel.Models.IntermediateDb;

public partial class MissioneTes
{
    /// <summary>
    /// PK MissioneTestata
    /// </summary>
    public int MteMissioneTesId { get; set; }

    /// <summary>
    /// Serie Numerazione
    /// </summary>
    public string? MteSerieNumero { get; set; }

    /// <summary>
    /// Anno
    /// </summary>
    public string? MteAnno { get; set; }

    /// <summary>
    /// Numero
    /// </summary>
    public int? MteNumero { get; set; }

    /// <summary>
    /// Data Missione
    /// </summary>
    public DateTime? MteDataMissione { get; set; }

    /// <summary>
    /// Descrizione
    /// </summary>
    public string? MteDescrizione { get; set; }

    /// <summary>
    /// Codice fiera
    /// </summary>
    public string? MteSapProjectCode { get; set; }

    /// <summary>
    /// Descrizione fiera
    /// </summary>
    public string? MteSapProjectName { get; set; }

    /// <summary>
    /// Data creazione record
    /// </summary>
    public DateTime MteCreateDt { get; set; }

    /// <summary>
    /// Data ultimo aggiornamento
    /// </summary>
    public DateTime MteUpdateDt { get; set; }

    public virtual ICollection<MissioneRig> MissioneRig { get; set; } = new List<MissioneRig>();
}
