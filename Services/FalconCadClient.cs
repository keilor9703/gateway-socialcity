using System.Net;
using System.Text;
using GatewaySocialCity.Models;
using Microsoft.Extensions.Options;

namespace GatewaySocialCity.Services
{
    public interface IFalconCadClient
    {
        /// <summary>Radica el caso en FalconCAD (header x-api-key). Devuelve status y cuerpo.</summary>
        Task<(HttpStatusCode status, string cuerpo)> RadicarCasoAsync(string jsonBody);
    }

    public class FalconCadClient : IFalconCadClient
    {
        private readonly HttpClient _http;
        private readonly FalconCadOptions _opt;
        private readonly ILogger<FalconCadClient> _log;

        public FalconCadClient(HttpClient http, IOptions<FalconCadOptions> opt, ILogger<FalconCadClient> log)
        {
            _http = http;
            _opt = opt.Value;
            _log = log;
        }

        public async Task<(HttpStatusCode, string)> RadicarCasoAsync(string jsonBody)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, _opt.Url);
            req.Content = new StringContent(jsonBody ?? "", Encoding.UTF8, "application/json");
            req.Headers.TryAddWithoutValidation("x-api-key", _opt.ApiKey);

            using var resp = await _http.SendAsync(req);
            string cuerpo = await resp.Content.ReadAsStringAsync();
            _log.LogInformation("FalconCAD respondió {Status}", (int)resp.StatusCode);
            return (resp.StatusCode, cuerpo);
        }
    }
}
