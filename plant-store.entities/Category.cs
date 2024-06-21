using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace plant_store.entities;

[Table("category")]
public partial class Category
{
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [InverseProperty("Categories")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
