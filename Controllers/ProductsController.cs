//skapa en kontroller som kan hantera inkommande anrop från en klient (t.ex. webbläsare).

using ProductManagerWebAPI.Domain;
using Microsoft.AspNetCore.Mvc;
using ProductManagerWebAPI.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace ProductManagerWebAPI.Controllers;

// GET | POST | PUT | DELETE ... /students -> StudentsController
// Ge klitenten meddelande Status: 401 Unauthorized om de inte är inloggade
[Authorize]
//Detta är en attribut som märker klassen StudentsController som en Web API-kontroller. 
[ApiController]
// Detta är ett attribut som används för att definiera ruttens basväg för denna kontroller.
[Route("[controller]")]

// Detta deklarerar själva kontrollerklassen och ärver från ControllerBase. 
// ControllerBase är en grundläggande kontrollerklass som används för API:er.
public class ProductsController : ControllerBase
{

    // readonly betyder att värdet av context inte kan ändras efter att det har tilldelats i konstruktorn.
    private readonly ApplicationDbContext context;


    // Detta är konstruktorn för StudentsController. 
    // Den tar in en parameter av typen ApplicationDbContext
    // och använder den för att sätta värdet på det privata fältet context. 
    public ProductsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    // POST /products
    // {
    //   "name": "Vera",
    //   "sku": "AAA111",
    //   "description": "Black leather jacket",
    //   "image": "img//url"
    //   "price": 199
    // }
    //Detta attribut anger att metoden svarar på HTTP POST-förfrågningar. 
    //Det betyder att denna metod används för att skicka data till servern 
    //för att skapa en ny resurs, i det här fallet en ny produkt.
    /// <summary>
    /// Create product 
    /// </summary>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<ProductDto> CreateProduct(CreateProductRequest createProductRequest)
    {
        // Check if SKU already exists in the database
        if (context.Products.Any(x => x.Sku == createProductRequest.Sku))
        {
            // SKU already exists, return a 400 Bad Request response
            return BadRequest("SKU already exists");
        }

        // Här skapas ett nytt objekt av typen Product baserat på 
        //informationen som kommer från createProductRequest, som antas 
        //vara en modell som innehåller uppgifter om den nya produkten.
        var product = new Product
        {
            Name = createProductRequest.Name,
            Sku = createProductRequest.Sku,
            Description = createProductRequest.Description,
            Image = createProductRequest.Image,
            Price = createProductRequest.Price
        };

        // Produkten läggs till i kontextens uppsättning av Products, och 
        //SaveChanges anropas för att spara ändringarna i databasen. 

        context.Products.Add(product);

        context.SaveChanges();

        // Efter att produkten har sparats i databasen skapas ett nytt objekt av typen ProductDto.
        // Detta används för att skicka information om den skapade produkten tillbaka till klienten.
        var productDto = new ProductDto
        {
            Id = product.Id, // denna kan tas bort? 
            Name = product.Name,
            Sku = product.Sku,
            Description = product.Description,
            Image = product.Image,
            Price = product.Price
        };
        // Returnera en HTTP 201 Created-respons till klienten och inkludera ProductDto i svaret.
        return Created("", productDto); //productDto returneras i  bodyn
    }





