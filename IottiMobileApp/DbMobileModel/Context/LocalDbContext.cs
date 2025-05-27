using System;
using System.Collections.Generic;
using DbMobileModel.Models.LocalDb;
using Microsoft.EntityFrameworkCore;

namespace DbMobileModel.Context;

public partial class LocalDbContext : DbContext
{
    public LocalDbContext()
    {
    }

    public LocalDbContext(DbContextOptions<LocalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CambioStato> CambioStato { get; set; }

    public virtual DbSet<Config> Config { get; set; }

    public virtual DbSet<MissioneRig> MissioneRig { get; set; }

    public virtual DbSet<MissioneTes> MissioneTes { get; set; }

    public virtual DbSet<Stato> Stato { get; set; }

    public virtual DbSet<Utente> Utente { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source='C:\\Projects\\IOTTI_APP\\IottiMobileApp\\IottiMobileApp\\Resources\\MobileDb\\DbIOT.db'");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CambioStato>(entity =>
        {
            entity.HasKey(e => e.CstCambioId);

            entity.Property(e => e.CstCambioId).HasColumnName("cstCambioId");
            entity.Property(e => e.CstDataCambio)
                .HasColumnType("datetime")
                .HasColumnName("cstDataCambio");
            entity.Property(e => e.CstMissioneRigId)
                .HasColumnType("INT")
                .HasColumnName("cstMissioneRigId");
            entity.Property(e => e.CstStatoNew)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(50)")
                .HasColumnName("cstStatoNew");
            entity.Property(e => e.CstStatoOld)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(50)")
                .HasColumnName("cstStatoOld");
            entity.Property(e => e.CstUtenteId)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4)")
                .HasColumnName("cstUtenteId");

            entity.HasOne(d => d.CstMissioneRig).WithMany(p => p.CambioStato)
                .HasForeignKey(d => d.CstMissioneRigId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CstUtente).WithMany(p => p.CambioStato)
                .HasForeignKey(d => d.CstUtenteId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Config>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.ScriptNr).HasColumnType("INT");
        });

