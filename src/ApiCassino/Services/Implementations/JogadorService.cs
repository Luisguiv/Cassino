using ApiCassino.DTOs;
using ApiCassino.Models;
using ApiCassino.Repositories.Interfaces;
using ApiCassino.Services.Interfaces;

namespace ApiCassino.Services.Implementations
{
    public class JogadorService : IJogadorService
    {
        private readonly IJogadorRepository _jogadorRepository;
        private readonly ICarteiraRepository _carteiraRepository;

        public JogadorService(IJogadorRepository jogadorRepository, ICarteiraRepository carteiraRepository)
        {
            _jogadorRepository = jogadorRepository;
            _carteiraRepository = carteiraRepository;
        }

        public async Task<JogadorResponseDto> RegistrarJogadorAsync(JogadorCreateDto jogadorDto)
        {
            // Verificar se email já existe
            if (await _jogadorRepository.EmailExistsAsync(jogadorDto.Email))
            {
                throw new InvalidOperationException("Email já está em uso.");
            }

            // Criar jogador
            var jogador = new Jogador
            {
                Nome = jogadorDto.Nome,
                Email = jogadorDto.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(jogadorDto.Senha), // Hash da senha
                DataCriacao = DateTime.Now
            };

            await _jogadorRepository.AddAsync(jogador);
            await _jogadorRepository.SaveChangesAsync();

            // Criar carteira para o jogador
            var carteira = new Carteira
            {
                JogadorId = jogador.Id,
                Saldo = 1000, // Saldo inicial de R$ 1000
                Moeda = "BRL",
                DataCriacao = DateTime.Now
            };

            await _carteiraRepository.AddAsync(carteira);
            await _carteiraRepository.SaveChangesAsync();

            return new JogadorResponseDto
            {
                Id = jogador.Id,
                Nome = jogador.Nome,
                Email = jogador.Email,
                DataCriacao = jogador.DataCriacao,
                SaldoCarteira = carteira.Saldo
            };
        }

        public async Task<JogadorResponseDto> LoginAsync(LoginDto loginDto)
        {
            var jogador = await _jogadorRepository.GetByEmailAsync(loginDto.Email);
            
            if (jogador == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, jogador.Senha))
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos.");
            }

            var carteira = await _carteiraRepository.GetByJogadorIdAsync(jogador.Id);

            return new JogadorResponseDto
            {
                Id = jogador.Id,
                Nome = jogador.Nome,
                Email = jogador.Email,
                DataCriacao = jogador.DataCriacao,
                SaldoCarteira = carteira?.Saldo ?? 0
            };
        }

        public async Task<JogadorResponseDto> GetPerfilAsync(int jogadorId)
        {
            var jogador = await _jogadorRepository.GetWithCarteiraAsync(jogadorId);
            
            if (jogador == null)
            {
                throw new ArgumentException("Jogador não encontrado.");
            }

            return new JogadorResponseDto
            {
                Id = jogador.Id,
                Nome = jogador.Nome,
                Email = jogador.Email,
                DataCriacao = jogador.DataCriacao,
                SaldoCarteira = jogador.Carteira?.Saldo ?? 0
            };
        }
    }
}