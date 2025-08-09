using ApiCassino.Services.Implementations;
using ApiCassino.Repositories.Interfaces;
using ApiCassino.Models;
using ApiCassino.DTOs;
using ApiCassino.Helpers;
using Moq;
using Xunit;

namespace ApiCassino.Tests.Services
{
    public class TransacaoServiceTests
    {
        private readonly Mock<ITransacaoRepository> _transacaoRepositoryMock;
        private readonly Mock<ICarteiraRepository> _carteiraRepositoryMock;
        private readonly TransacaoService _transacaoService;

        public TransacaoServiceTests()
        {
            _transacaoRepositoryMock = new Mock<ITransacaoRepository>();
            _carteiraRepositoryMock = new Mock<ICarteiraRepository>();
            _transacaoService = new TransacaoService(
                _transacaoRepositoryMock.Object,
                _carteiraRepositoryMock.Object);
        }

        [Fact]
        public async Task GetTransacoesAsync_ComCarteiraInexistente_DeveLancarArgumentException()
        {
            // Arrange
            var jogadorId = 1;
            _carteiraRepositoryMock.Setup(x => x.GetByJogadorIdAsync(jogadorId))
                .ReturnsAsync((Carteira?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _transacaoService.GetTransacoesAsync(jogadorId));
            
            Assert.Equal("Carteira não encontrada.", exception.Message);
        }

        [Fact]
        public async Task CriarTransacaoApostaAsync_DeveUsarValorNegativo()
        {
            // Arrange
            var carteiraId = 1;
            var apostaId = 1;
            var valor = 25m;

            // Act
            await _transacaoService.CriarTransacaoApostaAsync(carteiraId, apostaId, valor);

            // Assert
            _transacaoRepositoryMock.Verify(x => x.AddAsync(It.Is<Transacao>(t => 
                t.CarteiraId == carteiraId &&
                t.Tipo == "Aposta" &&
                t.Valor == -25m &&
                t.Descricao == "Aposta#1"
            )), Times.Once);
        }

        [Fact]
        public async Task CriarTransacaoPremioAsync_DeveUsarValorPositivo()
        {
            // Arrange
            var carteiraId = 1;
            var apostaId = 2;
            var valorPremio = 50m;

            // Act
            await _transacaoService.CriarTransacaoPremioAsync(carteiraId, apostaId, valorPremio);

            // Assert
            _transacaoRepositoryMock.Verify(x => x.AddAsync(It.Is<Transacao>(t => 
                t.CarteiraId == carteiraId &&
                t.Tipo == "Premio" &&
                t.Valor == 50m &&
                t.Descricao == "Prêmio da Aposta#2"
            )), Times.Once);
        }
    }
}