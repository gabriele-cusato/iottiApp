using System;
using System.Collections.Generic;

namespace DbMobileModel.Models.IntermediateDb;

public partial class MissioneRig
{
    /// <summary>
    /// PK Riga Missione
    /// </summary>
    public int MriMissioneRigId { get; set; }

    /// <summary>
    /// FK a Testata Missione
    /// </summary>
    public int? MriMissioneTesId { get; set; }

    /// <summary>
    /// Codice Articolo QQ
    /// </summary>
    public string? MriArticoloCodQq { get; set; }

    /// <summary>
    /// Descrizione Articolo
    /// </summary>
    public string? MriArticoloDesc { get; set; }

    /// <summary>
    /// Codice Articolo SAP
    /// </summary>
    public string? MriArticoloCodSap { get; set; }

    /// <summary>
    /// Stato Articolo
    /// </summary>
    public string? MriArticoloStato { get; set; }

    /// <summary>
    /// IdRk Variante Articolo
    /// </summary>
    public int? MriVarianteId { get; set; }

    /// <summary>
    /// Codice Fase
    /// </summary>
    public string? MriFaseCod { get; set; }

    /// <summary>
    /// Lotto Anno
    /// </summary>
    public int? MriLottoAnno { get; set; }

    /// <summary>
    /// Lotto Numero
    /// </summary>
    public string? MriLottoNumero { get; set; }

    /// <summary>
    /// Matricola/Seriale
    /// </summary>
    public string? MriMatrCod { get; set; }

    /// <summary>
    /// Quantità
    /// </summary>
    public decimal MriQuantita { get; set; }

    /// <summary>
    /// Codice UDM Origine
    /// </summary>
    public string? MriUdmOrigineCod { get; set; }

    /// <summary>
    /// Codice UDM Fiera
    /// </summary>
    public string? MriUdmFieraCod { get; set; }

    /// <summary>
    /// Numero Ordine SAP
    /// </summary>
    public string? MriSapDocNum { get; set; }

    /// <summary>
    /// Doc Entry SAP
    /// </summary>
    public string? MriSapDocEntry { get; set; }

    /// <summary>
    /// Numero Riga Ordine SAP
    /// </summary>
    public string? MriSapLineNum { get; set; }

    /// <summary>
    /// Codice Cliente Fatturazione
    /// </summary>
    public string? MriSapCardCode { get; set; }

    /// <summary>
    /// Ragione Sociale Cliente Fatturazione
    /// </summary>
    public string? MriSapCardName { get; set; }

    /// <summary>
    /// Cliente Fiera/Espositore
    /// </summary>
    public string? MriClienteFiera { get; set; }

    /// <summary>
    /// Padiglione
    /// </summary>
    public string? MriPadiglione { get; set; }

    /// <summary>
    /// Stand
    /// </summary>
    public string? MriStand { get; set; }

    /// <summary>
    /// Optional Aggiuntivi Articolo
    /// </summary>
    public string? MriDescOptional { get; set; }

    /// <summary>
    /// Optional Aggiuntivi Articolo 2
    /// </summary>
    public string? MriDescOptional2 { get; set; }

    /// <summary>
    /// Indicatore Articolo Scorta (S/N)
    /// </summary>
    public string? MriIsScorta { get; set; }

    /// <summary>
    /// Targa Andata
    /// </summary>
    public string? MriTargaAndata { get; set; }

    /// <summary>
    /// Targa Ritorno
    /// </summary>
    public string? MriTargaRitorno { get; set; }

    /// <summary>
    /// Serie DDT Emesso QQ
    /// </summary>
    public string? MriBetSerie { get; set; }

    /// <summary>
    /// Anno DDT Emesso QQ
    /// </summary>
    public string? MriBetAnno { get; set; }

    /// <summary>
    /// Numero DDT Emesso QQ
    /// </summary>
    public int? MriBetNumero { get; set; }

    /// <summary>
    /// Data creazione record
    /// </summary>
    public DateTime MriCreateDt { get; set; }

    /// <summary>
    /// Data ultimo aggiornamento
    /// </summary>
    public DateTime MriUpdateDt { get; set; }

    public virtual ICollection<CambioStato> CambioStato { get; set; } = new List<CambioStato>();

    public virtual Stato? MriArticoloStatoNavigation { get; set; }

    public virtual MissioneTes? MriMissioneTes { get; set; }
}
