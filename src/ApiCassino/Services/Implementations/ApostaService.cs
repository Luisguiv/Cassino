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

            var jogador = await _jogadorRepository.GetByIdAsync(jogadorId);
            if (jogador == null)
            {
                throw new ArgumentException("Jogador não encontrado.");
            }

            // Criar aposta
            var aposta = new Aposta
            {
                JogadorId = jogadorId,
                Valor = apostaDto.Valor,
                Status = "Ativa",
                DataAposta = DateTime.UtcNow
            };

            await _apostaRepository.AddAsync(aposta);
            await _apostaRepository.SaveChangesAsync();

            // Debitar valor da carteira
            carteira.Saldo -= apostaDto.Valor;
            await _carteiraRepository.UpdateAsync(carteira);

            await _transacaoService.CriarTransacaoApostaAsync(carteira.Id, aposta.Id, apostaDto.Valor);

            // Processar resultado da aposta
            await ProcessarResultadoApostaAsync(aposta.Id);

            // Recarregar aposta após processamento para obter status/prêmio atualizados
            aposta = await _apostaRepository.GetByIdAsync(aposta.Id);

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
            if (carteira == null)
                return;

            // 30% de chance de ganhar
            bool ganhou = _random.NextDouble() <= 0.30;

            if (ganhou)
            {
                aposta.Status = "Ganha";
                aposta.ValorPremio = aposta.Valor * 2; // Prêmio de 2x

                // Creditar prêmio na carteira
                carteira.Saldo += aposta.ValorPremio.Value;
                await _carteiraRepository.UpdateAsync(carteira);

                await _transacaoService.CriarTransacaoPremioAsync(carteira.Id, aposta.Id, aposta.ValorPremio.Value);
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
            // Buscar as últimas apostas do jogador
            var ultimasApostas = await _apostaRepository.GetUltimasApostasAsync(jogadorId, 5);
            
            // Verificar se EXATAMENTE as últimas 5 são perdidas consecutivas
            if (ultimasApostas.Count() == 5 && ultimasApostas.All(a => a.Status == "Perdida"))
            {
                var ultimaTransacaoBonus = await _transacaoService.GetUltimaTransacaoBonusAsync(jogadorId);
                
                // Se nunca recebeu bônus OU a última foi antes das 5 apostas perdidas atuais
                bool podeReceberBonus = ultimaTransacaoBonus == null || 
                                    ((dynamic)ultimaTransacaoBonus).DataTransacao < ultimasApostas.Min(a => a.DataAposta);
                
                if (podeReceberBonus)
                {
                    // Calcular 10% do valor total gasto nas 5 apostas
                    decimal valorGasto = ultimasApostas.Sum(a => a.Valor);
                    decimal bonus = valorGasto * 0.10m;

                    // Creditar bônus na carteira
                    var carteira = await _carteiraRepository.GetByIdAsync(carteiraId);
                    if (carteira != null)
                    {
                        carteira.Saldo += bonus;
                        await _carteiraRepository.UpdateAsync(carteira);

                        // Criar transação de bônus
                        await _transacaoService.CriarTransacaoBonusAsync(carteiraId, bonus, 5);
                    }
                }
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
                NomeJogador = jogador?.Nome ?? "Jogador não encontrado"
            }).ToList();

            return new PagedResult<ApostaResponseDto>(apostasDto, totalApostas, page, pageSize);
        }

        public async Task<ApostaResponseDto> CancelarApostaAsync(int apostaId, int jogadorId)
        {
            var aposta = await _apostaRepository.GetByIdAsync(apostaId);
            if (aposta == null || aposta.JogadorId != jogadorId)
                throw new ArgumentException("Aposta não encontrada.");

            if (aposta.Status == "Cancelada")
                throw new InvalidOperationException("Esta aposta já foi cancelada anteriormente.");

            var carteira = await _carteiraRepository.GetByJogadorIdAsync(jogadorId);
            if (carteira == null)
                throw new ArgumentException("Carteira não encontrada.");

            string statusAnterior = aposta.Status;

            if (aposta.Status == "Ganha" && aposta.ValorPremio.HasValue && aposta.ValorPremio > 0)
            {
                // 1. Estornar valor da aposta (+)
                carteira.Saldo += aposta.Valor;
                
                // 2. Remover o prêmio (-) 
                carteira.Saldo -= aposta.ValorPremio.Value;
                
                await _carteiraRepository.UpdateAsync(carteira);

                // 3. Criar DUAS transações separadas para ficar claro
                
                // Transação 1: Estorno da aposta (+)
                await _transacaoService.CriarTransacaoAsync(
                    carteira.Id, 
                    "Cancelamento", 
                    aposta.Valor, // Positivo
                    $"Cancelamento da Aposta#{aposta.Id} - estorno"
                );
                
                // Transação 2: Remoção do prêmio (-)
                await _transacaoService.CriarTransacaoAsync(
                    carteira.Id, 
                    "Cancelamento", 
                    -aposta.ValorPremio.Value,
                    $"Cancelamento da Aposta#{aposta.Id} - remoção do prêmio"
                );
            }
            else
            {
                // Para apostas Ativa ou Perdida: apenas estornar valor
                decimal valorEstorno = aposta.Valor;
                carteira.Saldo += valorEstorno;
                await _carteiraRepository.UpdateAsync(carteira);

                await _transacaoService.CriarTransacaoCancelamentoAsync(carteira.Id, aposta.Id, valorEstorno, statusAnterior);
            }

            // Atualizar status da aposta
            aposta.Status = "Cancelada";
            await _apostaRepository.UpdateAsync(aposta);
            await _apostaRepository.SaveChangesAsync();

            var jogador = await _jogadorRepository.GetByIdAsync(aposta.JogadorId);

            return new ApostaResponseDto
            {
                Id = aposta.Id,
                Valor = aposta.Valor,
                Status = aposta.Status,
                ValorPremio = aposta.ValorPremio,
                DataAposta = aposta.DataAposta,
                NomeJogador = jogador?.Nome ?? "Jogador não encontrado"
            };
        }
    }
}