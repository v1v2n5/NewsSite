using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using NewsSite.Models;
using Microsoft.AspNetCore.Identity;

namespace NewsSite.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Category category1 = new() { Id = 1, Name = "<Без категории>" };

            modelBuilder.Entity<Category>().HasData(category1);

        }
        */
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<NewsTags> NewsTags { get; set; }

        public DbSet<ActivityDirections> ActivityDirections { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Contact> Contacts { get; set; }


    }
}
