using Domain.Entities.Identity;
using Domain.Wrappers;
using Infrastructure.Identity.Contexts;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Presentation.Common.Filters;

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
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
    options.CustomSchemaIds(type => type.FullName);
    options.OperationFilter<SwaggerDefaultValuesFilter>();
    options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "Mockifyr API - V1" });
});

// Add - Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins(_corsOrigins).AllowAnyHeader().WithMethods("GET", "PUT", "POST", "DELETE", "UPDATE", "OPTIONS");
        });
});

#pragma warning disable CS0618 // Type or member is obsolete
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());
#pragma warning restore CS0618 // Type or member is obsolete
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

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlite("Data Source=/Data/database.db"));


// Add Db Context - Identity Context
//builder.Services.AddDbContext<IdentityContext>(options =>
//{
//#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
//    string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
//#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
//#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
//    string provider = Environment.GetEnvironmentVariable("DB");
//#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
//    switch (provider)
//    {
//        case "sqlite":
//            options.UseSqlite(connectionString);
//            break;
//        case "postgres":
//            options.UseNpgsql(connectionString);
//            break;
//        case "mssql":
//            options.UseSqlServer(connectionString);
//            break;
//        case "mysql":
//#pragma warning disable CS8604 // Possible null reference argument.
//            options.UseMySQL(connectionString);
//#pragma warning restore CS8604 // Possible null reference argument.
//            break;
//        case "oraclesql":
//            options.UseOracle(connectionString);
//            break;
//        default:
//            options.UseSqlite(connectionString); // Use SQLite by default
//            break;
//    }
//});
#endregion

#region Api Versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new QueryStringApiVersionReader("x-api-version"),
                                                    new HeaderApiVersionReader("x-api-version"),
                                                    new MediaTypeApiVersionReader("x-api-version"));
});

builder.Services.AddVersionedApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
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
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Mockifyr - {description.GroupName.ToUpper()}");
        }
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
