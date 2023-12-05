using Backend.Dto;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly DataContext dc;

        public CommentsController(DataContext dc)
        {
            this.dc = dc;
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] CommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid comment data.");
            }

            var comment = new Comment
            {
                IdKorisnika = commentDto.IdKorisnika,
                IdRecipe = commentDto.IdRecipe,
                Text = commentDto.Text
            };

            try
            {
                dc.Comments.Add(comment);
                await dc.SaveChangesAsync();
                return Ok("Comment added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to add comment: {ex.Message}");
            }
        }
    }
}
