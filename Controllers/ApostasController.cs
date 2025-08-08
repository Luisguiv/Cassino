using Microsoft.AspNetCore.Mvc;
using ApiCassino.DTOs;
using ApiCassino.Services.Interfaces;
using ApiCassino.Helpers;

namespace ApiCassino.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApostasController : ControllerBase
    {
        private readonly IApostaService _apostaService;

        public ApostasController(IApostaService apostaService)
        {
            _apostaService = apostaService;
        }

        /// <summary>
        /// Criar nova aposta
        /// </summary>
        [HttpPost("jogador/{jogadorId}")]
        public async Task<ActionResult<ApostaResponseDto>> CriarAposta(int jogadorId, [FromBody] ApostaCreateDto apostaDto)
        {
            try
            {
                var aposta = await _apostaService.CriarApostaAsync(jogadorId, apostaDto);
                return Ok(aposta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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
        /// Consultar apostas do jogador
        /// </summary>
        [HttpGet("jogador/{jogadorId}")]
        public async Task<ActionResult<PagedResult<ApostaResponseDto>>> GetApostas(
            int jogadorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var apostas = await _apostaService.GetApostasJogadorAsync(jogadorId, page, pageSize);
                return Ok(apostas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Cancelar aposta
        /// </summary>
        [HttpPut("{apostaId}/cancelar/jogador/{jogadorId}")]
        public async Task<ActionResult<ApostaResponseDto>> CancelarAposta(int apostaId, int jogadorId)
        {
            try
            {
                var aposta = await _apostaService.CancelarApostaAsync(apostaId, jogadorId);
                return Ok(aposta);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
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
    }
}