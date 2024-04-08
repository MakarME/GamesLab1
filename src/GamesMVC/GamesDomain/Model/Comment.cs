using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GamesDomain.Model;

public partial class Comment : Entity
{
    [Display(Name="Player")]
    public int PlayerId { get; set; }
    [Display(Name = "Game")]
    public int GameId { get; set; }
    [Display(Name = "Comment")]
    public string? Text { get; set; }

    public DateTime CommentDate { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
