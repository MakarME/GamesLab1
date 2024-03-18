using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesDomain.Model
{
    public abstract class Entity
    {
        [Required(ErrorMessage = "This field must not be empty")]
        public int Id { get; set; }
    }
}
