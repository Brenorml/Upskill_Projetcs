using System;
using System.Collections.Generic;

namespace LibDB_EF.Models;

public partial class Requisico
{
    public int RequisicaoId { get; set; }

    public int UserId { get; set; }

    public int ExemplarId { get; set; }

    public DateOnly DataRequisicao { get; set; }

    public DateOnly? DataDevolucao { get; set; }

    public virtual Exemplare Exemplar { get; set; } = null!;

    public virtual Usuario User { get; set; } = null!;
}
