using Microsoft.EntityFrameworkCore;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo > Photos { get; set; }
        public DbSet<Shopping> Shoppings { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Fridge> Fridges { get; set; }
		public DbSet<Friendship> Friendships { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().HasOne(r => r.Recipe)
                                     .WithMany(r => r.Products)
                                     .HasForeignKey(r => r.RecipeId)
                                     .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>().HasOne(r => r.Shopping)
                                     .WithMany(r => r.Products)
                                     .HasForeignKey(r => r.ShoppingId)
                                     .OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Product>().HasOne(r => r.Fridge)
									 .WithMany(r => r.Products)
									 .HasForeignKey(r => r.FridgeId)
									 .OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Photo>().HasOne(r => r.User)
                                   .WithOne(r => r.Photo)
                                   .HasForeignKey<User>(r => r.PhotoId)
                                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Photo>().HasOne(r => r.Recipe)
                                   .WithOne(r => r.Photo)
                                   .HasForeignKey<Recipe>(r => r.PhotoId)
                                   .OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Recipe>().HasOne(s => s.Shopping)
									  .WithOne(s => s.Recipe)
									  .HasForeignKey<Shopping>(s => s.RecipeId)
									  .OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Shopping>().HasOne(r => r.Recipe)
									  .WithOne(r => r.Shopping)
									  .HasForeignKey<Shopping>(r => r.RecipeId)
									  .OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Fridge>().HasOne(f => f.User)
								   .WithOne(f => f.Fridge)
								   .HasForeignKey<User>(f => f.FridgeId)
								   .OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Friendship>().HasKey(k => new { k.UserFollowId, k.UserIsFollowedId });

			builder.Entity<Friendship>().HasOne(u => u.UserIsFollowed)
										.WithMany(u => u.UserFollow)
										.HasForeignKey(u => u.UserIsFollowedId)
										.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Friendship>().HasOne(u => u.UserFollow)
										.WithMany(u => u.UserIsFollowed)
										.HasForeignKey(u => u.UserFollowId)
										.OnDelete(DeleteBehavior.Restrict);
		}
    }
}
