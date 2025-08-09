namespace ApiCassino.DTOs
{
    public class ApostaCreateDto
    {
        public decimal Valor { get; set; }
    }

    public class ApostaResponseDto
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public decimal? ValorPremio { get; set; }
        public DateTime DataAposta { get; set; }
        public string NomeJogador { get; set; }
    }
}