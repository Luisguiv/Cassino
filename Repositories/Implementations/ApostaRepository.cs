using Microsoft.EntityFrameworkCore;
using ApiCassino.Data;
using ApiCassino.Models;
using ApiCassino.Repositories.Interfaces;

namespace ApiCassino.Repositories.Implementations
{
    public class ApostaRepository : Repository<Aposta>, IApostaRepository
    {
        public ApostaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Aposta>> GetByJogadorAsync(int jogadorId, int page = 1, int pageSize = 10)
        {
            return await _dbSet
                .Where(a => a.JogadorId == jogadorId)
                .OrderByDescending(a => a.DataAposta)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Aposta>> GetUltimasApostasPerdidasAsync(int jogadorId, int quantidade = 5)
        {
            return await _dbSet
                .Where(a => a.JogadorId == jogadorId && a.Status == "Perdida")
                .OrderByDescending(a => a.DataAposta)
                .Take(quantidade)
                .ToListAsync();
        }

        public async Task<int> CountByJogadorAsync(int jogadorId)
        {
            return await _dbSet.CountAsync(a => a.JogadorId == jogadorId);
        }
    }
}