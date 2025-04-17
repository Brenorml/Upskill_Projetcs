using System;
using System.Collections.Generic;

namespace LibDB_EF.Models;

public partial class Usuario
{
    public int UserId { get; set; }

    public string Nome { get; set; } = null!;

    public DateOnly DataNascimento { get; set; }

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public DateOnly? DataRegisto { get; set; }

    public string Username { get; set; } = null!;

    public byte[] PalavraPasse { get; set; } = null!;

    public string? TipoUser { get; set; }

    public bool? Ativo { get; set; }

    public virtual ICollection<Requisico> Requisicos { get; set; } = new List<Requisico>();
}
