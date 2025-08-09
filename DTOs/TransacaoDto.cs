namespace ApiCassino.DTOs
{
    public class TransacaoResponseDto
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataTransacao { get; set; }
        public string? Descricao { get; set; }
    }
}