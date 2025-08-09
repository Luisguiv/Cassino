using System.ComponentModel.DataAnnotations;

namespace ApiCassino.Models
{
    public class Jogador
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Senha { get; set; }
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        
        public virtual Carteira? Carteira { get; set; }
        public virtual ICollection<Aposta> Apostas { get; set; } = new List<Aposta>();
    }
}