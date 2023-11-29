namespace Backend.Dtos
{
    public class NewRecipeIngredientDTO
    {
        public decimal? Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? IngredientName { get; set; }
    }
}
