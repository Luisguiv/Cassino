using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCassino.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        
        public int CarteiraId { get; set; }
        
        [StringLength(20)]
        public string Tipo { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }
        
        public DateTime DataTransacao { get; set; } = DateTime.Now;
        
        [StringLength(200)]
        public string? Descricao { get; set; }
        
        // Navigation Properties
        public virtual Carteira Carteira { get; set; }
    }
}