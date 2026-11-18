namespace GatewaySocialCity.Models
{
    /// <summary>
    /// Credenciales del propio gateway (quemadas en appsettings) y parámetros del token JWT
    /// que el gateway emite. Los clientes (SECAD / FalconCAD) piden el token con estas
    /// credenciales y luego llaman a los endpoints del gateway con ese token (estilo PIP).
    /// </summary>
    public class GatewayOptions
    {
        public string Usuario { get; set; } = "";
        public string Clave { get; set; } = "";
        public JwtOptions Jwt { get; set; } = new JwtOptions();
    }

    public class JwtOptions
    {
        /// <summary>Clave simétrica para firmar/validar el JWT. Mínimo 32 caracteres.</summary>
        public string ClaveFirma { get; set; } = "";
        public string Emisor { get; set; } = "GatewaySocialCity";
        public string Audiencia { get; set; } = "IntegracionSecadFalcon";
        public int MinutosVigencia { get; set; } = 60;
    }

    /// <summary>Destino FalconCAD (flujo SECAD -> FalconCAD).</summary>
    public class FalconCadOptions
    {
        public string Url { get; set; } = "";
        public string ApiKey { get; set; } = "";
    }

    /// <summary>
    /// Destino SECAD (flujo FalconCAD -> SECAD). La autenticación hacia SECAD se deja
    /// configurable por header (nombre/valor) por si SECAD exige token o api-key propio.
    /// </summary>
    public class SecadOptions
    {
        public string Url { get; set; } = "";
        public string AuthHeaderNombre { get; set; } = "";
        public string AuthHeaderValor { get; set; } = "";
    }
}
