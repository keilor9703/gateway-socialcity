# Gateway Social City — Intermediario SECAD ↔ FalconCAD

Servicio intermedio (gateway) para la integración **bidireccional** entre **SECAD** y **FalconCAD**.
Cumple el rol que la **PIP** cumple en la integración con PremierOne: expone su propio endpoint de
autenticación (JWT) y reenvía los casos al destino correspondiente.

```
SECAD     ──►  [Gateway]/api/Integracion/EnviarAFalcon  ──►  FalconCAD
FalconCAD ──►  [Gateway]/api/Integracion/EnviarASecad   ──►  SECAD
```

## ¿Por qué se hospeda en una instancia específica?

El flujo **FalconCAD → SECAD** solo es aceptado por SECAD desde la IP **`34.231.219.210`**
(IP elástica de la instancia `invrt-0001-sc-prod-iis-server`, Windows Server 2022 + IIS, en subred
pública). Como esa instancia tiene la EIP adjunta directamente, su tráfico de salida usa esa IP, por
lo que **el gateway debe ejecutarse en esa instancia** para que la llamada a SECAD tenga el origen
permitido. El sentido SECAD → FalconCAD no tiene restricción de IP.

## Autenticación (estilo PIP)

1. El cliente (SECAD o FalconCAD) pide un token:
   - `POST /api/Cuenta/Token`  body: `{ "usuario": "...", "clave": "..." }`
   - Respuesta: `{ codigo, mensaje, estado, respuesta: { token, expiraUtc } }`
2. El cliente llama a los endpoints de integración con `Authorization: Bearer <token>`.

Las credenciales del gateway y la clave de firma del JWT están en `appsettings.json`
(sección `Gateway`). **Cámbialas antes de desplegar.**

## Endpoints

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| POST | `/api/Cuenta/Token` | — | Emite el JWT del gateway. |
| POST | `/api/Integracion/EnviarAFalcon` | Bearer | SECAD → gateway → FalconCAD (agrega `x-api-key`). |
| POST | `/api/Integracion/EnviarASecad` | Bearer | FalconCAD → gateway → SECAD (sale por la IP habilitada). |
| GET | `/api/health` | — | Salud del servicio. |

El gateway **reenvía el cuerpo tal cual** al destino y devuelve su status/cuerpo. Si SECAD o
FalconCAD requieren una transformación del payload, se agrega en `SecadClient` / `FalconCadClient`.

## Configuración (`appsettings.json`)

- `Gateway.Usuario` / `Gateway.Clave`: credenciales para pedir el token del gateway.
- `Gateway.Jwt.ClaveFirma`: clave simétrica (mínimo 32 caracteres) para firmar el JWT.
- `FalconCad.Url` / `FalconCad.ApiKey`: destino FalconCAD y su `x-api-key`.
- `Secad.Url`: endpoint de SECAD que recibe los casos (flujo inverso). **Reemplazar.**
- `Secad.AuthHeaderNombre` / `Secad.AuthHeaderValor`: auth opcional hacia SECAD (si aplica).

> Para producción, usar `appsettings.Production.json` o variables de entorno (p. ej.
> `Gateway__Clave`, `FalconCad__ApiKey`, `Secad__Url`) y **no** versionar secretos reales.

## Build / publicación

Requiere SDK .NET 8.

```bash
dotnet restore
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

## Despliegue en IIS (Windows Server 2022)

1. Instalar el **ASP.NET Core 8 Hosting Bundle** en el servidor (incluye el módulo `AspNetCoreModuleV2`).
2. Copiar el contenido de `./publish` a una carpeta del servidor (p. ej. `C:\inetpub\GatewaySocialCity`).
3. En IIS, crear un **sitio** (o aplicación) apuntando a esa carpeta, con un **Application Pool** en modo
   **"No Managed Code"** (ASP.NET Core corre out-of-process/in-process vía el módulo, no el CLR de .NET Framework).
4. Ajustar `appsettings.Production.json` con las URLs, credenciales y llaves reales.
5. Enlaces (bindings): habilitar **HTTPS** con un certificado válido.

## Red / Seguridad (AWS Security Groups)

- **Entrada** al gateway (443): permitir el origen de SECAD y el de FalconCAD.
- **Salida** del gateway: a FalconCAD (`falconcad.com.co`) y a SECAD.
- En SECAD: confirmar que `34.231.219.210` esté habilitada también para el endpoint que recibe el flujo inverso.

## Pendientes a definir con las contrapartes

- **Contrato del flujo inverso** (FalconCAD → gateway → SECAD): payload que envía FalconCAD y el
  contrato exacto que espera SECAD (si requiere mapeo, va en `SecadClient`).
- **Auth hacia SECAD**: si SECAD exige token/api-key propio, configurarlo en `Secad.AuthHeader*`.
