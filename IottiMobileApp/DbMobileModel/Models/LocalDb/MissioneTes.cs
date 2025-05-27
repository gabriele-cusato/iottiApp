using System;
using System.Collections.Generic;

namespace DbMobileModel.Models.LocalDb;

public partial class MissioneTes
{
    public int MteMissioneTesId { get; set; }

    public string? MteSerieNumero { get; set; }

    public string? MteAnno { get; set; }

    public int? MteNumero { get; set; }

    public DateTime? MteDataMissione { get; set; }

    public string? MteDescrizione { get; set; }

    public string? MteSapProjectCode { get; set; }

    public string? MteSapProjectName { get; set; }

    public DateTime MteCreateDt { get; set; }

    public DateTime MteUpdateDt { get; set; }

    public virtual ICollection<MissioneRig> MissioneRig { get; set; } = new List<MissioneRig>();
}
