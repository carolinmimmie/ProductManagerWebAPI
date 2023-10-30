using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProductManagerkWebAPI.Domain;


[Index(nameof(UserName), IsUnique = true)]
public class User
{
    public int Id { get; set; }
    [MaxLength(50)]
    public required string UserName { get; set; }

    [MaxLength(50)]
    public required string Password { get; set; }

    [MaxLength(50)]
    public required string FirstName { get; set; }

    [MaxLength(50)]
    public required string LastName { get; set; }


}