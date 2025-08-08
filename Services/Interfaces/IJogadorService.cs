using ApiCassino.DTOs;
using ApiCassino.Models;

namespace ApiCassino.Services.Interfaces
{
    public interface IJogadorService
    {
        Task<JogadorResponseDto> RegistrarJogadorAsync(JogadorCreateDto jogadorDto);
        Task<JogadorResponseDto> LoginAsync(LoginDto loginDto);
        Task<JogadorResponseDto> GetPerfilAsync(int jogadorId);
    }
}