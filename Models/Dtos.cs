namespace GatewaySocialCity.Models
{
    /// <summary>Body del endpoint de autenticación (POST /api/Cuenta/Token).</summary>
    public class TokenRequest
    {
        public string Usuario { get; set; } = "";
        public string Clave { get; set; } = "";
    }
}
