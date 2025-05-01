using System;
using System.ComponentModel.DataAnnotations;

namespace teste_cliente.Models
{
    public class Review
    {
        public int IdReview { get; set; }
        public int IdEmpresa { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime DataCriacao { get; set; }
        public string NomeUsuario { get; set; }

        
    }
}