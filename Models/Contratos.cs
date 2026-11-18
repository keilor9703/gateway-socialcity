using System.Text.Json.Serialization;

namespace GatewaySocialCity.Models
{
    // ============================================================
    // Contrato que FalconCAD REQUIERE recibir (flujo SECAD -> FalconCAD)
    // POST https://falconcad.com.co/api/integracion/casos  (header x-api-key)
    // ============================================================
    public class CasoFalconCadRequest
    {
        /// <summary>Código del catálogo del municipio. Obligatorio.</summary>
        [JsonPropertyName("codigoCaso")] public string CodigoCaso { get; set; } = "";
        /// <summary>Si falta, FalconCAD usa la descripción del código.</summary>
        [JsonPropertyName("titulo")] public string? Titulo { get; set; }
        /// <summary>El relato del caso.</summary>
        [JsonPropertyName("descripcion")] public string? Descripcion { get; set; }
        /// <summary>Id del caso en SECAD (se anexa a la descripción).</summary>
        [JsonPropertyName("referencia")] public string? Referencia { get; set; }
        [JsonPropertyName("ciudadano")] public string? Ciudadano { get; set; }
        [JsonPropertyName("telefono")] public string? Telefono { get; set; }
        [JsonPropertyName("lat")] public double? Lat { get; set; }
        [JsonPropertyName("lng")] public double? Lng { get; set; }
    }

    // ============================================================
    // Contrato que SECAD REQUIERE recibir (flujo FalconCAD -> SECAD)
    // (mismo modelo cabecera + Payload_Json usado en la integración PIP/PremierOne)
    // ============================================================
    public class CasoSecadRequest
    {
        [JsonPropertyName("SourceId")] public int SourceId { get; set; }
        [JsonPropertyName("SourceSystem")] public string SourceSystem { get; set; } = "";
        [JsonPropertyName("RecordingDb")] public int RecordingDb { get; set; }
        /// <summary>Identificador único del caso en el origen (idempotencia).</summary>
        [JsonPropertyName("CallNumber")] public string CallNumber { get; set; } = "";
        [JsonPropertyName("Payload_Json")] public PayloadSecad Payload_Json { get; set; } = new PayloadSecad();
        [JsonPropertyName("FUENTE_TRANSACCION")] public string? FUENTE_TRANSACCION { get; set; }
        [JsonPropertyName("SISTEMA_INFORMACION")] public string? SISTEMA_INFORMACION { get; set; }
        [JsonPropertyName("IP_MAQUINA")] public string? IP_MAQUINA { get; set; }
        [JsonPropertyName("USUARIO")] public string? USUARIO { get; set; }
    }

    public class PayloadSecad
    {
        [JsonPropertyName("SitioGraba")] public int SitioGraba { get; set; }
        [JsonPropertyName("NumeLlamada")] public string NumeLlamada { get; set; } = "";
        /// <summary>Formato dd/MM/yyyy HH:mm:ss.</summary>
        [JsonPropertyName("HoraCaso")] public string HoraCaso { get; set; } = "";
        [JsonPropertyName("CeldaMarcacion")] public string? CeldaMarcacion { get; set; }
        [JsonPropertyName("NumeTelefono")] public long NumeTelefono { get; set; }
        [JsonPropertyName("PropTelefono")] public string? PropTelefono { get; set; }
        [JsonPropertyName("DireTelefono")] public string? DireTelefono { get; set; }
        /// <summary>Municipio del teléfono (distinto del municipio del caso).</summary>
        [JsonPropertyName("LugeTelefono")] public int LugeTelefono { get; set; }
        [JsonPropertyName("BarrTelefono")] public string? BarrTelefono { get; set; }
        /// <summary>Código 2 dígitos. Sujeto a homologación.</summary>
        [JsonPropertyName("TipoPedido")] public string? TipoPedido { get; set; }
        [JsonPropertyName("CaliPedido")] public string? CaliPedido { get; set; }
        [JsonPropertyName("NombLlamante")] public string? NombLlamante { get; set; }
        [JsonPropertyName("DireLlamante")] public string? DireLlamante { get; set; }
        /// <summary>Municipio del CASO (rutea en SECAD). Obligatorio.</summary>
        [JsonPropertyName("LugeCodigo")] public int LugeCodigo { get; set; }
        [JsonPropertyName("DescMpio")] public string? DescMpio { get; set; }
        [JsonPropertyName("DireCaso")] public string? DireCaso { get; set; }
        [JsonPropertyName("PucaCodigo")] public int PucaCodigo { get; set; }
        [JsonPropertyName("CodiBarrio")] public int CodiBarrio { get; set; }
        [JsonPropertyName("LugeBarrio")] public int LugeBarrio { get; set; }
        [JsonPropertyName("DescBarrio")] public string? DescBarrio { get; set; }
        /// <summary>Coordenada con punto decimal.</summary>
        [JsonPropertyName("LatitudCaso")] public string? LatitudCaso { get; set; }
        [JsonPropertyName("LongitudCaso")] public string? LongitudCaso { get; set; }
        [JsonPropertyName("Comentario")] public string? Comentario { get; set; }
        /// <summary>Código de clasificación del incidente. Sujeto a homologación.</summary>
        [JsonPropertyName("CodiPedido")] public string? CodiPedido { get; set; }
        [JsonPropertyName("Estado")] public string? Estado { get; set; }
        [JsonPropertyName("Enviar")] public string? Enviar { get; set; }
        [JsonPropertyName("Usuario")] public string? Usuario { get; set; }
    }
}
