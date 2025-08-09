using Microsoft.AspNetCore.Mvc;
using ApiCassino.DTOs;
using ApiCassino.Services.Interfaces;

namespace ApiCassino.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JogadorController : ControllerBase
    {
        private readonly IJogadorService _jogadorService;

        public JogadorController(IJogadorService jogadorService)
        {
            _jogadorService = jogadorService;
        }

        /// <summary>
        /// Obter perfil do jogador
        /// </summary>
        [HttpGet("{id}/perfil")]
        public async Task<ActionResult<JogadorResponseDto>> GetPerfil(int id)
        {
            try
            {
                var jogador = await _jogadorService.GetPerfilAsync(id);
                return Ok(jogador);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}