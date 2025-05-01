using System.ComponentModel.DataAnnotations;

namespace JobPortal_API.DTOs
{
    public class ReviewDTO
    {
        public int IdEmpresa { get; set; }        
        public string Titulo { get; set; }
        [Required]
        public string Descricao { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string NomeUsuario { get; set; } = "Anônimo";
        public DateTime DataCriacao { get; set; }
    }
}
