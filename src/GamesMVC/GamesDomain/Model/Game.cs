using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GamesDomain.Model;

public partial class Game : Entity
{
    [Display(Name = "Developer")]
    public int DeveloperId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly ReleaseDate { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    [JsonIgnore]
    public virtual Developer Developer { get; set; } = null!;
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    [JsonIgnore]
    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
