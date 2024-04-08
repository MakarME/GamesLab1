using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GamesDomain.Model;

public partial class Player : Entity
{
    public string Name { get; set; } = null!;
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
