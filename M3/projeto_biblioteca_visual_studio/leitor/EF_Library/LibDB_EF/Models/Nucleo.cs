using System;
using System.Collections.Generic;

namespace LibDB_EF.Models;

public partial class Nucleo
{
    public int NucleoId { get; set; }

    public string Nome { get; set; } = null!;

    public string Endereco { get; set; } = null!;

    public virtual ICollection<Exemplare> Exemplares { get; set; } = new List<Exemplare>();
}
