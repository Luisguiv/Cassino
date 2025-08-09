using ApiCassino.DTOs;
using ApiCassino.Models;
using ApiCassino.Repositories.Interfaces;
using ApiCassino.Services.Interfaces;
using ApiCassino.Helpers;

namespace ApiCassino.Services.Implementations
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly ICarteiraRepository _carteiraRepository;

        public TransacaoService(ITransacaoRepository transacaoRepository, ICarteiraRepository carteiraRepository)
        {
            _transacaoRepository = transacaoRepository;
            _carteiraRepository = carteiraRepository;
        }

        public async Task<PagedResult<TransacaoResponseDto>> GetTransacoesAsync(int jogadorId, int page = 1, int pageSize = 10)
        {
            var carteira = await _carteiraRepository.GetByJogadorIdAsync(jogadorId);
            if (carteira == null)
            {
                throw new ArgumentException("Carteira não encontrada.");
            }

            var transacoes = await _transacaoRepository.GetByCarteiraAsync(carteira.Id, page, pageSize);
            var totalTransacoes = await _transacaoRepository.CountByCarteiraAsync(carteira.Id);

            var transacoesDto = transacoes.Select(t => new TransacaoResponseDto
            {
                Id = t.Id,
                Tipo = t.Tipo,
                Valor = t.Valor,
                DataTransacao = t.DataTransacao,
                Descricao = t.Descricao
            }).ToList();

            return new PagedResult<TransacaoResponseDto>(transacoesDto, totalTransacoes, page, pageSize);
        }

        public async Task<object?> GetUltimaTransacaoBonusAsync(int jogadorId)
        {
            var carteira = await _carteiraRepository.GetByJogadorIdAsync(jogadorId);
            if (carteira == null)
                return null;

            // Buscar última transação de bônus
            var ultimaTransacaoBonus = await _transacaoRepository.GetUltimaTransacaoBonusAsync(carteira.Id);
            
            return ultimaTransacaoBonus != null ? new 
            {
                Id = ultimaTransacaoBonus.Id,
                DataTransacao = ultimaTransacaoBonus.DataTransacao,
                Valor = ultimaTransacaoBonus.Valor
            } : null;
        }

        public async Task CriarTransacaoAsync(int carteiraId, string tipo, decimal valor, string? descricao = null)
        {
            var transacao = new Transacao
            {
                CarteiraId = carteiraId,
                Tipo = tipo,
                Valor = valor,
                DataTransacao = DateTime.UtcNow, // Mudei para UTC
                Descricao = descricao
            };

            await _transacaoRepository.AddAsync(transacao);
            await _transacaoRepository.SaveChangesAsync();
        }

        public async Task CriarTransacaoApostaAsync(int carteiraId, int apostaId, decimal valor)
        {
            await CriarTransacaoAsync(
                carteiraId, 
                "Aposta", 
                -Math.Abs(valor), // Garantir que seja negativo
                $"Aposta#{apostaId}"
            );
        }

        public async Task CriarTransacaoPremioAsync(int carteiraId, int apostaId, decimal valorPremio)
        {
            await CriarTransacaoAsync(
                carteiraId, 
                "Premio", 
                Math.Abs(valorPremio), // Garantir que seja positivo
                $"Prêmio da Aposta#{apostaId}"
            );
        }

        public async Task CriarTransacaoCancelamentoAsync(int carteiraId, int apostaId, decimal valorEstorno, string statusAnterior)
        {
            string descricao = statusAnterior switch
            {
                "Ativa" => $"Cancelamento da Aposta#{apostaId}",
                "Ganha" => $"Cancelamento da Aposta#{apostaId} (ganha) - prêmio removido",
                "Perdida" => $"Cancelamento da Aposta#{apostaId} (perdida)",
                _ => $"Cancelamento da Aposta#{apostaId}"
            };

            await CriarTransacaoAsync(
                carteiraId, 
                "Cancelamento", 
                Math.Abs(valorEstorno), // Garantir que seja positivo
                descricao
            );
        }

        public async Task CriarTransacaoBonusAsync(int carteiraId, decimal valorBonus, int quantidadeApostasConsecutivas)
        {
            await CriarTransacaoAsync(
                carteiraId, 
                "Bonus", 
                Math.Abs(valorBonus), // Garantir que seja positivo
                $"Bônus por {quantidadeApostasConsecutivas} apostas perdidas consecutivas"
            );
        }
    }
}