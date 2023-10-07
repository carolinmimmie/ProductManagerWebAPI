//Vi behöver ett DbContext så att vi kan kommunicera med databasen.
using Microsoft.EntityFrameworkCore;
using ProductManagerWebAPI.Domain;

namespace ProductManagerWebAPI.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options)
  { }

  public DbSet<Product> Product { get; set; }
}
