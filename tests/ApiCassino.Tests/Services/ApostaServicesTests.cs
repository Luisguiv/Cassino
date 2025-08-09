using ApiCassino.Services.Implementations;
using ApiCassino.Repositories.Interfaces;
using ApiCassino.Services.Interfaces;
using ApiCassino.DTOs;
using ApiCassino.Models;
using Moq;
using Xunit;

namespace ApiCassino.Tests.Services
{
    public class ApostaServiceTests
    {
        private readonly Mock<IApostaRepository> _apostaRepositoryMock;
        private readonly Mock<ICarteiraRepository> _carteiraRepositoryMock;
        private readonly Mock<IJogadorRepository> _jogadorRepositoryMock;
        private readonly Mock<ITransacaoService> _transacaoServiceMock;
        private readonly ApostaService _apostaService;

        public ApostaServiceTests()
        {
            _apostaRepositoryMock = new Mock<IApostaRepository>();
            _carteiraRepositoryMock = new Mock<ICarteiraRepository>();
            _jogadorRepositoryMock = new Mock<IJogadorRepository>();
            _transacaoServiceMock = new Mock<ITransacaoService>();
            _apostaService = new ApostaService(
                _apostaRepositoryMock.Object,
                _carteiraRepositoryMock.Object,
                _jogadorRepositoryMock.Object,
                _transacaoServiceMock.Object);
        }

        [Fact]
        public async Task CriarApostaAsync_ComValorMenorQue1_DeveLancarArgumentException()
        {
            // Arrange
            var apostaDto = new ApostaCreateDto { Valor = 0.50m };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _apostaService.CriarApostaAsync(1, apostaDto));
            
            Assert.Equal("O valor da aposta deve ser maior que R$ 1,00", exception.Message);
        }

        [Fact]
        public async Task CriarApostaAsync_ComSaldoInsuficiente_DeveLancarInvalidOperationException()
        {
            // Arrange
            var jogadorId = 1;
            var apostaDto = new ApostaCreateDto { Valor = 100m };
            var carteira = new Carteira { Id = 1, JogadorId = jogadorId, Saldo = 50m };

            _carteiraRepositoryMock.Setup(x => x.GetByJogadorIdAsync(jogadorId))
                .ReturnsAsync(carteira);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _apostaService.CriarApostaAsync(jogadorId, apostaDto));
            
            Assert.Equal("Saldo insuficiente para realizar a aposta.", exception.Message);
        }

        [Fact]
        public async Task CancelarApostaAsync_ComApostaCancelada_DeveLancarInvalidOperationException()
        {
            // Arrange
            var apostaId = 1;
            var jogadorId = 1;
            var aposta = new Aposta { Id = apostaId, JogadorId = jogadorId, Status = "Cancelada" };

            _apostaRepositoryMock.Setup(x => x.GetByIdAsync(apostaId))
                .ReturnsAsync(aposta);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _apostaService.CancelarApostaAsync(apostaId, jogadorId));
            
            Assert.Equal("Esta aposta já foi cancelada anteriormente.", exception.Message);
        }

        [Fact]
        public async Task CancelarApostaAsync_ComApostaAtiva_DeveFuncionarCorretamente()
        {
            // Arrange
            var apostaId = 1;
            var jogadorId = 1;
            var aposta = new Aposta 
            { 
                Id = apostaId, 
                JogadorId = jogadorId, 
                Status = "Ativa", 
                Valor = 25m 
            };
            var carteira = new Carteira { Id = 1, JogadorId = jogadorId, Saldo = 75m };
            var jogador = new Jogador { Id = jogadorId, Nome = "Test User" };

            _apostaRepositoryMock.Setup(x => x.GetByIdAsync(apostaId))
                .ReturnsAsync(aposta);
            _carteiraRepositoryMock.Setup(x => x.GetByJogadorIdAsync(jogadorId))
                .ReturnsAsync(carteira);
            _jogadorRepositoryMock.Setup(x => x.GetByIdAsync(jogadorId))
                .ReturnsAsync(jogador);

            // Act
            var result = await _apostaService.CancelarApostaAsync(apostaId, jogadorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Cancelada", result.Status);
            _carteiraRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Carteira>()), Times.Once);
        }
    }
}