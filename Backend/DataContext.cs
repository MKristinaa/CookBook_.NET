using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User>? Users { get; set; }
        public DbSet<Recipe>? Recipes { get; set; }
        public DbSet<Ingredient>? Ingredients { get; set; }
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }
    }
}
