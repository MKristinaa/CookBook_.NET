using Backend.Dto;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly DataContext dc;

        public RatingController(DataContext dc)
        {
            this.dc = dc;
        }

        [HttpPost("AddRating")]
        public async Task<IActionResult> AddRating([FromBody] RecipeRatingDto ratingDto)
        {
            try
            {
                var existingRating = await dc.RecipeRatings
                    .FirstOrDefaultAsync(rr => rr.RecipeId == ratingDto.RecipeId && rr.UserId == ratingDto.UserId);

                if (existingRating != null)
                {
                    return BadRequest("Ocjena već postoji za ovaj recept i korisnika.");
                }

                var newRating = new RecipeRating
                {
                    RecipeId = ratingDto.RecipeId,
                    UserId = ratingDto.UserId,
                    Rating = ratingDto.Rating
                };

                dc.RecipeRatings.Add(newRating);
                await dc.SaveChangesAsync();

                return Ok("Ocjena uspješno dodana.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška prilikom dodavanja ocjene: {ex.Message}");
            }
        }
        [HttpGet("GetAverageRating/{recipeId}")]
        public async Task<IActionResult> GetAverageRating(int recipeId)
        {
            try
            {
                var averageRating = await dc.RecipeRatings
                    .Where(rr => rr.RecipeId == recipeId)
                    .AverageAsync(rr => rr.Rating);

                return Ok(averageRating);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška prilikom dohvatanja prosečne ocene: {ex.Message}");
            }
        }


    }
}
