using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    string provider = Environment.GetEnvironmentVariable("DatabaseProvider");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    switch (provider)
    {
        case "sqlite":
            options.UseSqlite(connectionString);
            break;
        case "postgres":
            options.UseNpgsql(connectionString);
            break;
        case "mssql":
            options.UseSqlServer(connectionString);
            break;
        case "mysql":
#pragma warning disable CS8604 // Possible null reference argument.
            options.UseMySQL(connectionString);
#pragma warning restore CS8604 // Possible null reference argument.
            break;
        case "oraclesql":
            options.UseOracle(connectionString);
            break;
        default:
            options.UseSqlite(connectionString); // Use SQLite by default
            break;
    }
});

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
