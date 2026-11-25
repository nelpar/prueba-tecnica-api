using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace PruebaTecnica.Api.Security;

public class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private const string HeaderName = "X-API-KEY";

    private readonly byte[] _apiKey = Encoding.UTF8.GetBytes(
        configuration["ApiKey"] ?? throw new InvalidOperationException("Falta 'ApiKey' en appsettings.json"));

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey) ||
            !CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(providedKey.ToString()), _apiKey))
        {
            await Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "No autorizado",
                detail: $"Falta el header {HeaderName} o su valor es incorrecto.")
                .ExecuteAsync(context);
            return;
        }

        await next(context);
    }
}