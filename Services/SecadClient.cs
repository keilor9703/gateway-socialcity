using System.Net;
using System.Text;
using GatewaySocialCity.Models;
using Microsoft.Extensions.Options;

namespace GatewaySocialCity.Services
{
    public interface ISecadClient
    {
        /// <summary>
        /// Envía el caso a SECAD. Esta llamada sale desde la instancia con la IP elástica
        /// habilitada en SECAD (34.231.219.210), por eso el flujo inverso debe ejecutarse aquí.
        /// </summary>
        Task<(HttpStatusCode status, string cuerpo)> EnviarCasoAsync(string jsonBody);
    }

    public class SecadClient : ISecadClient
    {
        private readonly HttpClient _http;
        private readonly SecadOptions _opt;
        private readonly ILogger<SecadClient> _log;

        public SecadClient(HttpClient http, IOptions<SecadOptions> opt, ILogger<SecadClient> log)
        {
            _http = http;
            _opt = opt.Value;
            _log = log;
        }

        public async Task<(HttpStatusCode, string)> EnviarCasoAsync(string jsonBody)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, _opt.Url);
            req.Content = new StringContent(jsonBody ?? "", Encoding.UTF8, "application/json");

            // Autenticación hacia SECAD (opcional, configurable por header).
            if (!string.IsNullOrWhiteSpace(_opt.AuthHeaderNombre) && !string.IsNullOrWhiteSpace(_opt.AuthHeaderValor))
            {
                req.Headers.TryAddWithoutValidation(_opt.AuthHeaderNombre, _opt.AuthHeaderValor);
            }

            using var resp = await _http.SendAsync(req);
            string cuerpo = await resp.Content.ReadAsStringAsync();
            _log.LogInformation("SECAD respondió {Status}", (int)resp.StatusCode);
            return (resp.StatusCode, cuerpo);
        }
    }
}
