namespace JobPortal_API.DTOs
{
    public class AplicacaoTrabalhoDTO
    {
        public int IdAplicacao { get; set; }

        public int IdOferta { get; set; }
      
        public int IdCandidato { get; set; }
        public DateTime DataAplicacao { get; set; } = DateTime.Now;

        public string? aplicacaoAceite { get; set; } = null;
    }
}
