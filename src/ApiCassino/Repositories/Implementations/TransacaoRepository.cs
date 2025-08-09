using ApiCassino.Data;
using ApiCassino.Models;
using ApiCassino.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiCassino.Repositories.Implementations
{
    public class TransacaoRepository : ITransacaoRepository
    {
        private readonly ApplicationDbContext _context;

        public TransacaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transacao>> GetByCarteiraAsync(int carteiraId, int page, int pageSize)
        {
            return await _context.Transacoes
                .Where(t => t.CarteiraId == carteiraId)
                .OrderByDescending(t => t.DataTransacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByCarteiraAsync(int carteiraId)
        {
            return await _context.Transacoes
                .Where(t => t.CarteiraId == carteiraId)
                .CountAsync();
        }

        public async Task<IEnumerable<Transacao>> GetAllByCarteiraAsync(int carteiraId)
        {
            return await _context.Transacoes
                .Where(t => t.CarteiraId == carteiraId)
                .OrderByDescending(t => t.DataTransacao)
                .ToListAsync();
        }

        public async Task AddAsync(Transacao transacao)
        {
            await _context.Transacoes.AddAsync(transacao);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Transacao?> GetByIdAsync(int id)
        {
            return await _context.Transacoes.FindAsync(id);
        }

        public Task UpdateAsync(Transacao transacao)
        {
            _context.Transacoes.Update(transacao);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var transacao = await GetByIdAsync(id);
            if (transacao != null)
            {
                _context.Transacoes.Remove(transacao);
            }
        }

        public async Task<Transacao?> GetUltimaTransacaoBonusAsync(int carteiraId)
        {
            return await _context.Transacoes
                .Where(t => t.CarteiraId == carteiraId && t.Tipo == "Bonus")
                .OrderByDescending(t => t.DataTransacao)
                .FirstOrDefaultAsync();
        }
    }
}
