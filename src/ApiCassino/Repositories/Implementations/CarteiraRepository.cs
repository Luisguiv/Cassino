using Microsoft.EntityFrameworkCore;
using ApiCassino.Data;
using ApiCassino.Models;
using ApiCassino.Repositories.Interfaces;

namespace ApiCassino.Repositories.Implementations
{
    public class CarteiraRepository : Repository<Carteira>, ICarteiraRepository
    {
        public CarteiraRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Carteira> GetByJogadorIdAsync(int jogadorId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.JogadorId == jogadorId);
        }

        public async Task AtualizarSaldoAsync(int carteiraId, decimal novoSaldo)
        {
            var carteira = await _dbSet.FindAsync(carteiraId);
            if (carteira != null)
            {
                carteira.Saldo = novoSaldo;
                await _context.SaveChangesAsync();
            }
        }
    }
}