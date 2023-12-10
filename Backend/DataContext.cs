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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<RecipeRating> RecipeRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.User)
                .WithMany(u => u.Recipes) 
                .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.ClientSetNull); 

            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(i => i.RecipeId)
               .OnDelete(DeleteBehavior.ClientSetNull); 

            modelBuilder.Entity<Comment>()
               .HasOne(c => c.Recipe)
               .WithMany(r => r.Comments)
               .HasForeignKey(c => c.RecipeId)
               .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<RecipeRating>()
                .HasOne(rr => rr.Recipe)
                .WithMany(r => r.Ratings)
                .HasForeignKey(rr => rr.RecipeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<RecipeRating>()
                .HasOne(rr => rr.User)
                .WithMany(u => u.RatedRecipes)
                .HasForeignKey(rr => rr.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);



            base.OnModelCreating(modelBuilder);
        }
    }
}
