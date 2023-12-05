using Backend.Dto;
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
        public void AddNewRecipe(NewRecipeDTO newRecipeDto)
        {
            var newRecipe = new Recipe
            {
                Name = newRecipeDto.Name,
                Kategory = newRecipeDto.Kategory,
                PreparationTime = newRecipeDto.PreparationTime,
                PreparationTimeMH = newRecipeDto.PreparationTimeMH,
                NumberOfServings = newRecipeDto.NumberOfServings,
                CookingTime = newRecipeDto.CookingTime,
                CookingTimeMH = newRecipeDto.CookingTimeMH,
                Difficulty = newRecipeDto.Difficulty,
                Image = newRecipeDto.Image,
                Description = newRecipeDto.Description,
                UserId = newRecipeDto.UserId
            };

            var ingredients = newRecipeDto.Ingredients.Select(i => new Ingredient
            {
                Name = i.Name,
                Quantity = i.Quantity,
                UnitOfMeasure = i.UnitOfMeasure,
                Recipe = newRecipe 
            }).ToList();

            newRecipe.Ingredients = ingredients;

            dc.Recipes.Add(newRecipe);
            dc.SaveChanges();
        }
        [HttpGet("GetAllRecipes")]
        public IActionResult GetAllRecipes()
        {
            try
            {
                var recipesWithIngredients = dc.Recipes
                    .Include(r => r.Ingredients)
                    .Select(r => new RecipeDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Kategory = r.Kategory,
                        PreparationTime = r.PreparationTime,
                        PreparationTimeMH = r.PreparationTimeMH,
                        NumberOfServings = r.NumberOfServings,
                        CookingTime = r.CookingTime,
                        CookingTimeMH = r.CookingTimeMH,
                        Difficulty = r.Difficulty,
                        Image = r.Image,
                        Description = r.Description,
                        UserId = r.UserId,
                        Ingredients = r.Ingredients.Select(i => new IngridientDto
                        {
                            Name = i.Name,
                            Quantity = i.Quantity,
                            UnitOfMeasure = i.UnitOfMeasure
                        }).ToList()
                    })
                    .ToList();

                return Ok(recipesWithIngredients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("GetUserByRecipeId/{recipeId}")]
        public IActionResult GetUserByRecipeId(int recipeId)
        {
            try
            {
                var user = dc.Recipes
                    .Where(r => r.Id == recipeId)
                    .Select(r => new
                    {
                        UserId = r.User.Id,
                        UserName = r.User.Name,
                        UserLastname = r.User.Lastname,
                        UserImage = r.User.Image
                    })
                    .FirstOrDefault();

                if (user == null)
                {
                    return NotFound($"Recipe with ID {recipeId} not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("GetRecipesByUserId/{userId}")]
        public IActionResult GetRecipesByUserId(int userId)
        {
            try
            {
                var userRecipes = dc.Users
                    .Where(u => u.Id == userId)
                    .SelectMany(u => u.Recipes) 
                    .Include(r => r.Ingredients) 
                    .Select(r => new
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Kategory = r.Kategory,
                        PreparationTime= r.PreparationTime,
                        PreparationTimeMH=r.PreparationTimeMH,
                        NumberOfServings=r.NumberOfServings,
                        CookingTime=r.CookingTime,
                        CookingTimeMH=r.CookingTimeMH,
                        Difficulty=r.Difficulty,
                        Image=r.Image,
                        Description=r.Description,
                        Ingredients = r.Ingredients.Select(i => new
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Quantity = i.Quantity,
                            UnitOfMeasure = i.UnitOfMeasure
                        }).ToList()
                    })
                    .ToList();

                if (userRecipes == null || !userRecipes.Any())
                {
                    return NotFound($"No recipes found for user with ID {userId}.");
                }

                return Ok(userRecipes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("GetRecipeWithIngredientsById/{recipeId}")]
        public IActionResult GetRecipeWithIngredientsById(int recipeId)
        {
            try
            {
                var recipeWithIngredients = dc.Recipes
                    .Include(r => r.Ingredients)
                    .FirstOrDefault(r => r.Id == recipeId);

                if (recipeWithIngredients == null)
                {
                    return NotFound();
                }

                var recipeDto = new RecipeDto
                {
                    Id = recipeWithIngredients.Id,
                    Name = recipeWithIngredients.Name,
                    Kategory = recipeWithIngredients.Kategory,
                    PreparationTime = recipeWithIngredients.PreparationTime,
                    PreparationTimeMH = recipeWithIngredients.PreparationTimeMH,
                    NumberOfServings = recipeWithIngredients.NumberOfServings,
                    CookingTime = recipeWithIngredients.CookingTime,
                    CookingTimeMH = recipeWithIngredients.CookingTimeMH,
                    Difficulty = recipeWithIngredients.Difficulty,
                    Image = recipeWithIngredients.Image,
                    Description = recipeWithIngredients.Description,
                    UserId = recipeWithIngredients.UserId,
                    Ingredients = recipeWithIngredients.Ingredients.Select(i => new IngridientDto
                    {
                        Name = i.Name,
                        Quantity = i.Quantity,
                        UnitOfMeasure = i.UnitOfMeasure
                    }).ToList()
                };

                return Ok(recipeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
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

