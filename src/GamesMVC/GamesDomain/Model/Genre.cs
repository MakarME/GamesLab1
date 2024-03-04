using System;
using System.Collections.Generic;

namespace GamesDomain.Model;

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Info { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
