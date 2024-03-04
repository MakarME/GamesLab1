using System;
using System.Collections.Generic;

namespace GamesDomain.Model;

public partial class Game
{
    public int Id { get; set; }

    public int DeveloperId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly ReleaseDate { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Developer Developer { get; set; } = null!;

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
