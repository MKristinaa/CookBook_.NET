using Backend.Models;

namespace Backend.Dto
{
    public class NewRecipeDTO
    {
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

        public int UserId { get; set; }

        public List<IngridientDto> Ingredients { get; set; }
    }
}
