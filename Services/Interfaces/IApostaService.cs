using ApiCassino.DTOs;
using ApiCassino.Helpers;

namespace ApiCassino.Services.Interfaces
{
    public interface IApostaService
    {
        Task<ApostaResponseDto> CriarApostaAsync(int jogadorId, ApostaCreateDto apostaDto);
        Task<PagedResult<ApostaResponseDto>> GetApostasJogadorAsync(int jogadorId, int page = 1, int pageSize = 10);
        Task<ApostaResponseDto> CancelarApostaAsync(int apostadorId, int jogadorId);
        Task ProcessarResultadoApostaAsync(int apostaId);
    }
}
