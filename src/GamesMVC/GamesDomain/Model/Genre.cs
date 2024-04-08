using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GamesDomain.Model;

public partial class Genre : Entity
{
    [Required(ErrorMessage = "This field must not be empty")]
    [CustomValidation(typeof(Genre), "NameValidation", ErrorMessage = "Name should not consist only of digits")]
    [Display(Name="Genre")]
    public string Name { get; set; } = null!;

    [Display(Name = "Genre info")]
    public string? Info { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public static ValidationResult NameValidation(string name, ValidationContext context)
    {
        if (name != null && name.All(char.IsDigit))
        {
            return new ValidationResult("Name should not consist only of digits");
        }
        return ValidationResult.Success;
    }
}
