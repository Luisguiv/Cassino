using Microsoft.EntityFrameworkCore;
using ApiCassino.Data;
using ApiCassino.Models;
using ApiCassino.Repositories.Interfaces;

namespace ApiCassino.Repositories.Implementations
{
    public class JogadorRepository : Repository<Jogador>, IJogadorRepository
    {
        public JogadorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Jogador> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(j => j.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(j => j.Email == email);
        }

        public async Task<Jogador> GetWithCarteiraAsync(int id)
        {
            return await _dbSet
                .Include(j => j.Carteira)
                .FirstOrDefaultAsync(j => j.Id == id);
        }
    }
}