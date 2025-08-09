using ApiCassino.Services.Interfaces;
using ApiCassino.Repositories.Interfaces;
using ApiCassino.DTOs;
using ApiCassino.Models;
using System.Security.Cryptography;
using System.Text;

namespace ApiCassino.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IJogadorRepository _jogadorRepository;
        private readonly ICarteiraRepository _carteiraRepository;

        public AuthService(IJogadorRepository jogadorRepository, ICarteiraRepository carteiraRepository)
        {
            _jogadorRepository = jogadorRepository;
            _carteiraRepository = carteiraRepository;
        }

        public async Task<JogadorResponseDto> RegistrarJogadorAsync(JogadorCreateDto jogadorDto)
        {
            // Validar se email já existe
            var emailExiste = await _jogadorRepository.EmailExistsAsync(jogadorDto.Email);
            if (emailExiste)
            {
                throw new ArgumentException("Email já está em uso.");
            }

            // Validações básicas
            if (string.IsNullOrWhiteSpace(jogadorDto.Nome))
                throw new ArgumentException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(jogadorDto.Email) || !IsValidEmail(jogadorDto.Email))
                throw new ArgumentException("Email inválido.");

            if (string.IsNullOrWhiteSpace(jogadorDto.Senha) || jogadorDto.Senha.Length < 6)
                throw new ArgumentException("Senha deve ter pelo menos 6 caracteres.");

            // Criar jogador
            var jogador = new Jogador
            {
                Nome = jogadorDto.Nome,
                Email = jogadorDto.Email,
                Senha = HashPassword(jogadorDto.Senha),
                DataCriacao = DateTime.UtcNow
            };

            // ✅ CORREÇÃO: Usar AddAsync em vez de CreateAsync
            await _jogadorRepository.AddAsync(jogador);
            await _jogadorRepository.SaveChangesAsync();

            // Criar carteira com saldo inicial de R$ 1000
            var carteira = new Carteira
            {
                JogadorId = jogador.Id,
                Saldo = 1000m
            };

            // ✅ CORREÇÃO: Usar AddAsync em vez de CreateAsync
            await _carteiraRepository.AddAsync(carteira);
            await _carteiraRepository.SaveChangesAsync();

            // Retornar DTO
            return new JogadorResponseDto
            {
                Id = jogador.Id,
                Nome = jogador.Nome,
                Email = jogador.Email,
                SaldoCarteira = carteira.Saldo,
                DataCriacao = jogador.DataCriacao
            };
        }

        public async Task<JogadorResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Senha))
                throw new ArgumentException("Email e senha são obrigatórios.");

            // Buscar jogador por email
            var jogador = await _jogadorRepository.GetByEmailAsync(loginDto.Email);
            if (jogador == null)
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            // Verificar senha
            if (!VerifyPassword(loginDto.Senha, jogador.Senha))
            {
                throw new ArgumentException("Credenciais inválidas.");
            }

            // Buscar carteira para retornar saldo atual
            var carteira = await _carteiraRepository.GetByJogadorIdAsync(jogador.Id);

            return new JogadorResponseDto
            {
                Id = jogador.Id,
                Nome = jogador.Nome,
                Email = jogador.Email,
                SaldoCarteira = carteira?.Saldo ?? 0m,
                DataCriacao = jogador.DataCriacao
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}