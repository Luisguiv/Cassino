using ApiCassino.DTOs;
using ApiCassino.Helpers;

namespace ApiCassino.Services.Interfaces
{
    public interface ITransacaoService
    {
        Task<PagedResult<TransacaoResponseDto>> GetTransacoesAsync(int jogadorId, int page = 1, int pageSize = 10);
        Task CriarTransacaoAsync(int carteiraId, string tipo, decimal valor, string descricao = null);
    }
}