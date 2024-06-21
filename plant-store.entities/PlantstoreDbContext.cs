using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace plant_store.entities;

public partial class PlantstoreDbContext : DbContext
{
    public PlantstoreDbContext()
    {
    }

    public PlantstoreDbContext(DbContextOptions<PlantstoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySQL("Configuration.GetConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PRIMARY");

            entity.HasOne(d => d.Customer).WithMany(p => p.Posts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("post_ibfk_1");

            entity.HasMany(d => d.Categories).WithMany(p => p.Posts)
                .UsingEntity<Dictionary<string, object>>(
                    "PostCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("post_categories_ibfk_2"),
                    l => l.HasOne<Post>().WithMany()
                        .HasForeignKey("PostId")
                        .HasConstraintName("post_categories_ibfk_1"),
                    j =>
                    {
                        j.HasKey("PostId", "CategoryId").HasName("PRIMARY");
                        j.ToTable("post_categories");
                        j.HasIndex(new[] { "CategoryId" }, "category_id");
                        j.IndexerProperty<int>("PostId").HasColumnName("post_id");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("category_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
