using plant_store.entities;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
public static class CustomerEndpoints
{
   

    public static void RegisterCustomerEndpoints(this WebApplication app)
    {
        
         app.MapGet("api/customers/{id:int}", 
           (
            [FromServices] PlantstoreDbContext db,
            [FromRoute] int id) => {
               var customer = db.Customers
                    .Where(c => c.CustomerId == id)
                    .Select(c => new CustomerDTO
                        {
                            CustomerId = c.CustomerId,
                            Name = c.Name,
                            Username = c.Username
                        })
                    .FirstOrDefault();

                if (customer == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(customer);
            })
            .WithName("GetCustomerById")
            .WithOpenApi()
            .Produces<CustomerDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

         app.MapGet("api/customers/{name}", 
           (
            [FromServices] PlantstoreDbContext db,
            [FromRoute] string name) => {
               var customer = db.Customers
                    .Where(c => c.Name.Equals(name))
                    .Select(c => new CustomerDTO
                        {
                            CustomerId = c.CustomerId,
                            Name = c.Name,
                            Username = c.Username
                        })
                    .FirstOrDefault();

                if (customer == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(customer);
            })
            .WithName("GetCustomerByName")
            .WithOpenApi()
            .Produces<CustomerDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        

        app.MapPost("api/customers/signup", async ([FromBody] Customer customer, 
            [FromServices] PlantstoreDbContext db) =>
            {
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                var newCustomer = new CustomerDTO
                {
                
                    CustomerId = customer.CustomerId,
                    Name = customer.Name,
                    Username = customer.Username
                };
                return Results.Created($"api/customers/{newCustomer.CustomerId}", newCustomer );
            }).WithOpenApi()
            .Produces<CustomerDTO>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
        
        app.MapPost("api/customers/login",  ([FromBody] LoginDTO loginDTO,
            [FromServices] PlantstoreDbContext db) =>{
            
                var customer = db.Customers
                    .Where(c => c.Username == loginDTO.Username && c.Password == loginDTO.Password)
                    .Select(c => new CustomerDTO
                    {
                       CustomerId = c.CustomerId,
                       Name = c.Name,
                       Username = c.Username
                    }
                    )
                    .FirstOrDefault();    
                if (customer == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(customer);
            })
                
            .WithOpenApi()
            .Produces<CustomerDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPut("api/customers/{id:int}", async (
            [FromRoute] int id, 
            [FromBody] Customer customer, 
            [FromServices] PlantstoreDbContext db) =>
        {
            Customer? foundCustomer = await db.Customers.FindAsync(id);
            
            if (foundCustomer is null) return Results.NotFound();
                
            foundCustomer.Username = customer.Username ?? foundCustomer.Username;
            foundCustomer.Password = customer.Password ?? foundCustomer.Password;
               
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).WithOpenApi()
          .Produces(StatusCodes.Status404NotFound)
          .Produces(StatusCodes.Status204NoContent);
        
        app.MapDelete("api/customers/{id:int}", async (
            [FromRoute] int id,
            [FromServices] PlantstoreDbContext db) =>
            {
                Customer? foundCustomer = await db.Customers.FindAsync(id);
                if(foundCustomer is null) return Results.NotFound();

                db.Customers.Remove(foundCustomer);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }
         ).WithOpenApi()
          .Produces(StatusCodes.Status404NotFound)
          .Produces(StatusCodes.Status204NoContent);
    }
}