using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int IdKorisnika { get; set; }
        [ForeignKey("Recipe")]
        public int IdRecipe { get; set; }
        public string Text { get; set; }
    }
}
