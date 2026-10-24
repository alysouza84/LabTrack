using LabTrack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LabTrack.Api.Data;

public class LabTrackContext : DbContext // ':' é a herença, ou seja, LabTrackContext herda de DbContext. Herda tudo que o DbContext tem, e pode sobrescrever métodos dele.
{
    public LabTrackContext(DbContextOptions<LabTrackContext> options) : base(options) // Construtor que recebe as opções de configuração do DbContext e passa para a classe base (DbContext) para inicializar o contexto do banco de dados.
    // Nesse contexto, quem decide o banco é o Program.cs, que le o appsettings.json. Trocar de banco ou de arquivo torna-se configuracao, e nao codigo.
    {
    }

    // Construcao da tabela Equipamentos no banco de dados, com base na classe Equipamento. O nome da tabela será Equipamentos (que é o nome da propriedade), e as colunas serão definidas pelas propriedades da classe Equipamento.

    public DbSet<Equipamento> Equipamentos => Set<Equipamento>(); 
    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Emprestimo> Emprestimos => Set<Emprestimo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Equipamento>(entity =>
        {
            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Patrimonio)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.HasIndex(e => e.Patrimonio)
                .IsUnique();
                
        });

        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.Property(a => a.Nome)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(a => a.Matricula)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(a => a.Matricula)
                .IsUnique();
        });

        modelBuilder.Entity<Emprestimo>(entity=>
        {
            entity.HasOne<Aluno>()
                .WithMany()
                .HasForeignKey(e => e.AlunoId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne<Equipamento>()
                .WithMany()
                .HasForeignKey(e => e.EquipamentoId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.EquipamentoId)
                .IsUnique()
                .HasFilter("\"DataDevolução\" IS NULL");
        });
    }
}