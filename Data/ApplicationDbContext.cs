//Vi behöver ett DbContext så att vi kan kommunicera med databasen.
using Microsoft.EntityFrameworkCore;
using ProductManagerkWebAPI.Domain;
using ProductManagerWebAPI.Domain;

namespace ProductManagerWebAPI.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options)
  { }

  public DbSet<Product> Products { get; set; }
  public DbSet<User> Users { get; set; }
}
