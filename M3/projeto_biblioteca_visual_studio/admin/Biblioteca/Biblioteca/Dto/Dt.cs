using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Dt
{
        public class ObraAtualizacaoDto
        {
            public int ObraID { get; set; }
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public int? AnoPublicacao { get; set; }
            public string Genero { get; set; }
            public string Descricao { get; set; }
            public string ImagemBase64 { get; set; }
            //public string CaminhoImagem { get; set; }
        }

        public class ObraDto
        {
            public int ObraID { get; set; }
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public int? AnoPublicacao { get; set; }
            public string Genero { get; set; }
            public string Descricao { get; set; }
            public string ImagemBase64 { get; set; }
        }

        public class GeneroObraDTO
        {
            public string Genero { get; set; }
            public int NumeroObras { get; set; }
        }

        public class DevolucaoDTO
        {
            public string TituloObra { get; set; }
            public string Autor { get; set; }
            public string Nucleo { get; set; }
            public DateTime DataRequisicao { get; set; }
            public DateTime? DataDevolucao { get; set; }
            public int ExemplarID { get; set; }
        }

        public class HistoricoRequisicaoDTO
        {
            public string Leitor { get; set; }
            public string Obra { get; set; }
            public string Nucleo { get; set; }
            public DateTime DataRequisicao { get; set; }
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

        public class SituacaoLeitor
        {
            public string Leitor { get; set; }
            public string Obra { get; set; }
            public string Nucleo { get; set; }
            public DateTime DataRequisicao { get; set; }
            public string StatusDevolucao { get; set; }
        }

        public class LeitorSuspenso
        {
            public int UserID { get; set; }
            public string Nome { get; set; }
        }

        public class ExemplarDTO
        {
            public int ExemplarID { get; set; }
            public int ObraID { get; set; }
            public bool Disponivel { get; set; }
        }

        public class NucleoUltimaRequisicao
        {
            public string NomeNucleo { get; set; }
            public string UltimaRequisicao { get; set; }
        }

        public class ExemplarPorNucleo
        {
            public string Titulo { get; set; }
            public string NomeNucleo { get; set; }
            public int Quantidade { get; set; }
        }

        public class ResultadoTransferencia
        {
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
        }

        public class TransferenciaDTO
        {
            public int ObraID { get; set; }
            public int OrigemNucleoID { get; set; }
            public int DestinoNucleoID { get; set; }
            public int QuantidadeTransferida { get; set; }
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
        }

}
