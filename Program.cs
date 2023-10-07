using Microsoft.EntityFrameworkCore;
using ProductManagerWebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

//Detta registrerar ApplicationDbContext i DI-containern (DI = dependency injection) samt 
//ställer även in var ApplicationDbContext kan hitta vår connection string ("Default").
//"Default" är enbart ett namn eller etikett som identifierar en specifik connection string 
//- vi behöver lägga till denna filen .\appsettings.Development.json.

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
