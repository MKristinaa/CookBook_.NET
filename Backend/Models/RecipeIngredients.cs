namespace Backend.Models
{
    public class RecipeIngredients
    {
        public int Id { get; set; }
        public decimal? Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }

        public int IdRecipe { get; set; }
        public Recipe? Recipe { get; set; }
        public int IdIngredient { get; set; }
        public Ingredient? Ingredient { get; set; }
    }
}