        modelBuilder.Entity<MissioneRig>(entity =>
        {
            entity.HasKey(e => e.MriMissioneRigId);

            entity.Property(e => e.MriMissioneRigId).HasColumnName("mriMissioneRigId");
            entity.Property(e => e.MriArticoloCodQq)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(25)")
                .HasColumnName("mriArticoloCodQQ");
            entity.Property(e => e.MriArticoloCodSap)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(125)")
                .HasColumnName("mriArticoloCodSAP");
            entity.Property(e => e.MriArticoloDesc)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(125)")
                .HasColumnName("mriArticoloDesc");
            entity.Property(e => e.MriArticoloStato)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(50)")
                .HasColumnName("mriArticoloStato");
            entity.Property(e => e.MriBetAnno)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4)")
                .HasColumnName("mriBetAnno");
            entity.Property(e => e.MriBetNumero)
                .HasColumnType("INT")
                .HasColumnName("mriBetNumero");
            entity.Property(e => e.MriBetSerie)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(2)")
                .HasColumnName("mriBetSerie");
            entity.Property(e => e.MriClienteFiera)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mriClienteFiera");
            entity.Property(e => e.MriCreateDt)
                .HasColumnType("datetime")
                .HasColumnName("mriCreateDt");
            entity.Property(e => e.MriDescOptional)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mriDescOptional");
            entity.Property(e => e.MriDescOptional2)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mriDescOptional2");
            entity.Property(e => e.MriFaseCod)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(3)")
                .HasColumnName("mriFaseCod");
            entity.Property(e => e.MriIsScorta)
                .UseCollation("NOCASE")
                .HasColumnType("nchar(1)")
                .HasColumnName("mriIsScorta");
            entity.Property(e => e.MriLottoAnno)
                .HasColumnType("INT")
                .HasColumnName("mriLottoAnno");
            entity.Property(e => e.MriLottoNumero)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(25)")
                .HasColumnName("mriLottoNumero");
            entity.Property(e => e.MriMatrCod)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(150)")
                .HasColumnName("mriMatrCod");
            entity.Property(e => e.MriMissioneTesId)
                .HasColumnType("INT")
                .HasColumnName("mriMissioneTesId");
            entity.Property(e => e.MriPadiglione)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mriPadiglione");
            entity.Property(e => e.MriQuantita)
                .HasColumnType("numeric(18,5)")
                .HasColumnName("mriQuantita");
            entity.Property(e => e.MriSapCardCode)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mriSapCardCode");
            entity.Property(e => e.MriSapCardName)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mriSapCardName");
            entity.Property(e => e.MriSapDocEntry)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mriSapDocEntry");
            entity.Property(e => e.MriSapDocNum)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mriSapDocNum");
            entity.Property(e => e.MriSapLineNum)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mriSapLineNum");
            entity.Property(e => e.MriStand)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mriStand");
            entity.Property(e => e.MriTargaAndata)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(25)")
                .HasColumnName("mriTargaAndata");
            entity.Property(e => e.MriTargaRitorno)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(25)")
                .HasColumnName("mriTargaRitorno");
            entity.Property(e => e.MriUdmFieraCod)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(50)")
                .HasColumnName("mriUdmFieraCod");
            entity.Property(e => e.MriUdmOrigineCod)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(50)")
                .HasColumnName("mriUdmOrigineCod");
            entity.Property(e => e.MriUpdateDt)
                .HasColumnType("datetime")
                .HasColumnName("mriUpdateDt");
            entity.Property(e => e.MriVarianteId)
                .HasColumnType("INT")
                .HasColumnName("mriVarianteId");

            entity.HasOne(d => d.MriArticoloStatoNavigation).WithMany(p => p.MissioneRig)
                .HasForeignKey(d => d.MriArticoloStato)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.MriMissioneTes).WithMany(p => p.MissioneRig).HasForeignKey(d => d.MriMissioneTesId);
        });

        modelBuilder.Entity<MissioneTes>(entity =>
        {
            entity.HasKey(e => e.MteMissioneTesId);

            entity.Property(e => e.MteMissioneTesId).HasColumnName("mteMissioneTesId");
            entity.Property(e => e.MteAnno)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4)")
                .HasColumnName("mteAnno");
            entity.Property(e => e.MteCreateDt)
                .HasColumnType("datetime")
                .HasColumnName("mteCreateDt");
            entity.Property(e => e.MteDataMissione)
                .HasColumnType("datetime")
                .HasColumnName("mteDataMissione");
            entity.Property(e => e.MteDescrizione)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(255)")
                .HasColumnName("mteDescrizione");
            entity.Property(e => e.MteNumero)
                .HasColumnType("INT")
                .HasColumnName("mteNumero");
            entity.Property(e => e.MteSapProjectCode)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mteSAP_ProjectCode");
            entity.Property(e => e.MteSapProjectName)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("mteSAP_ProjectName");
            entity.Property(e => e.MteSerieNumero)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(2)")
                .HasColumnName("mteSerieNumero");
            entity.Property(e => e.MteUpdateDt)
                .HasColumnType("datetime")
                .HasColumnName("mteUpdateDt");
        });

        modelBuilder.Entity<Stato>(entity =>
        {
            entity.HasKey(e => e.StsNome);

            entity.Property(e => e.StsNome)
                .HasColumnType("nvarchar(50)")
                .HasColumnName("stsNome");
            entity.Property(e => e.StsDescrizione)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(255)")
                .HasColumnName("stsDescrizione");
        });

        modelBuilder.Entity<Utente>(entity =>
        {
            entity.HasKey(e => e.UtnUtenteId);

            entity.HasIndex(e => e.UtnEmail, "Utente_UQ_Utente_Email_NotNull").IsUnique();

            entity.HasIndex(e => e.UtnUsername, "Utente_UQ_Utente_Username_NotNull").IsUnique();

            entity.Property(e => e.UtnUtenteId)
                .HasColumnType("nvarchar(4)")
                .HasColumnName("utnUtenteId");
            entity.Property(e => e.UtnCreateDt)
                .HasColumnType("datetime")
                .HasColumnName("utnCreateDt");
            entity.Property(e => e.UtnDescription)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(35)")
                .HasColumnName("utnDescription");
            entity.Property(e => e.UtnEmail)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("utnEmail");
            entity.Property(e => e.UtnPassword)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("utnPassword");
            entity.Property(e => e.UtnPhone)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(4000)")
                .HasColumnName("utnPhone");
            entity.Property(e => e.UtnRuolo)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(50)")
                .HasColumnName("utnRuolo");
            entity.Property(e => e.UtnUpdateDt)
                .HasColumnType("datetime")
                .HasColumnName("utnUpdateDt");
            entity.Property(e => e.UtnUsername)
                .UseCollation("NOCASE")
                .HasColumnType("nvarchar(50)")
                .HasColumnName("utnUsername");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
