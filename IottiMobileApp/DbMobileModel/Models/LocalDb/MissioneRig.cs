using System;
using System.Collections.Generic;

namespace DbMobileModel.Models.LocalDb;

public partial class MissioneRig
{
    public int MriMissioneRigId { get; set; }

    public int? MriMissioneTesId { get; set; }

    public string? MriArticoloCodQq { get; set; }

    public string? MriArticoloDesc { get; set; }

    public string? MriArticoloCodSap { get; set; }

    public string? MriArticoloStato { get; set; }

    public int? MriVarianteId { get; set; }

    public string? MriFaseCod { get; set; }

    public int? MriLottoAnno { get; set; }

    public string? MriLottoNumero { get; set; }

    public string? MriMatrCod { get; set; }

    public int MriQuantita { get; set; }

    public string? MriUdmOrigineCod { get; set; }

    public string? MriUdmFieraCod { get; set; }

    public string? MriSapDocNum { get; set; }

    public string? MriSapDocEntry { get; set; }

    public string? MriSapLineNum { get; set; }

    public string? MriSapCardCode { get; set; }

    public string? MriSapCardName { get; set; }

    public string? MriClienteFiera { get; set; }

    public string? MriPadiglione { get; set; }

    public string? MriStand { get; set; }

    public string? MriDescOptional { get; set; }

    public string? MriDescOptional2 { get; set; }

    public string? MriIsScorta { get; set; }

    public string? MriTargaAndata { get; set; }

    public string? MriTargaRitorno { get; set; }

    public string? MriBetSerie { get; set; }

    public string? MriBetAnno { get; set; }

    public int? MriBetNumero { get; set; }

    public DateTime MriCreateDt { get; set; }

    public DateTime MriUpdateDt { get; set; }

    public virtual ICollection<CambioStato> CambioStato { get; set; } = new List<CambioStato>();

    public virtual Stato? MriArticoloStatoNavigation { get; set; }

    public virtual MissioneTes? MriMissioneTes { get; set; }
}
