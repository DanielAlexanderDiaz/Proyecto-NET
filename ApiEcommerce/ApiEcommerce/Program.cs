using System.Text;
using ApiEcommerce.Constants;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));

builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1 MB
    options.UseCaseSensitivePaths = true;
});

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("La clave secreta no está configurada");
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers(option =>
{
  option.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);
  option.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
  {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
      Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                    "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                    "Ejemplo: \"12345abcdef\"",
      Name = "Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.Http,
      Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
          In = ParameterLocation.Header
        },
        new List<string>()
      }
    }
    );
    options.SwaggerDoc("v1", new OpenApiInfo
    {
      Version = "v1",
      Title = "ApiEcommerce",
      Description = "ApiEcommerce es un proyecto de ejemplo para demostrar la implementación de una API RESTful utilizando ASP.NET Core y Entity Framework Core.",
      TermsOfService = new Uri("https://example.com/terms"),
      Contact = new OpenApiContact
      {
        Name = "Daniel",
        Email = "dan@dan.com",
        Url = new Uri("https://example.com/contact")
      },
      License = new OpenApiLicense
      {
        Name = "Licencia de ejemplo",
        Url = new Uri("https://example.com/license")
      }
    });
  }
);

var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
  options.AssumeDefaultVersionWhenUnspecified = true;
  options.DefaultApiVersion = new ApiVersion(1, 0);
  options.ReportApiVersions = true;
  // options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); //?api-version
});
apiVersioningBuilder.AddApiExplorer(options =>
{
  options.GroupNameFormat = "'v'VVV"; // v1, v2, etc.
  options.SubstituteApiVersionInUrl = true; // api/v1/values
});

// CORS
builder.Services.AddCors( options =>
{
    options.AddPolicy(PolicyNames.AllowSpecificOrigin, builder =>
    {
        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    });
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();

// CORS
app.UseCors(PolicyNames.AllowSpecificOrigin);

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
