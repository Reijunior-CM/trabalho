using Locadora.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Data;

public class LocadoraDbContext(DbContextOptions<LocadoraDbContext> options) : DbContext(options)
{
    public DbSet<Fabricante> Fabricantes => Set<Fabricante>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Aluguel> Alugueis => Set<Aluguel>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Fabricante>(e =>
        {
            e.ToTable("Fabricantes"); e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.Property(x => x.Pais).HasMaxLength(80).IsRequired();
            e.HasIndex(x => x.Nome).IsUnique();
        });
        b.Entity<Veiculo>(e =>
        {
            e.ToTable("Veiculos", t => { t.HasCheckConstraint("CK_Veiculo_Ano", "[AnoFabricacao] BETWEEN 1900 AND 2100"); t.HasCheckConstraint("CK_Veiculo_Km", "[Quilometragem] >= 0"); t.HasCheckConstraint("CK_Veiculo_Diaria", "[ValorDiaria] > 0"); });
            e.HasKey(x => x.Id);
            e.Property(x => x.Modelo).HasMaxLength(100).IsRequired();
            e.Property(x => x.Placa).HasMaxLength(8).IsRequired();
            e.Property(x => x.ValorDiaria).HasPrecision(12, 2);
            e.HasIndex(x => x.Placa).IsUnique();
            e.HasOne(x => x.Fabricante).WithMany(x => x.Veiculos).HasForeignKey(x => x.FabricanteId).OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<Cliente>(e =>
        {
            e.ToTable("Clientes"); e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            e.Property(x => x.Cpf).HasMaxLength(11).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Cpf).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });
        b.Entity<Aluguel>(e =>
        {
            e.ToTable("Alugueis", t => { t.HasCheckConstraint("CK_Aluguel_Periodo", "[FimPrevisto] > [Inicio]"); t.HasCheckConstraint("CK_Aluguel_Km", "[QuilometragemInicial] >= 0 AND ([QuilometragemFinal] IS NULL OR [QuilometragemFinal] >= [QuilometragemInicial])"); t.HasCheckConstraint("CK_Aluguel_Diaria", "[ValorDiaria] > 0"); t.HasCheckConstraint("CK_Aluguel_Total", "[ValorTotal] IS NULL OR [ValorTotal] >= 0"); t.HasCheckConstraint("CK_Aluguel_Devolucao", "([Devolucao] IS NULL AND [QuilometragemFinal] IS NULL AND [ValorTotal] IS NULL) OR ([Devolucao] IS NOT NULL AND [QuilometragemFinal] IS NOT NULL AND [ValorTotal] IS NOT NULL AND [Devolucao] >= [Inicio])"); });
            e.HasKey(x => x.Id);
            e.Property(x => x.ValorDiaria).HasPrecision(12, 2);
            e.Property(x => x.ValorTotal).HasPrecision(12, 2);
            e.HasOne(x => x.Cliente).WithMany(x => x.Alugueis).HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Veiculo).WithMany(x => x.Alugueis).HasForeignKey(x => x.VeiculoId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.VeiculoId, x.Inicio, x.FimPrevisto });
        });
        b.Entity<Pagamento>(e =>
        {
            e.ToTable("Pagamentos", t => t.HasCheckConstraint("CK_Pagamento_Valor", "[Valor] > 0"));
            e.HasKey(x => x.Id);
            e.Property(x => x.Valor).HasPrecision(12, 2);
            e.Property(x => x.Forma).HasMaxLength(30).IsRequired();
            e.HasOne(x => x.Aluguel).WithMany(x => x.Pagamentos).HasForeignKey(x => x.AluguelId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
