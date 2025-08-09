using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCassino.Models
{
    public class Carteira
    {
        public int Id { get; set; }
        
        public int JogadorId { get; set; }
        
        [Column(TypeName = "decimal(15,2)")]
        public decimal Saldo { get; set; } = 0;
        
        [StringLength(3)]
        public string Moeda { get; set; } = "BRL";
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        
        // Navigation Properties
        public virtual Jogador Jogador { get; set; }
        public virtual ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}