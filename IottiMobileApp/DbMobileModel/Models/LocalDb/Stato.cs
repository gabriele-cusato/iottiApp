using System;
using System.Collections.Generic;

namespace DbMobileModel.Models.LocalDb;

public partial class Stato
{
    public string StsNome { get; set; } = null!;

    public string? StsDescrizione { get; set; }

    public virtual ICollection<MissioneRig> MissioneRig { get; set; } = new List<MissioneRig>();
}
