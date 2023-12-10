using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Lastname { get; set; }
        public string? Image { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public byte[]? Password { get; set; }
        public byte[]? KeyPassword { get; set; }
        public List<Recipe>? Recipes { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<RecipeRating>? RatedRecipes { get; set; }
    }
}
