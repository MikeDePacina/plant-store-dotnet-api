using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace plant_store.entities;

[Table("post")]
[Index("CustomerId", Name = "customer_id")]
public partial class Post
{
    [Key]
    [Column("post_id")]
    public int PostId { get; set; }

    [Column("customer_id")]
    public int? CustomerId { get; set; }

    [Column("post_name")]
    [StringLength(255)]
    public string PostName { get; set; } = null!;

    [Column("description", TypeName = "text")]
    public string Description { get; set; } = null!;

    [Column("price", TypeName = "float(6,2)")]
    public float Price { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Posts")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("Posts")]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
