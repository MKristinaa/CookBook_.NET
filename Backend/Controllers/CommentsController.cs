using Backend.Dto;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                UserId = commentDto.UserId,
                RecipeId = commentDto.RecipeId,
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

        [HttpGet("CountCommentsByRecipeId/{recipeId}")]
        public async Task<IActionResult> CountCommentsByRecipeId(int recipeId)
        {
            try
            {
                var commentCount = await dc.Comments
                    .Where(c => c.RecipeId == recipeId)
                    .CountAsync();

                return Ok(commentCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to count comments: {ex.Message}");
            }
        }


        [HttpGet("GetCommentsByRecipeId/{recipeId}")]
        public async Task<IActionResult> GetCommentsByRecipeId(int recipeId)
        {
            try
            {
                var comments = await dc.Comments
                    .Where(c => c.RecipeId == recipeId)
                    .Include(c => c.User) 
                    .Select(c => new
                    {
                        c.Id,
                        c.Text,
                        c.UserId,
                        UserInfo = new
                        {
                            c.User.Name,
                            c.User.Lastname,
                            c.User.Image
                        }
                    })
                    .ToListAsync();

                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve comments: {ex.Message}");
            }
        }


        [HttpGet("GetUserByRecipeAndCommentId/{recipeId}/{commentId}")]
        public async Task<IActionResult> GetUserByRecipeAndCommentId(int recipeId, int commentId)
        {
            try
            {
                var comment = await dc.Comments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == commentId && c.RecipeId == recipeId);

                var userId = comment.UserId;
                var user = await dc.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);


                var userInfo = new
                {
                    user.Name,
                    user.Lastname,
                    user.Image
                };

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve user info: {ex.Message}");
            }
        }


    }
}
