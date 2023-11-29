namespace Backend.Dtos
{
    public class NewRecipeWithIngredientsDTO
    {
        public NewRecipeDTO? RecipeInfo { get; set; }
        public List<NewRecipeIngredientDTO>? IngredientsInfo { get; set; }
    }
}
