using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCassino.Models
{
    public class Aposta
    {
        public int Id { get; set; }
        
        public int JogadorId { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        [Range(1.0, double.MaxValue, ErrorMessage = "O valor da aposta deve ser maior que R$ 1,00")]
        public decimal Valor { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Ativa";
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? ValorPremio { get; set; }
        
        public DateTime DataAposta { get; set; } = DateTime.Now;
        
        public virtual Jogador Jogador { get; set; }
    }
}