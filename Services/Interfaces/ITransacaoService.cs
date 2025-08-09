using ApiCassino.DTOs;
using ApiCassino.Helpers;

namespace ApiCassino.Services.Interfaces
{
    public interface ITransacaoService
    {
        Task<PagedResult<TransacaoResponseDto>> GetTransacoesAsync(int jogadorId, int page = 1, int pageSize = 10);
        Task CriarTransacaoAsync(int carteiraId, string tipo, decimal valor, string? descricao = null);

        // Novos métodos especializados
        Task<object?> GetUltimaTransacaoBonusAsync(int jogadorId);
        Task CriarTransacaoApostaAsync(int carteiraId, int apostaId, decimal valor);
        Task CriarTransacaoPremioAsync(int carteiraId, int apostaId, decimal valorPremio);
        Task CriarTransacaoCancelamentoAsync(int carteiraId, int apostaId, decimal valorEstorno, string statusAnterior);
        Task CriarTransacaoBonusAsync(int carteiraId, decimal valorBonus, int quantidadeApostasConsecutivas);
    }
}