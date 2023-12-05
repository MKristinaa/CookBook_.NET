using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Dto
{
    public class CommentDto
    {
        public int IdKorisnika { get; set; }
        public int IdRecipe { get; set; }
        public string? Text { get; set; }
    }
}
