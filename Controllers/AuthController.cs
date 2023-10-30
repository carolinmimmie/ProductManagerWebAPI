using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductManagerkWebAPI.Domain;
using ProductManagerWebAPI.Data;

namespace ProductManagerWebAP.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IConfiguration config; // är ett interface - config är ett objekt gör så vi kommer åt configationen

    public AuthController(ApplicationDbContext context, IConfiguration config) // hämta vårat context
    {
        this.context = context;
        this.config = config;
    }
    /// <summary>
    /// Log in with username and password
    /// </summary>
    //VÅR ENDA OPERATION - ACTION METOD
    [HttpPost]
    [Consumes("application/json")] // datan som kommer in i api:et
    [Produces("application/json")] // datan som kommer ut från api:et
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<TokenDto> Authenticate(AuthenticateRequest authenticateRequest) // returnera TokenDto
    {
        // 1 - Kontroller om användaren finns (användarnamn + lösenord stämmer)
        var user = context
            .Users
            .FirstOrDefault(x => x.UserName == authenticateRequest.UserName
                && x.Password == authenticateRequest.Password);

        // 1:1 - Om användaren inte finns, returnera 401 Unauthorized
        if (user is null)
        {
            return Unauthorized(); // 401 Unauthorized
        }
        // RETURNERA ARMBANDET!!!!!!!!!
        // 1:2 - Om användaren finns, generera token (JWT = JSON Web Token) och returnera denna - ALLTSÅ ARMBANDET!!!!!
        var tokenDto = GenerateToken(user);

        // SÄTT BREAKPOINT
        // RETURNERA ARMBANDET!!!!!!!!!
        return tokenDto; // 200 OK
    }

    //GENERERAR VÅR TOKEN
    private TokenDto GenerateToken(User user) // Skicka in användaren i vår token
    {
        // signeringsnyckel = används för att generera token och virifera att token inte är manipulerad är användare
        var signingKey = Convert.FromBase64String(config["Jwt:SigningSecret"] ?? "");

        var claims = new List<Claim> // lista av claims
        {
            new Claim(ClaimTypes.GivenName,user.FirstName),//kommer åt användarens förnamn 
            new Claim(ClaimTypes.Surname,user.LastName)//och efternamn
        };

        // Kod som behöver finnas bara
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(signingKey),
            SecurityAlgorithms.HmacSha256Signature),
            Subject = new ClaimsIdentity(claims)//lägger till våra claim i vår token
        };

        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var jwtSecurityToken = jwtTokenHandler
          .CreateJwtSecurityToken(tokenDescriptor);

        var token = new TokenDto
        {
            // Generera token (t.ex. "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
            Token = jwtTokenHandler.WriteToken(jwtSecurityToken)
        };

        return token;
    }
}
// token som vi skickar tillbaka
public class TokenDto
{
    [Required]
    public string Token { get; set; }
}

// ancändarnamn och lösenord som skickas in till webAPi.
public class AuthenticateRequest
{
    [Required]
    [MaxLength(50)]

    public string UserName { get; set; }
    [Required]
    [MaxLength(50)]
    public string Password { get; set; }

}
// AuthenticateRequest är en enkel C#-klass som representerar data för autentiseringsbegäran.
// Den har två egenskaper (UserName och Password) som antas användas för att överföra användarnamn 
//och lösenord från klienten till servern under autentiseringsprocessen.

