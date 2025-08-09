using ApiCassino.DTOs;

namespace ApiCassino.Services.Interfaces
{
    public interface IAuthService
    {
        Task<JogadorResponseDto> RegistrarJogadorAsync(JogadorCreateDto jogadorDto);
        Task<JogadorResponseDto> LoginAsync(LoginDto loginDto);
    }
}