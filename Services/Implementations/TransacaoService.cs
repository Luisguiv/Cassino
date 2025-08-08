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
            });

            return new PagedResult<TransacaoResponseDto>(transacoesDto, totalTransacoes, page, pageSize);
        }

        public async Task CriarTransacaoAsync(int carteiraId, string tipo, decimal valor, string descricao = null)
        {
            var transacao = new Transacao
            {
                CarteiraId = carteiraId,
                Tipo = tipo,
                Valor = valor,
                DataTransacao = DateTime.Now,
                Descricao = descricao
            };

            await _transacaoRepository.AddAsync(transacao);
            await _transacaoRepository.SaveChangesAsync();
        }
    }
}