namespace Backend.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
