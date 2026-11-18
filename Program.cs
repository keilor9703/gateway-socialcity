using System.Text;
using GatewaySocialCity.Models;
using GatewaySocialCity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración (appsettings)
builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
builder.Services.Configure<FalconCadOptions>(builder.Configuration.GetSection("FalconCad"));
builder.Services.Configure<SecadOptions>(builder.Configuration.GetSection("Secad"));

builder.Services.AddControllers();

// Swagger / OpenAPI (con soporte de token Bearer para probar los endpoints protegidos)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gateway Social City", Version = "v1" });

    var esquema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue solo el token (sin la palabra 'Bearer'). Obténgalo en POST /api/Cuenta/Token."
    };
    c.AddSecurityDefinition("Bearer", esquema);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddHttpClient<IFalconCadClient, FalconCadClient>();
builder.Services.AddHttpClient<ISecadClient, SecadClient>();

// Autenticación JWT emitida por el propio gateway
var jwt = builder.Configuration.GetSection("Gateway:Jwt");
var claveFirma = jwt["ClaveFirma"] ?? "";
var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveFirma));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Emisor"],
            ValidateAudience = true,
            ValidAudience = jwt["Audiencia"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = llave,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Swagger habilitado (UI en /swagger). Si desea limitarlo a desarrollo, envuelva en
// if (app.Environment.IsDevelopment()) { ... }.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
