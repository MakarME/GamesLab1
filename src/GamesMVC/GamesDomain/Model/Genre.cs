using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GamesDomain.Model;

public partial class Genre : Entity
{
    [Required(ErrorMessage = "This field must not be empty")]
    [Display(Name="Genre")]
    public string Name { get; set; } = null!;

    [Display(Name = "Genre info")]
    public string? Info { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
