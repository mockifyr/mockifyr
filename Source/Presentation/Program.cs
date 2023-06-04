using Domain.Entities.Identity;
using Domain.Wrappers;
using Infrastructure.Identity.Contexts;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Variables
ConfigurationManager configuration = builder.Configuration;

JwtOptions _jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
string _corsOrigins = configuration["AllowedHosts"];
byte[] secret = Encoding.UTF8.GetBytes(_jwtOptions.Secret);
#endregion

// Add services to the container.

#region General Definitions
builder.Services.AddSingleton(_jwtOptions);

builder.Services.AddControllers();
// Add - Endpoints Api Explorer
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add - Swagger Gen
builder.Services.AddSwaggerGen();

// Add - Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins(_corsOrigins).AllowAnyHeader().WithMethods("GET", "PUT", "POST", "DELETE", "UPDATE", "OPTIONS");
        });
});
#endregion

#region Db Contexts
// Add Db Context - Application Db Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    string connectionString = Environment.GetEnvironmentVariable("DB_URL");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    string provider = Environment.GetEnvironmentVariable("DB");
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

// Add Db Context - Identity Context
builder.Services.AddDbContext<IdentityContext>(options =>
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    string provider = Environment.GetEnvironmentVariable("DB");
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
#endregion

#region Auth
// Add - Identity
builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

// Add - Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Audience = _jwtOptions.Audience;
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
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
