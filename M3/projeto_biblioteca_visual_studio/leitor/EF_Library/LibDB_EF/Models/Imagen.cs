using System;
using System.Collections.Generic;

namespace LibDB_EF.Models;

public partial class Imagen
{
    public int? IdObraId { get; set; }

    public byte[]? Capa { get; set; }

    public virtual Obra? IdObra { get; set; }
}
