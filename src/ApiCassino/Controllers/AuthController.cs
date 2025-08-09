using ApiCassino.Services.Interfaces;
using ApiCassino.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiCassino.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] JogadorCreateDto jogadorDto)
        {
            if (jogadorDto == null)
            {
                return BadRequest(new { message = "Dados de registro são obrigatórios." });
            }

            try
            {
                var result = await _authService.RegistrarJogadorAsync(jogadorDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest(new { message = "Dados de login são obrigatórios." });
            }

            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor." });
            }
        }
    }
}