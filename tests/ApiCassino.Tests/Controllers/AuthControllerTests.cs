using ApiCassino.Controllers;
using ApiCassino.Services.Interfaces;
using ApiCassino.DTOs;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace ApiCassino.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        #region Register Tests

        [Fact]
        public async Task Register_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var registerDto = new JogadorCreateDto
            {
                Nome = "João Teste",
                Email = "joao@teste.com",
                Senha = "MinhaSenh@123"
            };

            var responseDto = new JogadorResponseDto
            {
                Id = 1,
                Nome = "João Teste",
                Email = "joao@teste.com",
                SaldoCarteira = 1000m,
                DataCriacao = DateTime.UtcNow
            };

            _authServiceMock.Setup(x => x.RegistrarJogadorAsync(registerDto))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<JogadorResponseDto>(okResult.Value);
            Assert.Equal("João Teste", returnedDto.Nome);
            Assert.Equal("joao@teste.com", returnedDto.Email);
            Assert.Equal(1000m, returnedDto.SaldoCarteira);
        }

        [Fact]
        public async Task Register_ComEmailDuplicado_DeveRetornarBadRequest()
        {
            // Arrange
            var registerDto = new JogadorCreateDto
            {
                Nome = "João Teste",
                Email = "joao@teste.com",
                Senha = "MinhaSenh@123"
            };

            _authServiceMock.Setup(x => x.RegistrarJogadorAsync(registerDto))
                .ThrowsAsync(new ArgumentException("Email já está em uso."));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Register_ComDadosNulos_DeveRetornarBadRequest()
        {
            // Arrange
            JogadorCreateDto registerDto = null;

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_ComSenhaFraca_DeveRetornarBadRequest()
        {
            // Arrange
            var registerDto = new JogadorCreateDto
            {
                Nome = "João Teste",
                Email = "joao@teste.com",
                Senha = "123"  // Senha muito fraca
            };

            _authServiceMock.Setup(x => x.RegistrarJogadorAsync(registerDto))
                .ThrowsAsync(new ArgumentException("Senha deve ter pelo menos 6 caracteres."));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarOk()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "joao@teste.com",
                Senha = "MinhaSenh@123"
            };

            var responseDto = new JogadorResponseDto
            {
                Id = 1,
                Nome = "João Teste",
                Email = "joao@teste.com",
                SaldoCarteira = 850m,
                DataCriacao = DateTime.UtcNow.AddDays(-10)
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<JogadorResponseDto>(okResult.Value);
            Assert.Equal("João Teste", returnedDto.Nome);
            Assert.Equal("joao@teste.com", returnedDto.Email);
        }

        [Fact]
        public async Task Login_ComCredenciaisInvalidas_DeveRetornarBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "joao@teste.com",
                Senha = "senhaerrada"
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new ArgumentException("Credenciais inválidas."));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ComEmailInexistente_DeveRetornarBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "inexistente@teste.com",
                Senha = "MinhaSenh@123"
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new ArgumentException("Usuário não encontrado."));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ComDadosNulos_DeveRetornarBadRequest()
        {
            // Arrange
            LoginDto loginDto = null;

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task Register_ComEmailInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var registerDto = new JogadorCreateDto
            {
                Nome = "João Teste",
                Email = "email-invalido",  // Email inválido
                Senha = "MinhaSenh@123"
            };

            _authServiceMock.Setup(x => x.RegistrarJogadorAsync(registerDto))
                .ThrowsAsync(new ArgumentException("Email inválido."));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_ComNomeVazio_DeveRetornarBadRequest()
        {
            // Arrange
            var registerDto = new JogadorCreateDto
            {
                Nome = "",  // Nome vazio
                Email = "joao@teste.com",
                Senha = "MinhaSenh@123"
            };

            _authServiceMock.Setup(x => x.RegistrarJogadorAsync(registerDto))
                .ThrowsAsync(new ArgumentException("Nome é obrigatório."));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Business Logic Tests

        [Fact]
        public async Task Register_SempreDeveCriarComSaldoInicial1000()
        {
            // Arrange
            var registerDto = new JogadorCreateDto
            {
                Nome = "Novo Jogador",
                Email = "novo@teste.com",
                Senha = "MinhaSenh@123"
            };

            var responseDto = new JogadorResponseDto
            {
                Id = 2,
                Nome = "Novo Jogador",
                Email = "novo@teste.com",
                SaldoCarteira = 1000m,  // Saldo inicial padrão
                DataCriacao = DateTime.UtcNow
            };

            _authServiceMock.Setup(x => x.RegistrarJogadorAsync(registerDto))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<JogadorResponseDto>(okResult.Value);
            Assert.Equal(1000m, returnedDto.SaldoCarteira);
        }

        [Fact]
        public async Task Login_DeveRetornarSaldoAtualizado()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "jogador@teste.com",
                Senha = "MinhaSenh@123"
            };

            var responseDto = new JogadorResponseDto
            {
                Id = 1,
                Nome = "Jogador Existente",
                Email = "jogador@teste.com",
                SaldoCarteira = 750m,  // Saldo pode ter mudado
                DataCriacao = DateTime.UtcNow.AddDays(-5)
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<JogadorResponseDto>(okResult.Value);
            Assert.True(returnedDto.SaldoCarteira >= 0);  // Saldo não pode ser negativo
        }

        #endregion
    }
}