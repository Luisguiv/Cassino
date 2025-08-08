using ApiCassino.DTOs;
using ApiCassino.Models;
using ApiCassino.Repositories.Interfaces;
using ApiCassino.Services.Interfaces;
using ApiCassino.Helpers;

namespace ApiCassino.Services.Implementations
{
    public class ApostaService : IApostaService
    {
        private readonly IApostaRepository _apostaRepository;
        private readonly ICarteiraRepository _carteiraRepository;
        private readonly IJogadorRepository _jogadorRepository;
        private readonly ITransacaoService _transacaoService;
        private readonly Random _random;

        public ApostaService(
            IApostaRepository apostaRepository,
            ICarteiraRepository carteiraRepository,
            IJogadorRepository jogadorRepository,
            ITransacaoService transacaoService)
        {
            _apostaRepository = apostaRepository;
            _carteiraRepository = carteiraRepository;
            _jogadorRepository = jogadorRepository;
            _transacaoService = transacaoService;
            _random = new Random();
        }

        public async Task<ApostaResponseDto> CriarApostaAsync(int jogadorId, ApostaCreateDto apostaDto)
        {
            // Validações
            if (apostaDto.Valor < 1)
            {
                throw new ArgumentException("O valor da aposta deve ser maior que R$ 1,00");
            }

            var carteira = await _carteiraRepository.GetByJogadorIdAsync(jogadorId);
            if (carteira == null)
            {
                throw new ArgumentException("Carteira não encontrada.");
            }

            if (carteira.Saldo < apostaDto.Valor)
            {
                throw new InvalidOperationException("Saldo insuficiente para realizar a aposta.");
            }

            // Criar aposta
            var aposta = new Aposta
            {
                JogadorId = jogadorId,
                Valor = apostaDto.Valor,
                Status = "Ativa",
                DataAposta = DateTime.Now
            };

            await _apostaRepository.AddAsync(aposta);

            // Debitar valor da carteira
            carteira.Saldo -= apostaDto.Valor;
            await _carteiraRepository.UpdateAsync(carteira);

            // Registrar transação
            await _transacaoService.CriarTransacaoAsync(
                carteira.Id, 
                "Aposta", 
                -apostaDto.Valor, 
                $"Aposta #{aposta.Id}"
            );

            await _apostaRepository.SaveChangesAsync();

            // Processar resultado da aposta
            await ProcessarResultadoApostaAsync(aposta.Id);

            var jogador = await _jogadorRepository.GetByIdAsync(jogadorId);

            return new ApostaResponseDto
            {
                Id = aposta.Id,
                Valor = aposta.Valor,
                Status = aposta.Status,
                ValorPremio = aposta.ValorPremio,
                DataAposta = aposta.DataAposta,
                NomeJogador = jogador.Nome
            };
        }

        public async Task ProcessarResultadoApostaAsync(int apostaId)
        {
            var aposta = await _apostaRepository.GetByIdAsync(apostaId);
            if (aposta == null || aposta.Status != "Ativa")
                return;

            var carteira = await _carteiraRepository.GetByJogadorIdAsync(aposta.JogadorId);

            // 30% de chance de ganhar (você pode ajustar)
            bool ganhou = _random.NextDouble() <= 0.30;

            if (ganhou)
            {
                aposta.Status = "Ganha";
                aposta.ValorPremio = aposta.Valor * 2; // Prêmio de 2x

                // Creditar prêmio na carteira
                carteira.Saldo += aposta.ValorPremio.Value;
                await _carteiraRepository.UpdateAsync(carteira);

                // Registrar transação de prêmio
                await _transacaoService.CriarTransacaoAsync(
                    carteira.Id,
                    "Premio",
                    aposta.ValorPremio.Value,
                    $"Prêmio da aposta #{aposta.Id}"
                );
            }
            else
            {
                aposta.Status = "Perdida";
                
                // Verificar sistema de bônus (5 apostas perdidas consecutivas)
                await VerificarBonusConsecutivoAsync(aposta.JogadorId, carteira.Id);
            }

            await _apostaRepository.UpdateAsync(aposta);
            await _apostaRepository.SaveChangesAsync();
        }

        private async Task VerificarBonusConsecutivoAsync(int jogadorId, int carteiraId)
        {
            var ultimasApostas = await _apostaRepository.GetUltimasApostasPerdidasAsync(jogadorId, 5);
            
            if (ultimasApostas.Count() == 5)
            {
                // Calcular 10% do valor total gasto nas 5 apostas
                decimal valorGasto = ultimasApostas.Sum(a => a.Valor);
                decimal bonus = valorGasto * 0.10m;

                // Creditar bônus na carteira
                var carteira = await _carteiraRepository.GetByIdAsync(carteiraId);
                carteira.Saldo += bonus;
                await _carteiraRepository.UpdateAsync(carteira);

                // Registrar transação de bônus
                await _transacaoService.CriarTransacaoAsync(
                    carteiraId,
                    "Bonus",
                    bonus,
                    $"Bônus por 5 apostas perdidas consecutivas (10% de R$ {valorGasto:F2})"
                );
            }
        }

        public async Task<PagedResult<ApostaResponseDto>> GetApostasJogadorAsync(int jogadorId, int page = 1, int pageSize = 10)
        {
            var apostas = await _apostaRepository.GetByJogadorAsync(jogadorId, page, pageSize);
            var totalApostas = await _apostaRepository.CountByJogadorAsync(jogadorId);
            var jogador = await _jogadorRepository.GetByIdAsync(jogadorId);

            var apostasDto = apostas.Select(a => new ApostaResponseDto
            {
                Id = a.Id,
                Valor = a.Valor,
                Status = a.Status,
                ValorPremio = a.ValorPremio,
                DataAposta = a.DataAposta,
                NomeJogador = jogador.Nome
            });

            return new PagedResult<ApostaResponseDto>(apostasDto, totalApostas, page, pageSize);
        }

        public async Task<ApostaResponseDto> CancelarApostaAsync(int apostaId, int jogadorId)
        {
            var aposta = await _apostaRepository.GetByIdAsync(apostaId);
            
            if (aposta == null)
            {
                throw new ArgumentException("Aposta não encontrada.");
            }

            if (aposta.JogadorId != jogadorId)
            {
                throw new UnauthorizedAccessException("Você não pode cancelar esta aposta.");
            }

            if (aposta.Status != "Ativa")
            {
                throw new InvalidOperationException("Apenas apostas ativas podem ser canceladas.");
            }

            // Cancelar aposta
            aposta.Status = "Cancelada";
            await _apostaRepository.UpdateAsync(aposta);

            // Estornar valor para a carteira
            var carteira = await _carteiraRepository.GetByJogadorIdAsync(jogadorId);
            carteira.Saldo += aposta.Valor;
            await _carteiraRepository.UpdateAsync(carteira);

            // Registrar transação de cancelamento
            await _transacaoService.CriarTransacaoAsync(
                carteira.Id,
                "Cancelamento",
                aposta.Valor,
                $"Cancelamento da aposta #{aposta.Id}"
            );

            await _apostaRepository.SaveChangesAsync();

            var jogador = await _jogadorRepository.GetByIdAsync(jogadorId);

            return new ApostaResponseDto
            {
                Id = aposta.Id,
                Valor = aposta.Valor,
                Status = aposta.Status,
                ValorPremio = aposta.ValorPremio,
                DataAposta = aposta.DataAposta,
                NomeJogador = jogador.Nome
            };
        }
    }
}