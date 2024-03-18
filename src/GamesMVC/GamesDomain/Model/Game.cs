using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamesDomain.Model;

public partial class Game : Entity
{
    [Display(Name = "Developer")]
    public int DeveloperId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly ReleaseDate { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    //[Display(Name = "Developer")]
    //[BindNever]
    public virtual Developer Developer { get; set; } = null!;

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
