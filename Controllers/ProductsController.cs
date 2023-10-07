//skapa en kontroller som kan hantera inkommande anrop från en klient (t.ex. webbläsare).

using ProductManagerWebAPI.Domain;
using Microsoft.AspNetCore.Mvc;
using ProductManagerWebAPI.Data;

namespace ProductManagerWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
  private readonly ApplicationDbContext context;

  public ProductsController(ApplicationDbContext context)
  {
    this.context = context;
  }

// Följande kommer nu att ske när det kommer in ett HTTP GET-anrop från klienten till /products
// ●	En instans av ProductsController skapas för att hantera anropet. 
// ●	Konstruktorn för ProductsController vill ha in en instans av ApplicationDbContext.
// ●	DI-containern skapar en sådan instans automatiskt och skickar in denna i konstruktorn för ProductsController.
// ●	Därefter anropas GetProducts() då denna är markerad med [HttpGet].

[HttpGet]
public IEnumerable<Product> GetProducts()
{
  //Vi kan nu använda vårt DbContext för att hämta filmer från databasen, för att sen slutligen returnera dessa till klienten:  
  var products = context.Product.ToList();

  return products;
}

}