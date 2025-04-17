using System;
using System.Collections.Generic;

namespace LibDB_EF.Models;

public partial class HistoricoRequisico
{
    public int IdHistorico { get; set; }

    public int? RequisicaoId { get; set; }

    public int? UserId { get; set; }

    public string? NomeUser { get; set; }

    public string? Telefone { get; set; }

    public string? TituloObra { get; set; }

    public int? ExemplarId { get; set; }

    public string? Nucleo { get; set; }

    public DateOnly? DataRequisicao { get; set; }

    public DateOnly? DataDevolucao { get; set; }
}
