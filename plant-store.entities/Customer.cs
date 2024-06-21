using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace plant_store.entities;

[Table("customer")]
[Index("Password", Name = "password", IsUnique = true)]
[Index("Username", Name = "username", IsUnique = true)]
public partial class Customer
{
    [Key]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("username")]
    public string Username { get; set; } = null!;

    [Column("password")]
    public string Password { get; set; } = null!;

    [InverseProperty("Customer")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
