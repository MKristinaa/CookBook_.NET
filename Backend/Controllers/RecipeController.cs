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
                    IdKorisnika = recipeWithIngredients.RecipeInfo.IdKorisnika,
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

        [HttpGet("GetAllRecipesWithIngredients/{idKorisnika}")]
        public IActionResult GetAllRecipesWithIngredients(int idKorisnika)
        {
            try
            {
                var recipesWithIngredients = dc.RecipeIngredients
                    .Include(ri => ri.Recipe)
                    .Include(ri => ri.Ingredient)
                    .Where(ri => ri.Recipe.IdKorisnika == idKorisnika)
                    .ToList();

                return Ok(recipesWithIngredients);
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to retrieve recipes with ingredients: {ex.Message}");
            }
        }

        [HttpGet("GetAllRecipesWithIngredientsByIdRecipe/{idRecepta}")]
        public IActionResult GetAllRecipesWithIngredientsByIdRecipe(int idRecepta)
        {
            try
            {
                var recipesWithIngredients = dc.RecipeIngredients
                    .Include(ri => ri.Recipe)
                    .Include(ri => ri.Ingredient)
                    .Where(ri => ri.Recipe.Id == idRecepta)
                    .ToList();

                return Ok(recipesWithIngredients);
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to retrieve recipes with ingredients: {ex.Message}");
            }
        }


        [HttpGet("GetUserDataByRecipeId/{idRecepta}")]
        public IActionResult GetUserDataByRecipeId(int idRecepta)
        {
            try
            {
                var userData = dc.Recipes
                    .Where(r => r.Id == idRecepta)
                    .Join(dc.Users,
                        recipe => recipe.IdKorisnika,
                        user => user.Id,
                        (recipe, user) => new
                        {
                            UserId = user.Id,
                            UserName = user.Name,
                            UserLastname = user.Lastname,
                            UserImage = user.Image
                        })
                    .FirstOrDefault();

                if (userData == null)
                {
                    return NotFound("Recipe or user not found");
                }

                return Ok(userData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to retrieve user data: {ex.Message}");
            }
        }

        [HttpGet("GetUserDataByUserId/{userId}")]
        public IActionResult GetUserDataByUserId(int userId)
        {
            try
            {
                var user = dc.Users
                    .FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var userData = new
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    UserLastname = user.Lastname,
                    UserImage = user.Image
                };

                return Ok(userData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to retrieve user data: {ex.Message}");
            }
        }

    }
}

