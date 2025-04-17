using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Biblioteca.Model
{
    public class Usuario
    {
        public int UserID { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public DateTime DataRegisto { get; set; }
        public string Username { get; set; }
        public byte[] PalavraPasse { get; set; }
        public string TipoUser { get; set; }
        public bool Ativo { get; set; }
    }

    public class Nucleo
    {
        public int NucleoID { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
    }

    public class Obra
    {
        public int ObraID { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public int AnoPublicacao { get; set; }
        public string Genero { get; set; }
        public string Descricao { get; set; }

    }

    public class Imagem
    {
        public int ID_ObraID { get; set; }
        public byte[] Capa { get; set; }
    }

    public class Exemplar
    {
        public int ExemplarID { get; set; }
        public int ObraID { get; set; }
        public int NucleoID { get; set; }
        public bool Disponivel { get; set; }
    }

    public class Requisicao
    {
        public int RequisicaoID { get; set; }
        public int UserID { get; set; }
        public int ExemplarID { get; set; }
        public DateTime DataRequisicao { get; set; }
        public DateTime? DataDevolucao { get; set; }
    }

    public class HistoricoRequisicao
    {
        public int IdHistorico { get; set; }
        public int? RequisicaoID { get; set; }
        public int? UserID { get; set; }
        public string NomeUser { get; set; }
        public string Telefone { get; set; }
        public string TituloObra { get; set; }
        public int? ExemplarID { get; set; }
        public string Nucleo { get; set; }
        public DateTime? DataRequisicao { get; set; }
        public DateTime? DataDevolucao { get; set; }
    }

}
