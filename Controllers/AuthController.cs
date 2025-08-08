using Microsoft.AspNetCore.Mvc;
using ApiCassino.DTOs;
using ApiCassino.Services.Interfaces;

namespace ApiCassino.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJogadorService _jogadorService;

        public AuthController(IJogadorService jogadorService)
        {
            _jogadorService = jogadorService;
        }

        /// <summary>
        /// Registrar novo jogador
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<JogadorResponseDto>> Register([FromBody] JogadorCreateDto jogadorDto)
        {
            try
            {
                var jogador = await _jogadorService.RegistrarJogadorAsync(jogadorDto);
                return Ok(jogador);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Fazer login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<JogadorResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var jogador = await _jogadorService.LoginAsync(loginDto);
                return Ok(jogador);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}