using ApiCassino.Models;

namespace ApiCassino.Repositories.Interfaces
{
    public interface IApostaRepository : IRepository<Aposta>
    {
        Task<IEnumerable<Aposta>> GetByJogadorAsync(int jogadorId, int page = 1, int pageSize = 10);
        Task<IEnumerable<Aposta>> GetUltimasApostasPerdidasAsync(int jogadorId, int quantidade = 5);
        Task<int> CountByJogadorAsync(int jogadorId);
    }
}