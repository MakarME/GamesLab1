using System;
using System.Collections.Generic;

namespace GamesDomain.Model;

public partial class Rating
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public int GameId { get; set; }

    public decimal Rating1 { get; set; }

    public DateTime RatingDate { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
