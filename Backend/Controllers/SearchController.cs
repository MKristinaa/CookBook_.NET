using Backend.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly DataContext dc;

        public SearchController(DataContext dc)
        {
            this.dc = dc;
        }

        [HttpGet("GetRecipesByCategoryOrName/{searchTerm}")]
        public IActionResult GetRecipesByCategoryOrName(string searchTerm)
        {
            try
            {
                var recipesByCategoryOrName = dc.Recipes
                    .Include(r => r.Ingredients)
                    .Where(r => r.Kategory.Contains(searchTerm) || r.Name.Contains(searchTerm))
                    .ToList();

                var recipeDtos = recipesByCategoryOrName.Select(recipe => new RecipeDto
                {
                    Id = recipe.Id,
                    Name = recipe.Name,
                    Kategory = recipe.Kategory,
                    PreparationTime = recipe.PreparationTime,
                    PreparationTimeMH = recipe.PreparationTimeMH,
                    NumberOfServings = recipe.NumberOfServings,
                    CookingTime = recipe.CookingTime,
                    CookingTimeMH = recipe.CookingTimeMH,
                    Difficulty = recipe.Difficulty,
                    Image = recipe.Image,
                    Description = recipe.Description,
                    UserId = recipe.UserId,
                    Ingredients = recipe.Ingredients.Select(ingredient => new IngridientDto
                    {
                        Name = ingredient.Name,
                        Quantity = ingredient.Quantity,
                        UnitOfMeasure = ingredient.UnitOfMeasure
                    }).ToList()
                }).ToList();

                return Ok(recipeDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to retrieve recipes by category or name: {ex.Message}");
            }
        }

        [HttpGet("GetRecipesByCategory")]
        public IActionResult GetRecipesByCategory([FromQuery(Name = "searchTerms")] List<string> searchTerms)
        {
            try
            {
                var recipesByCategoryOrName = dc.Recipes
                    .Include(r => r.Ingredients)
                    .AsEnumerable()
                    .Where(r => searchTerms.Any(term => r.Kategory.Contains(term.Trim())))
                    .ToList();


                var recipeDtos = recipesByCategoryOrName.Select(recipe => new RecipeDto
                {
                    Id = recipe.Id,
                    Name = recipe.Name,
                    Kategory = recipe.Kategory,
                    PreparationTime = recipe.PreparationTime,
                    PreparationTimeMH = recipe.PreparationTimeMH,
                    NumberOfServings = recipe.NumberOfServings,
                    CookingTime = recipe.CookingTime,
                    CookingTimeMH = recipe.CookingTimeMH,
                    Difficulty = recipe.Difficulty,
                    Image = recipe.Image,
                    Description = recipe.Description,
                    UserId = recipe.UserId,
                    Ingredients = recipe.Ingredients.Select(ingredient => new IngridientDto
                    {
                        Name = ingredient.Name,
                        Quantity = ingredient.Quantity,
                        UnitOfMeasure = ingredient.UnitOfMeasure
                    }).ToList()
                }).ToList();

                return Ok(recipeDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Unable to retrieve recipes by categories: {ex.Message}");
            }
        }

    }
}
