using Infrastructure.Identity;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

string secretStr = configuration["Application:Secret"];
byte[] secret = Encoding.UTF8.GetBytes(secretStr);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

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

builder.Services.AddDbContext<IdentityContext>(options =>
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

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Audience = configuration["Application:Audience"];
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = new SymmetricSecurityKey(secret),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x.SetIsOriginAllowed(origin => true).AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