    // GET /products          -  hämta alla produkter
    // GET /products?name=Dino -  hämta alla produkter av specifikt namn
    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IEnumerable<ProductDto> GetProducts([FromQuery] string? name)//string? säger att den kan va null eller namn
    {

        IEnumerable<Product> products;

        if (string.IsNullOrEmpty(name))
        {
            products = context.Products.ToList(); // Hämta alla produkter från databasen
        }
        else
        {
            products = context.Products.Where(x => x.Name == name);
        }

        // Konvertera produkterna till ProductDto-format
        var productsDto = products.Select(x => new ProductDto//skapar en ny samling av StudentDto
        {
            Id = x.Id,
            Name = x.Name,
            Sku = x.Sku,
            Description = x.Description,
            Image = x.Image,
            Price = x.Price
        });

        return productsDto; // 200 OK
    }
    /// <summary>
    /// Get product
    /// </summary>
    // GET /products/{sku}
    [HttpGet("{sku}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductDto> GetProduct(string sku)
    {
        var product = context.Products.FirstOrDefault(x => x.Sku == sku);//letar efter produkten

        if (product is null) // om produkten inte finns
            return NotFound(); // returnera 404 Not Found

        var productDto = new ProductDto // om produkten fanns vill vi mappa detaljerna från den studerande till en Dto
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Description = product.Description,
            Image = product.Image,
            Price = product.Price
        };

        return productDto; //retunera 200 OK plus informationen ovan om produkten
    }
    /// <summary>
    /// Delete product
    /// </summary>
    // DELETE /products/{id}
    [HttpDelete("{sku}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult DeleteProduct(string sku)
    {
        var product = context.Products.FirstOrDefault(x => x.Sku == sku);

        if (product is null)
            return NotFound(); // 404 Not Found        

        context.Products.Remove(product);

        // SQL DELETE skickas till databasen för att radera den studerande
        context.SaveChanges();

        return NoContent(); // 204 No Content
    }

    // DET SOM SKICKAS IN I JSON-DATAN SOM SKICKAS IN I BODY I THUNDER CLIENT
    // PUT /products/sku
    // 
    // {
    //   "id": 1,
    //   "name": "Vera",
    //   "sku": "111BBB",
    //   "description": "19900101-2010",
    //   "image": "jane@outlook.com",
    //   "image": "URL//img"

    // }

    // [HttpPut("{sku}")]: Denna attribut indikerar att metoden svarar på HTTP PUT-förfrågningar
    // och har en URL-route-parametrar {sku}. Det innebär att metoden förväntar sig ett SKU-värde som en del av URL:en.
    /// <summary>
    /// Update product 
    /// </summary>
    [HttpPut("{sku}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    // Metoden tar emot två parametrar: SKU som en sträng och en updateProductRequest som innehåller 
    // den inkommande data från klienten. IActionResult används för att ange att metoden kommer att returnera en HTTP-respons.
    public IActionResult UpdateProduct(string sku, updateProductRequest updateProductRequest) // sku och inkommande data
    {
        // Kollar om SKU från URL:en inte matchar SKU från den inkommande datan.
        // Om det är fallet returneras en HTTP 400 Bad Request med meddelandet 
        // "Sku does not match".
        if (sku != updateProductRequest.Sku)
            return BadRequest("Sku does not match");

        // Hämtar produkten från databasen baserat på SKU-värdet från URL:en.
        var product = context.Products.FirstOrDefault(x => x.Sku == sku);

        // Om produkten inte finns i databasen returneras en HTTP 404 Not Found.
        if (product is null)
            return NotFound();

        // Om produkten finns uppdateras med nya värden från updateProductRequest.
        product.Name = updateProductRequest.Name;
        product.Sku = updateProductRequest.Sku;
        product.Description = updateProductRequest.Description;
        product.Image = updateProductRequest.Image;
        product.Price = updateProductRequest.Price;

        // Sparar ändringarna i databasen.
        context.SaveChanges();
        // Returnerar en HTTP 204 No Content-status för att indikera
        // att åtgärden har utförts framgångsrikt men att ingen ny data returneras.
        return NoContent();
    }
}

public class updateProductRequest // fångar upp informationen som kommer in ifrån klienten i PUT
{
    public int Id { get; set; } // ta bort id?

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } //dessa properties fångar upp värdena som kommer in från klienten

    [Required]
    [MaxLength(20)]
    public string Sku { get; set; }

    [Required]
    [MaxLength(50)]
    public string Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Image { get; set; }

    [Required]
    public required decimal Price { get; set; }
}

// Dto-klass som returneras till klienten - response
public class ProductDto
{
    public int Id { get; set; } // Ta bort id? 
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } //dessa properties fångar upp värdena som kommer in från klienten

    [Required]
    [MaxLength(20)]
    public string Sku { get; set; }

    [Required]
    [MaxLength(50)]
    public string Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Image { get; set; }

    [Required]
    public required decimal Price { get; set; }
}


//Dto-klass som lägger till produkt - request
public class CreateProductRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } //dessa properties fångar upp värdena som kommer in från klienten

    [Required]
    [MaxLength(20)]
    public string Sku { get; set; }

    [Required]
    [MaxLength(50)]
    public string Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Image { get; set; }

    [Required]
    public required decimal Price { get; set; }
}
