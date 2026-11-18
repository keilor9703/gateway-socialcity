using System.Security.Claims;
using System.Text;
using GatewaySocialCity.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GatewaySocialCity.Services
{
    public interface ITokenService
    {
        /// <summary>Valida las credenciales del gateway y, si son correctas, emite un JWT.</summary>
        (bool ok, string token, DateTime expiraUtc) Emitir(string usuario, string clave);
    }

    public class TokenService : ITokenService
    {
        private readonly GatewayOptions _opt;

        public TokenService(IOptions<GatewayOptions> opt)
        {
            _opt = opt.Value;
        }

        public (bool, string, DateTime) Emitir(string usuario, string clave)
        {
            if (!string.Equals(usuario, _opt.Usuario, StringComparison.Ordinal) ||
                !string.Equals(clave, _opt.Clave, StringComparison.Ordinal))
            {
                return (false, "", default);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Jwt.ClaveFirma));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expira = DateTime.UtcNow.AddMinutes(_opt.Jwt.MinutosVigencia);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _opt.Jwt.Emisor,
                Audience = _opt.Jwt.Audiencia,
                Expires = expira,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("usuario", usuario)
                }),
                SigningCredentials = credenciales
            };

            string token = new JsonWebTokenHandler().CreateToken(descriptor);
            return (true, token, expira);
        }
    }
}
