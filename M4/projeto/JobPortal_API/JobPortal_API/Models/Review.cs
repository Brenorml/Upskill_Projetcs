using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobPortal_API.Models
{
    public class Review
    {
        [Key]
        public int IdReview { get; set; }

        [ForeignKey("Empresa")]
        public int IdEmpresa { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        // Novos campos
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public string NomeUsuario { get; set; } = "Anônimo";

        // Propriedade de navegação
        public virtual Empresa Empresa { get; set; }
    }
}