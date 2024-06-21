using plant_store.entities;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
public static class PostsEndpoints
{
   

    public static void RegisterPostsEndpoints(this WebApplication app,
        int pageSize = 10)
    {
        app.MapGet("api/posts", (
            [FromServices] PlantstoreDbContext db,
            [FromQuery] int? page  
        ) =>
        db.Posts
            .OrderBy(p => p.PostId)
            .Skip(((page ?? 1) - 1) * pageSize)
            .Take(pageSize)
        )
        .WithName("GetPosts")
        .WithOpenApi(operation =>
        {
            operation.Description = "Get posts default page size is 10";
            operation.Summary = "Get posts default page size is 10";
            return operation;
        })
        .Produces<Post[]>(StatusCodes.Status200OK);

         app.MapGet("api/posts/{id:int}",
            async Task<Results<Ok<Post>, NotFound>> (
            [FromServices] PlantstoreDbContext db,
            [FromRoute] int id) =>
                await db.Posts.FindAsync(id) is Post post ? TypedResults.Ok(post) : TypedResults.NotFound())
            .WithName("GetProductById")
            .WithOpenApi()
            .Produces<Post>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        
        app.MapGet("api/posts/{name}", (
            [FromServices] PlantstoreDbContext db,
            [FromRoute] string name) =>
                db.Posts.Where(p => p.PostName.Contains(name)))
            .WithName("GetPostByName")
            .WithOpenApi()
            .Produces<Post[]>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("api/posts", async ([FromBody] Post post, 
            [FromServices] PlantstoreDbContext db) =>
            {
                db.Posts.Add(post);
                await db.SaveChangesAsync();
                return Results.Created($"api/products/{post.PostId}", post);
            }).WithOpenApi()
            .Produces<Post>(StatusCodes.Status201Created);
        
        app.MapPut("api/products/{id:int}", async (
            [FromRoute] int id, 
            [FromBody] Post post, 
            [FromServices] PlantstoreDbContext db) =>
        {
            Post? foundPost = await db.Posts.FindAsync(id);
            
            if (foundPost is null) return Results.NotFound();
                
            foundPost.PostName = post.PostName;
            foundPost.Description = post.Description;
            foundPost.Price = post.Price;          

            await db.SaveChangesAsync();
            return Results.NoContent();
        }).WithOpenApi()
          .Produces(StatusCodes.Status404NotFound)
          .Produces(StatusCodes.Status204NoContent);
        
        app.MapDelete("api/products/{id:int}", async (
            [FromRoute] int id,
            [FromServices] PlantstoreDbContext db) =>
            {
                Post? foundpost = await db.Posts.FindAsync(id);
                if(foundpost is null) return Results.NotFound();

                db.Posts.Remove(foundpost);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }
         ).WithOpenApi()
          .Produces(StatusCodes.Status404NotFound)
          .Produces(StatusCodes.Status204NoContent);
    }
}