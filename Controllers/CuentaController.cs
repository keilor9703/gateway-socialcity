using GatewaySocialCity.Models;
using GatewaySocialCity.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewaySocialCity.Controllers
{
    /// <summary>
    /// Autenticación del gateway (estilo PIP). El cliente pide un token con usuario/clave
    /// (quemados en appsettings) y luego usa ese token (Bearer) para llamar a los endpoints
    /// de integración.
    /// </summary>
    [ApiController]
    [Route("api/Cuenta")]
    public class CuentaController : ControllerBase
    {
        private readonly ITokenService _tokens;

        public CuentaController(ITokenService tokens)
        {
            _tokens = tokens;
        }

        [HttpPost("Token")]
        public IActionResult Token([FromBody] TokenRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Usuario))
            {
                return BadRequest(new { codigo = 0, estado = false, mensaje = "Usuario/clave requeridos" });
            }

            var (ok, token, expira) = _tokens.Emitir(req.Usuario, req.Clave);
            if (!ok)
            {
                return Unauthorized(new { codigo = 0, estado = false, mensaje = "Credenciales inválidas" });
            }

            return Ok(new
            {
                codigo = 1,
                mensaje = "Token generado correctamente",
                operacion = "GatewaySocialCity.Auth.Token",
                estado = true,
                respuesta = new { token, expiraUtc = expira }
            });
        }
    }
}
