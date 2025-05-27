using System;
using System.Collections.Generic;
using DbMobileModel.Models.IntermediateDb;
using Microsoft.EntityFrameworkCore;

namespace DbMobileModel.Context;

public partial class IntermediateDbContext : DbContext
{
    public IntermediateDbContext()
    {
    }

    public IntermediateDbContext(DbContextOptions<IntermediateDbContext> options)
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
        => optionsBuilder.UseSqlServer("Server=192.168.184.130,1433;Database=DbIOT;User Id=sa;Password=aA1bB2cC3dD4eE5;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CambioStato>(entity =>
        {
            entity.HasKey(e => e.CstCambioId);

            entity.Property(e => e.CstCambioId)
                .HasComment("PK Cambio Stato")
                .HasColumnName("cstCambioId");
            entity.Property(e => e.CstDataCambio)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Data cambio stato")
                .HasColumnName("cstDataCambio");
            entity.Property(e => e.CstMissioneRigId)
                .HasComment("FK a Riga Missione")
                .HasColumnName("cstMissioneRigId");
            entity.Property(e => e.CstStatoNew)
                .HasMaxLength(50)
                .HasComment("Stato nuovo")
                .HasColumnName("cstStatoNew");
            entity.Property(e => e.CstStatoOld)
                .HasMaxLength(50)
                .HasComment("Stato precedente")
                .HasColumnName("cstStatoOld");
            entity.Property(e => e.CstUtenteId)
                .HasMaxLength(4)
                .HasComment("Utente che modifica")
                .HasColumnName("cstUtenteId");

            entity.HasOne(d => d.CstMissioneRig).WithMany(p => p.CambioStato)
                .HasForeignKey(d => d.CstMissioneRigId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CambioStato_MissioneRig");

            entity.HasOne(d => d.CstUtente).WithMany(p => p.CambioStato)
                .HasForeignKey(d => d.CstUtenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CambioStato_Utente");
        });

        modelBuilder.Entity<Config>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<MissioneRig>(entity =>
        {
            entity.HasKey(e => e.MriMissioneRigId);

            entity.Property(e => e.MriMissioneRigId)
                .HasComment("PK Riga Missione")
                .HasColumnName("mriMissioneRigId");
            entity.Property(e => e.MriArticoloCodQq)
                .HasMaxLength(25)
                .HasComment("Codice Articolo QQ")
                .HasColumnName("mriArticoloCodQQ");
            entity.Property(e => e.MriArticoloCodSap)
                .HasMaxLength(125)
                .HasComment("Codice Articolo SAP")
                .HasColumnName("mriArticoloCodSAP");
            entity.Property(e => e.MriArticoloDesc)
                .HasMaxLength(125)
                .HasComment("Descrizione Articolo")
                .HasColumnName("mriArticoloDesc");
            entity.Property(e => e.MriArticoloStato)
                .HasMaxLength(50)
                .HasComment("Stato Articolo")
                .HasColumnName("mriArticoloStato");
            entity.Property(e => e.MriBetAnno)
                .HasMaxLength(4)
                .HasComment("Anno DDT Emesso QQ")
                .HasColumnName("mriBetAnno");
            entity.Property(e => e.MriBetNumero)
                .HasComment("Numero DDT Emesso QQ")
                .HasColumnName("mriBetNumero");
            entity.Property(e => e.MriBetSerie)
                .HasMaxLength(2)
                .HasComment("Serie DDT Emesso QQ")
                .HasColumnName("mriBetSerie");
            entity.Property(e => e.MriClienteFiera)
                .HasMaxLength(4000)
                .HasComment("Cliente Fiera/Espositore")
                .HasColumnName("mriClienteFiera");
            entity.Property(e => e.MriCreateDt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Data creazione record")
                .HasColumnName("mriCreateDt");
            entity.Property(e => e.MriDescOptional)
                .HasMaxLength(4000)
                .HasComment("Optional Aggiuntivi Articolo")
                .HasColumnName("mriDescOptional");
            entity.Property(e => e.MriDescOptional2)
                .HasMaxLength(4000)
                .HasComment("Optional Aggiuntivi Articolo 2")
                .HasColumnName("mriDescOptional2");
            entity.Property(e => e.MriFaseCod)
                .HasMaxLength(3)
                .HasComment("Codice Fase")
                .HasColumnName("mriFaseCod");
            entity.Property(e => e.MriIsScorta)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasComment("Indicatore Articolo Scorta (S/N)")
                .HasColumnName("mriIsScorta");
            entity.Property(e => e.MriLottoAnno)
                .HasComment("Lotto Anno")
                .HasColumnName("mriLottoAnno");
            entity.Property(e => e.MriLottoNumero)
                .HasMaxLength(25)
                .HasComment("Lotto Numero")
                .HasColumnName("mriLottoNumero");
            entity.Property(e => e.MriMatrCod)
                .HasMaxLength(150)
                .HasComment("Matricola/Seriale")
                .HasColumnName("mriMatrCod");
            entity.Property(e => e.MriMissioneTesId)
                .HasComment("FK a Testata Missione")
                .HasColumnName("mriMissioneTesId");
            entity.Property(e => e.MriPadiglione)
                .HasMaxLength(4000)
                .HasComment("Padiglione")
                .HasColumnName("mriPadiglione");
            entity.Property(e => e.MriQuantita)
                .HasComment("Quantità")
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("mriQuantita");
            entity.Property(e => e.MriSapCardCode)
                .HasMaxLength(4000)
                .HasComment("Codice Cliente Fatturazione")
                .HasColumnName("mriSapCardCode");
            entity.Property(e => e.MriSapCardName)
                .HasMaxLength(4000)
                .HasComment("Ragione Sociale Cliente Fatturazione")
                .HasColumnName("mriSapCardName");
            entity.Property(e => e.MriSapDocEntry)
                .HasMaxLength(4000)
                .HasComment("Doc Entry SAP")
                .HasColumnName("mriSapDocEntry");
            entity.Property(e => e.MriSapDocNum)
                .HasMaxLength(4000)
                .HasComment("Numero Ordine SAP")
                .HasColumnName("mriSapDocNum");
            entity.Property(e => e.MriSapLineNum)
                .HasMaxLength(4000)
                .HasComment("Numero Riga Ordine SAP")
                .HasColumnName("mriSapLineNum");
            entity.Property(e => e.MriStand)
                .HasMaxLength(4000)
                .HasComment("Stand")
                .HasColumnName("mriStand");
            entity.Property(e => e.MriTargaAndata)
                .HasMaxLength(25)
                .HasComment("Targa Andata")
                .HasColumnName("mriTargaAndata");
            entity.Property(e => e.MriTargaRitorno)
                .HasMaxLength(25)
                .HasComment("Targa Ritorno")
                .HasColumnName("mriTargaRitorno");
            entity.Property(e => e.MriUdmFieraCod)
                .HasMaxLength(50)
                .HasComment("Codice UDM Fiera")
                .HasColumnName("mriUdmFieraCod");
            entity.Property(e => e.MriUdmOrigineCod)
                .HasMaxLength(50)
                .HasComment("Codice UDM Origine")
                .HasColumnName("mriUdmOrigineCod");
            entity.Property(e => e.MriUpdateDt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Data ultimo aggiornamento")
                .HasColumnName("mriUpdateDt");
            entity.Property(e => e.MriVarianteId)
                .HasComment("IdRk Variante Articolo")
                .HasColumnName("mriVarianteId");

            entity.HasOne(d => d.MriArticoloStatoNavigation).WithMany(p => p.MissioneRig)
                .HasForeignKey(d => d.MriArticoloStato)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_MissioneRig_Stato");

            entity.HasOne(d => d.MriMissioneTes).WithMany(p => p.MissioneRig)
                .HasForeignKey(d => d.MriMissioneTesId)
                .HasConstraintName("FK_MissioneRig_Testata");
        });

        modelBuilder.Entity<MissioneTes>(entity =>
        {
            entity.HasKey(e => e.MteMissioneTesId);

            entity.Property(e => e.MteMissioneTesId)
                .HasComment("PK MissioneTestata")
                .HasColumnName("mteMissioneTesId");
            entity.Property(e => e.MteAnno)
                .HasMaxLength(4)
                .HasComment("Anno")
                .HasColumnName("mteAnno");
            entity.Property(e => e.MteCreateDt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Data creazione record")
                .HasColumnName("mteCreateDt");
            entity.Property(e => e.MteDataMissione)
                .HasComment("Data Missione")
                .HasColumnType("datetime")
                .HasColumnName("mteDataMissione");
            entity.Property(e => e.MteDescrizione)
                .HasMaxLength(255)
                .HasComment("Descrizione")
                .HasColumnName("mteDescrizione");
            entity.Property(e => e.MteNumero)
                .HasComment("Numero")
                .HasColumnName("mteNumero");
            entity.Property(e => e.MteSapProjectCode)
                .HasMaxLength(4000)
                .HasComment("Codice fiera")
                .HasColumnName("mteSAP_ProjectCode");
            entity.Property(e => e.MteSapProjectName)
                .HasMaxLength(4000)
                .HasComment("Descrizione fiera")
                .HasColumnName("mteSAP_ProjectName");
            entity.Property(e => e.MteSerieNumero)
                .HasMaxLength(2)
                .HasComment("Serie Numerazione")
                .HasColumnName("mteSerieNumero");
            entity.Property(e => e.MteUpdateDt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Data ultimo aggiornamento")
                .HasColumnName("mteUpdateDt");
        });

        modelBuilder.Entity<Stato>(entity =>
        {
            entity.HasKey(e => e.StsNome);

            entity.Property(e => e.StsNome)
                .HasMaxLength(50)
                .HasColumnName("stsNome");
            entity.Property(e => e.StsDescrizione)
                .HasMaxLength(255)
                .HasColumnName("stsDescrizione");
        });

        modelBuilder.Entity<Utente>(entity =>
        {
            entity.HasKey(e => e.UtnUtenteId)
                .IsClustered(false)
                .HasFillFactor(90);

            entity.HasIndex(e => e.UtnEmail, "UQ_Utente_Email_NotNull")
                .IsUnique()
                .HasFilter("([utnEmail] IS NOT NULL)");

            entity.HasIndex(e => e.UtnUsername, "UQ_Utente_Username_NotNull")
                .IsUnique()
                .HasFilter("([utnUsername] IS NOT NULL)");

            entity.Property(e => e.UtnUtenteId)
                .HasMaxLength(4)
                .HasComment("Codice Utente PK")
                .HasColumnName("utnUtenteId");
            entity.Property(e => e.UtnCreateDt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Data creazione record")
                .HasColumnName("utnCreateDt");
            entity.Property(e => e.UtnDescription)
                .HasMaxLength(35)
                .HasComment("Descrizione Utente")
                .HasColumnName("utnDescription");
            entity.Property(e => e.UtnEmail)
                .HasMaxLength(4000)
                .HasComment("Email Utente")
                .HasColumnName("utnEmail");
            entity.Property(e => e.UtnPassword)
                .HasMaxLength(4000)
                .HasComment("Password Utente")
                .HasColumnName("utnPassword");
            entity.Property(e => e.UtnPhone)
                .HasMaxLength(4000)
                .HasComment("Telefono Utente")
                .HasColumnName("utnPhone");
            entity.Property(e => e.UtnRuolo)
                .HasMaxLength(50)
                .HasColumnName("utnRuolo");
            entity.Property(e => e.UtnUpdateDt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Data ultimo aggiornamento")
                .HasColumnName("utnUpdateDt");
            entity.Property(e => e.UtnUsername)
                .HasMaxLength(50)
                .HasColumnName("utnUsername");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
