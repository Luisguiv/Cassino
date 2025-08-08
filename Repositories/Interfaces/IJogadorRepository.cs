using ApiCassino.Models;

namespace ApiCassino.Repositories.Interfaces
{
    public interface IJogadorRepository : IRepository<Jogador>
    {
        Task<Jogador> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<Jogador> GetWithCarteiraAsync(int id);
    }
}