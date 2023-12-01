using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Kategory { get; set; }
        public int? PreparationTime { get; set; }
        public string? PreparationTimeMH { get; set; }
        public int? NumberOfServings { get; set; }
        public int? CookingTime { get; set; }
        

        public string? CookingTimeMH { get; set; }
        public string? Difficulty { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }

        [ForeignKey("User")]
        public int IdKorisnika { get; set; }

    }
}
