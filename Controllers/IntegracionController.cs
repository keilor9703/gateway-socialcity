using System.Text.Encodings.Web;
using System.Text.Json;
using GatewaySocialCity.Models;
using GatewaySocialCity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewaySocialCity.Controllers
{
    /// <summary>
    /// Endpoints de integración (protegidos con el JWT del gateway).
    ///
    /// Flujos:
    ///  - SECAD   -> [Gateway] /api/Integracion/EnviarAFalcon -> FalconCAD
    ///  - FalconCAD -> [Gateway] /api/Integracion/EnviarASecad -> SECAD
    ///
    /// La llamada a SECAD (EnviarASecad) sale desde esta instancia, cuya IP elástica
    /// (34.231.219.210) es la única habilitada en SECAD para el flujo inverso.
    /// </summary>
    [ApiController]
    [Route("api/Integracion")]
    public class IntegracionController : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions
        {
            // Respeta tildes/ñ y no re-escapa caracteres (texto en español intacto).
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private readonly IFalconCadClient _falcon;
        private readonly ISecadClient _secad;
        private readonly ILogger<IntegracionController> _log;

        public IntegracionController(IFalconCadClient falcon, ISecadClient secad, ILogger<IntegracionController> log)
        {
            _falcon = falcon;
            _secad = secad;
            _log = log;
        }

        /// <summary>Salud del servicio (sin autenticación).</summary>
        [HttpGet("~/api/health")]
        [AllowAnonymous]
        public IActionResult Health() => Ok(new { estado = "ok", servicio = "GatewaySocialCity", utc = DateTime.UtcNow });

        /// <summary>
        /// SECAD -> FalconCAD. Recibe el caso en el contrato de FalconCAD y lo radica allí (x-api-key).
        /// </summary>
        [HttpPost("EnviarAFalcon")]
        [Authorize]
        public async Task<IActionResult> EnviarAFalcon([FromBody] CasoFalconCadRequest caso)
        {
            string body = JsonSerializer.Serialize(caso, JsonOpts);
            _log.LogInformation("EnviarAFalcon: codigoCaso={Codigo} ref={Ref}", caso.CodigoCaso, caso.Referencia);
            var (status, cuerpo) = await _falcon.RadicarCasoAsync(body);
            return Reenviar(status, cuerpo);
        }

        /// <summary>
        /// FalconCAD -> SECAD. Recibe el caso en el contrato de SECAD y lo envía a SECAD
        /// (la llamada sale por la IP elástica habilitada, requisito del flujo inverso).
        /// </summary>
        [HttpPost("EnviarASecad")]
        [Authorize]
        public async Task<IActionResult> EnviarASecad([FromBody] CasoSecadRequest caso)
        {
            string body = JsonSerializer.Serialize(caso, JsonOpts);
            _log.LogInformation("EnviarASecad: callNumber={Call} luge={Luge}", caso.CallNumber, caso.Payload_Json?.LugeCodigo);
            var (status, cuerpo) = await _secad.EnviarCasoAsync(body);
            return Reenviar(status, cuerpo);
        }

        private IActionResult Reenviar(System.Net.HttpStatusCode status, string cuerpo)
        {
            return new ContentResult
            {
                StatusCode = (int)status,
                Content = cuerpo,
                ContentType = "application/json"
            };
        }
    }
}
