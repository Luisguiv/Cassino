using Microsoft.AspNetCore.Mvc;
using ApiCassino.DTOs;
using ApiCassino.Services.Interfaces;
using ApiCassino.Helpers;

namespace ApiCassino.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;

        public TransacoesController(ITransacaoService transacaoService)
        {
            _transacaoService = transacaoService;
        }

        /// <summary>
        /// Consultar transações do jogador
        /// </summary>
        [HttpGet("jogador/{jogadorId}")]
        public async Task<ActionResult<PagedResult<TransacaoResponseDto>>> GetTransacoes(
            int jogadorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var transacoes = await _transacaoService.GetTransacoesAsync(jogadorId, page, pageSize);
                return Ok(transacoes);
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