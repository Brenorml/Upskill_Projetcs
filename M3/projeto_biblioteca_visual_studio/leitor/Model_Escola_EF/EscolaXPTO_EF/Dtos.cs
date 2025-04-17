using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscolaXPTO_EF.Dto
{
    public class Dtos
    {
        public class DevolucaoDTO
        {
            public int RequisicaoId { get; set; }
            public string TituloObra { get; set; }
            public string Autor { get; set; }
            public string Nucleo { get; set; }
            public DateOnly DataRequisicao { get; set; }
            public DateOnly? DataDevolucao { get; set; }
            public int ExemplarID { get; set; }
        }

        public class HistoricoRequisicaoDTO
        {
            public string Leitor { get; set; }
            public string Obra { get; set; }
            public string Nucleo { get; set; }
            public DateOnly DataRequisicao { get; set; }
            public string StatusRequisicao { get; set; }
        }

        public class ObraDisponivelDto
        {
            public string Obra { get; set; }
            public string Autor { get; set; }
            public int AnoPublicacao { get; set; }
            public string Tema { get; set; }
            public int QuantidadeDisponivel { get; set; }
            public string Nucleo { get; set; }
        }

        public class SituacaoLeitorDTO
        {
            public int RequisicaoId { get; set; }
            public string Leitor { get; set; }
            public string Obra { get; set; }
            public string Nucleo { get; set; }
            public DateOnly DataRequisicao { get; set; }
            public string StatusDevolucao { get; set; }
        }

        public class PerfilDTO
        {
            public string Nome { get; set; }
            public DateOnly DataNascimento { get; set; }
            public string Email { get; set; }
            public string Telefone { get; set; }
            public string Username { get; set; }
        }

        public class ObraDto
        {
            public int ObraID { get; set; }
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public int? AnoPublicacao { get; set; }
            public string Genero { get; set; }
            public string Descricao { get; set; }
            public string? ImagemBase64 { get; set; }
        }

        public class Imagem
        {
            public int ID_ObraID { get; set; } // Nome igual ao da tabela
            public byte[] Capa { get; set; }
        }

        public class NucleoDto
        {
            public int NucleoID { get; set; }
            public string Nome { get; set; }
        }

        public class CancelarInscricaoDTO
        {
            public string Id { get; set; }
            public string Nome { get; set; }
            public DateOnly FimInscrincao { get; set; }
            public List<DevolucaoDTO> ObraDevolvida { get; set; }
        }
    }
}
