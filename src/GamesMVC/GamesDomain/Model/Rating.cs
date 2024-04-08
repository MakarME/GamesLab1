using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GamesDomain.Model;

public partial class Rating : Entity
{
    [Display(Name = "Player")]
    public int PlayerId { get; set; }
    [Display(Name = "Game")]
    public int GameId { get; set; }
    [Display(Name = "Score")]
    public decimal Rating1 { get; set; }
    [Display(Name = "Date")]
    public DateTime RatingDate { get; set; }
    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
