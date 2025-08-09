using ApiCassino.Models;

namespace ApiCassino.Repositories.Interfaces
{
    public interface ITransacaoRepository
    {
        Task<IEnumerable<Transacao>> GetByCarteiraAsync(int carteiraId, int page, int pageSize);
        Task<int> CountByCarteiraAsync(int carteiraId);
        Task<IEnumerable<Transacao>> GetAllByCarteiraAsync(int carteiraId); // ← Adicionar se não tiver
        Task<Transacao?> GetUltimaTransacaoBonusAsync(int carteiraId);
        Task AddAsync(Transacao transacao);
        Task SaveChangesAsync();
        Task<Transacao?> GetByIdAsync(int id);
        Task UpdateAsync(Transacao transacao);
        Task DeleteAsync(int id);
    }
}
