using Microsoft.EntityFrameworkCore;
using ApiCassino.Data;
using ApiCassino.Models;
using ApiCassino.Repositories.Interfaces;

namespace ApiCassino.Repositories.Implementations
{
    public class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
    {
        public TransacaoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transacao>> GetByCarteiraAsync(int carteiraId, int page = 1, int pageSize = 10)
        {
            return await _dbSet
                .Where(t => t.CarteiraId == carteiraId)
                .OrderByDescending(t => t.DataTransacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByCarteiraAsync(int carteiraId)
        {
            return await _dbSet.CountAsync(t => t.CarteiraId == carteiraId);
        }
    }
}