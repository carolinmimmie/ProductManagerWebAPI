//skapa en kontroller som kan hantera inkommande anrop från en klient (t.ex. webbläsare).

using ProductManagerWebAPI.Domain;
using Microsoft.AspNetCore.Mvc;
using ProductManagerWebAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace ProductManagerWebAPI.Controllers;

// GET | POST | PUT | DELETE ... /students -> StudentsController

//Detta är en attribut som märker klassen StudentsController som en Web API-kontroller. 
[ApiController]
// Detta är ett attribut som används för att definiera ruttens basväg för denna kontroller.
[Route("[controller]")]

//Detta deklarerar själva kontrollerklassen och ärver från ControllerBase. 
//ControllerBase är en grundläggande kontrollerklass som används för API:er.
public class ProductsController : ControllerBase
{

    // Detta skapar ett privat fält (context) som håller en instans av ApplicationDbContext.
    // ApplicationDbContext används för att interagera med databasen. readonly betyder att 
    // värdet av context inte kan ändras efter att det har tilldelats i konstruktorn.
    private readonly ApplicationDbContext context;


    // Detta är konstruktorn för StudentsController. 
    // Den tar in en parameter av typen ApplicationDbContext
    // och använder den för att sätta värdet på det privata fältet context. 
    // Konstruktorn spelar en viktig roll när en ny instans av StudentsController skapas,
    // eftersom den används för att ställa in objektet med den nödvändiga databaskontexten.
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
    [HttpPost]
    public ActionResult<ProductDto> CreateProduct(CreateProductRequest createProductRequest)
    {
        // 1 - Skapa ett objekt av typ Student och kopiera över värden från createStudentRequest
        var product = new Product
        {
            Name = createProductRequest.Name,
            Sku = createProductRequest.Sku,
            Description = createProductRequest.Description,
            Image = createProductRequest.Image,
            Price = createProductRequest.Price
        };

        // 2 - Spara studerande till databasen

        context.Product.Add(product);

        context.SaveChanges();

        // 3 - För över information från entitet till DTO och returnera till klienten
        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Description = product.Description,
            Image = product.Image,
            Price = product.Price
        };

        return Created("", productDto); // 201 Created
    }

    // GET /products          -  hämta alla produkter
    // GET /products?name=Dino -  hämta alla produkter av specifikt namn
    [HttpGet]
    public IEnumerable<ProductDto> GetProducts([FromQuery] string? name)//string? säger att den kan va null eller namn
    {

        IEnumerable<Product> products;

        if (string.IsNullOrEmpty(name))
        {
            products = context.Product.ToList(); // Hämta alla produkter från databasen
        }
        else
        {
            products = context.Product.Where(x => x.Name == name);
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

    // GET /products/{sku}
    [HttpGet("{sku}")]
    public ActionResult<ProductDto> GetProduct(string sku)
    {
        var product = context.Product.FirstOrDefault(x => x.Sku == sku);//letar efter den studerande 

        if (product is null) // om studerander inte finns
            return NotFound(); // returnera 404 Not Found

        var productDto = new ProductDto // om den studerande fanns vill vi mappa detaljerna från den studerande till en Dto
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Description = product.Description,
            Image = product.Image,
            Price = product.Price
        };

        return productDto; //retuerna 200 OK plis informationen ovan om studerande
    }

    // DELETE /students/{id}
    [HttpDelete("{sku}")]
    public ActionResult DeleteProduct(string sku)
    {
        var product = context.Product.FirstOrDefault(x => x.Sku == sku);

        if (product is null)
            return NotFound(); // 404 Not Found        

        context.Product.Remove(product);

        // SQL DELETE skickas till databasen för att radera den studerande
        context.SaveChanges();

        return NoContent(); // 204 No Content
    }

    // DET SOM SKICKAS IN I JSON-DATAN SOM SKICKAS IN I BODY I THUNDER CLIENT
    // PUT /students/1
    // 
    // {
    //   "id": 1,
    //   "firstName": "Jane",
    //   "lastName": "Doe",
    //   "socialSecurityNumber": "19900101-2010",
    //   "email": "jane@outlook.com"
    // }
    [HttpPut("{sku}")]
    public IActionResult UpdateProduct(string sku, updateProductRequest updateProductRequest) // sku och inkommande data
    {
        if (sku != updateProductRequest.Sku) // om id från body inte stämmer överrens med updatedstudendRequest Id
            return BadRequest(); // returnera 400 Bad Request

        var product = context.Product.FirstOrDefault(x => x.Sku == sku); // letar efter id i databasen

        if (product is null) // om det inte hittas 
            return NotFound(); //  returnera 404 Not Found        

        // om den studerande fanns då vill vi uppdatera egenskaperna så de matchar datat vi fick in.
        product.Name = updateProductRequest.Name;
        product.Sku = updateProductRequest.Sku;
        product.Description = updateProductRequest.Description;
        product.Image = updateProductRequest.Image;
        product.Price = updateProductRequest.Price;

        // Skickar SQL UPDATE till databasen
        context.SaveChanges();

        return NoContent(); // 204 No Content
    }
}

public class updateProductRequest
{
    public int Id { get; set; }  // id behövs för att kunna hitta, jämföra och uppdatera fordon
    public string Name { get; set; }

    public string Sku { get; set; }

    public string Description { get; set; }

    public string Image { get; set; }

    public required decimal Price { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string Sku { get; set; }

    public string Description { get; set; }

    public string Image { get; set; }

    public required decimal Price { get; set; }
}


//Dto-klass
public class CreateProductRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Sku { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Image { get; set; }

    [Required]
    public required decimal Price { get; set; }
}
