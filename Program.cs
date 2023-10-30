using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductManagerWebAPI.Data;
using System.Reflection;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Måste läggas till för auth
builder.Services
  .AddAuthentication()
  .AddJwtBearer(options =>
    {
        // Här används signeringsnyckeln för att verifiera att token inte har 
        // manipulerats på vägen (av klienten, eller av någon annan som vill attackera/utnyttja)
        // API:et

        //komma åt vår signeringsnyckel i inställningsfilen appsettings.Developement.json  ?? "" betyder att den kan va null.
        var signingKey = Convert.FromBase64String(builder.Configuration["Jwt:SigningSecret"] ?? "");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey)
        };
    });

//Detta registrerar ApplicationDbContext i DI-containern (DI = dependency injection) samt 
//ställer även in var ApplicationDbContext kan hitta vår connection string ("Default").
//"Default" är enbart ett namn eller etikett som identifierar en specifik connection string 
//- vi behöver lägga till denna filen .\appsettings.Development.json.

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));


builder.Services.AddControllers();

// från här - dokumentation
builder.Services.AddSwaggerGen(options =>
{

  options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
  {
    Title = "Product-Manager API",
    Version = "1.0",
    Description = "API for Product-Manager"
  });

  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer abc123'",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer"
  });

  options.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
            {
              Type = ReferenceType.SecurityScheme,
              Id = "Bearer"
            },
          Scheme = "oauth2",
          Name = "Bearer",
          In = ParameterLocation.Header,
        },
        new List<string>()
      }
  });

  // Dessa två behövs för att generera dokumentation utifrån XML-kommentarer (///)
  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});

// till hit - dokumentation

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
