using System;
using System.Collections.Generic;
using LibDB_EF.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibDB_EF;

public partial class BibliotecaXptoContext : DbContext
{
    private string _connectionString;

    public BibliotecaXptoContext(string connectionString)
    {
        if (connectionString != string.Empty)
        {
            _connectionString = connectionString;
        }
        else
            throw new ArgumentNullException("Connection string vazia");
        
    }
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
        
    public virtual DbSet<Exemplare> Exemplares { get; set; }

    public virtual DbSet<HistoricoRequisico> HistoricoRequisicoes { get; set; }

    public virtual DbSet<Imagen> Imagens { get; set; }

    public virtual DbSet<Nucleo> Nucleos { get; set; }

    public virtual DbSet<Obra> Obras { get; set; }

    public virtual DbSet<Requisico> Requisicoes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exemplare>(entity =>
        {
            entity.HasKey(e => e.ExemplarId).HasName("PK__Exemplar__4DDAC5BF3FD18DAB");

            entity.Property(e => e.ExemplarId).HasColumnName("ExemplarID");
            entity.Property(e => e.Disponivel).HasDefaultValue(true);
            entity.Property(e => e.NucleoId).HasColumnName("NucleoID");
            entity.Property(e => e.ObraId).HasColumnName("ObraID");

            entity.HasOne(d => d.Nucleo).WithMany(p => p.Exemplares)
                .HasForeignKey(d => d.NucleoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exemplare__Nucle__33D4B598");

            entity.HasOne(d => d.Obra).WithMany(p => p.Exemplares)
                .HasForeignKey(d => d.ObraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exemplare__ObraI__32E0915F");
        });

        modelBuilder.Entity<HistoricoRequisico>(entity =>
        {
            entity.HasKey(e => e.IdHistorico).HasName("PK__Historic__9CC7EBF36130C266");

            entity.Property(e => e.ExemplarId).HasColumnName("ExemplarID");
            entity.Property(e => e.NomeUser).HasMaxLength(255);
            entity.Property(e => e.Nucleo).HasMaxLength(100);
            entity.Property(e => e.RequisicaoId).HasColumnName("RequisicaoID");
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.TituloObra).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Imagen>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.IdObraId).HasColumnName("ID_ObraID");

            entity.HasOne(d => d.IdObra).WithMany()
                .HasForeignKey(d => d.IdObraId)
                .HasConstraintName("FK__Imagens__Capa__2F10007B");
        });

        modelBuilder.Entity<Nucleo>(entity =>
        {
            entity.HasKey(e => e.NucleoId).HasName("PK__Nucleos__B8C89F77E97CD6B0");

            entity.HasIndex(e => e.Nome, "UQ__Nucleos__7D8FE3B2C78F1006").IsUnique();

            entity.Property(e => e.NucleoId).HasColumnName("NucleoID");
            entity.Property(e => e.Endereco).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<Obra>(entity =>
        {
            entity.HasKey(e => e.ObraId).HasName("PK__Obras__F3E1F4120C1137D7");

            entity.Property(e => e.ObraId).HasColumnName("ObraID");
            entity.Property(e => e.Autor).HasMaxLength(100);
            entity.Property(e => e.Descricao).HasColumnType("text");
            entity.Property(e => e.Genero).HasMaxLength(50);
            entity.Property(e => e.Titulo).HasMaxLength(150);
        });

        modelBuilder.Entity<Requisico>(entity =>
        {
            entity.HasKey(e => e.RequisicaoId).HasName("PK__Requisic__8120ACF1A95F2483");

            entity.Property(e => e.RequisicaoId).HasColumnName("RequisicaoID");
            entity.Property(e => e.DataRequisicao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ExemplarId).HasColumnName("ExemplarID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Exemplar).WithMany(p => p.Requisicos)
                .HasForeignKey(d => d.ExemplarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Requisico__Exemp__38996AB5");

            entity.HasOne(d => d.User).WithMany(p => p.Requisicos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Requisico__UserI__37A5467C");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Usuario__1788CCACFD587666");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Email, "UQ__Usuario__A9D10534576F53D9").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.DataRegisto).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.TipoUser)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("Leitor");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
