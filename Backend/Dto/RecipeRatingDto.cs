namespace Backend.Dto
{
    public class RecipeRatingDto
    {
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public int? Rating { get; set; }
    }
}
