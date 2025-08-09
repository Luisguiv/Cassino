using ApiCassino.Models;

namespace ApiCassino.Repositories.Interfaces
{
    public interface ICarteiraRepository : IRepository<Carteira>
    {
        Task<Carteira> GetByJogadorIdAsync(int jogadorId);
        Task AtualizarSaldoAsync(int carteiraId, decimal novoSaldo);
    }
}