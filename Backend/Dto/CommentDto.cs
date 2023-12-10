using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Dto
{
    public class CommentDto
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public string? Text { get; set; }
    }
}
