using ApiCassino.Models;

namespace ApiCassino.Repositories.Interfaces
{
    public interface ITransacaoRepository : IRepository<Transacao>
    {
        Task<IEnumerable<Transacao>> GetByCarteiraAsync(int carteiraId, int page = 1, int pageSize = 10);
        Task<int> CountByCarteiraAsync(int carteiraId);
    }
}