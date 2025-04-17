using System;
using System.Collections.Generic;

namespace LibDB_EF.Models;

public partial class Obra
{
    public int ObraId { get; set; }

    public string Titulo { get; set; } = null!;

    public string Autor { get; set; } = null!;

    public int? AnoPublicacao { get; set; }

    public string? Genero { get; set; }

    public string? Descricao { get; set; }

    public virtual ICollection<Exemplare> Exemplares { get; set; } = new List<Exemplare>();
}
