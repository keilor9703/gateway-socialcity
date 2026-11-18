using System.Text;
using GatewaySocialCity.Models;
using GatewaySocialCity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configuración (appsettings)
builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
builder.Services.Configure<FalconCadOptions>(builder.Configuration.GetSection("FalconCad"));
builder.Services.Configure<SecadOptions>(builder.Configuration.GetSection("Secad"));

builder.Services.AddControllers();
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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
