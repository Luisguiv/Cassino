using Microsoft.EntityFrameworkCore;
using ApiCassino.Models;

namespace ApiCassino.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Jogador> Jogadores { get; set; }
        public DbSet<Carteira> Carteiras { get; set; }
        public DbSet<Aposta> Apostas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações específicas
            modelBuilder.Entity<Jogador>()
                .HasIndex(j => j.Email)
                .IsUnique();

            // Relacionamento 1:1 Jogador-Carteira
            modelBuilder.Entity<Carteira>()
                .HasOne(c => c.Jogador)
                .WithOne(j => j.Carteira)
                .HasForeignKey<Carteira>(c => c.JogadorId);

            // Relacionamento 1:N Jogador-Apostas
            modelBuilder.Entity<Aposta>()
                .HasOne(a => a.Jogador)
                .WithMany(j => j.Apostas)
                .HasForeignKey(a => a.JogadorId);

            // Relacionamento 1:N Carteira-Transacoes
            modelBuilder.Entity<Transacao>()
                .HasOne(t => t.Carteira)
                .WithMany(c => c.Transacoes)
                .HasForeignKey(t => t.CarteiraId);
        }
    }
}