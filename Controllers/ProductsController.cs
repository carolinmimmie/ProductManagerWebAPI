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

    // POST http://localhost:8000/products
    [HttpPost]
    public IActionResult AddProduct(Product product)
    {
        context.Product.Add(product);

        context.SaveChanges();

        return Created("", product);
    }
    // DELETE http://localhost:8000/products/ABC123
    [HttpDelete("{sku}")]
    public IActionResult DeleteProduct(string sku)
    {
        var product = context.Product.FirstOrDefault(x => x.Sku == sku);//Plocka ut produkt baserat på sku

        if (product == null)// om produkt inte finns
        {
            return NotFound(); // returnera 404 Not Found
        }

        context.Product.Remove(product);// om produkt finns så vill vi radera
        // Detta skickar en SQL DELETE till databashanteraren
        // Exempelvis "DELETE FROM prodct WHERE SKu = ABC123"
        context.SaveChanges();

        return NoContent(); // returnera204 No Content
    }




}