using System;
using System.Collections.Generic;

namespace GamesDomain.Model;

public partial class Comment : Entity
{
    public int PlayerId { get; set; }

    public int GameId { get; set; }

    public string? Text { get; set; }

    public byte[] CommentDate { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
