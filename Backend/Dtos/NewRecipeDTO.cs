using Backend.Models;

namespace Backend.Dtos
{
    public class NewRecipeDTO
    {
        public int IdKorisnika { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public int? PreparationTime { get; set; }
        public string? PreparationTimeMH { get; set; }
        public int? NumberOfServings { get; set; }
        public int? CookingTime { get; set; }
        public string? CookingTimeMH { get; set; }
        public string? Difficulty { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
    }
}
