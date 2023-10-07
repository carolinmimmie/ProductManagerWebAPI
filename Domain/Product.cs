
//Vi behöver en modell / entitet som representerar en produk.
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProductManagerWebAPI.Domain;

//required eller konstruktor gör det obligatoriskt att alla fält måste fyllas i 
[Index(nameof(Sku), IsUnique = true)]//Sku-värden är unika i din databas.
public class Product
{
    public int Id { get; set; }
    // auto-implemented properties som har get och set metoder
    [Required]
    [MaxLength(50)]//bestämd längd
    public required string Name { get; set; }

    [Required]
    [MaxLength(20)]
    public required string Sku { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Description { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Image { get; set; }

    [Required]
    [MaxLength(20)]
    public required decimal Price { get; set; }

}