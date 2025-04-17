using System;
using System.Collections.Generic;

namespace LibDB_EF.Models;

public partial class Exemplare
{
    public int ExemplarId { get; set; }

    public int ObraId { get; set; }

    public int NucleoId { get; set; }

    public bool? Disponivel { get; set; }

    public virtual Nucleo Nucleo { get; set; } = null!;

    public virtual Obra Obra { get; set; } = null!;

    public virtual ICollection<Requisico> Requisicos { get; set; } = new List<Requisico>();
}
