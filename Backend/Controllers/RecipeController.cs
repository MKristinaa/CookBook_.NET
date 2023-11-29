using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly DataContext dc;

        public RecipeController(DataContext dc)
        {
            this.dc = dc;
        }

        [HttpPost("AddRecipe")]
        public async Task<IActionResult> AddRecipe([FromBody] NewRecipeWithIngredientsDTO recipeWithIngredients)
        {
            try
            {
                var recipe = new Recipe
                {
                    Name = recipeWithIngredients.RecipeInfo.Name,
                    Kategory = recipeWithIngredients.RecipeInfo.Category,
                    PreparationTime = recipeWithIngredients.RecipeInfo.PreparationTime,
                    PreparationTimeMH = recipeWithIngredients.RecipeInfo.PreparationTimeMH,
                    NumberOfServings = recipeWithIngredients.RecipeInfo.NumberOfServings,
                    CookingTime = recipeWithIngredients.RecipeInfo.CookingTime,
                    CookingTimeMH = recipeWithIngredients.RecipeInfo.CookingTimeMH,
                    Difficulty = recipeWithIngredients.RecipeInfo.Difficulty,
                    Image = recipeWithIngredients.RecipeInfo.Image,
                    Description = recipeWithIngredients.RecipeInfo.Description
                };

                dc!.Recipes?.Add(recipe);
                await dc.SaveChangesAsync();

                foreach (var ingredientInfo in recipeWithIngredients.IngredientsInfo)
                {
                    var ingredient = new Ingredient
                    {
                        Name = ingredientInfo.IngredientName
                    };

                    dc.Ingredients!.Add(ingredient);
                    await dc.SaveChangesAsync();

                    var recipeIngredient = new RecipeIngredients
                    {
                        Quantity = ingredientInfo.Quantity,
                        UnitOfMeasure = ingredientInfo.UnitOfMeasure,
                        Recipe = recipe,
                        Ingredient = ingredient
                    };

                    dc.RecipeIngredients.Add(recipeIngredient);
                    await dc.SaveChangesAsync();
                }

                return Ok(recipe);
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to add recipe: {ex.Message}");
            }
        }

        [HttpGet("GetAllRecipesWithIngredients")]
        public IActionResult GetAllRecipesWithIngredients()
        {
            try
            {
                var recipesWithIngredients = dc.RecipeIngredients
                    .Include(ri => ri.Recipe)
                    .Include(ri => ri.Ingredient)
                    .ToList();

                return Ok(recipesWithIngredients);
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to retrieve recipes with ingredients: {ex.Message}");
            }
        }

    }
}

