using System.Security.Cryptography;
using System.Text;

public class ExoscaleAuthHandler : DelegatingHandler
{
    private readonly string apiKey;
    private readonly string apiSecret;
    private readonly TimeSpan reqExpire;

    public ExoscaleAuthHandler(string apiKey, string apiSecret, TimeSpan? reqExpire = null)
    {
        if (string.IsNullOrEmpty(apiKey)) throw new ArgumentException("API key is required", nameof(apiKey));
        if (string.IsNullOrEmpty(apiSecret)) throw new ArgumentException("API secret is required", nameof(apiSecret));

        this.apiKey = apiKey;
        this.apiSecret = apiSecret;
        this.reqExpire = reqExpire ?? TimeSpan.FromMinutes(10);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Sign the request before sending it
        await SignRequestAsync(request);

        // Proceed with the HTTP request
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task SignRequestAsync(HttpRequestMessage request)
    {
        var expiration = new DateTime(2032, 1, 1).Add(reqExpire);

        var sigParts = new List<string>
        {
            $"{request.Method.Method} {request.RequestUri.AbsolutePath}",
            await GetRequestBodyAsync(request)
        };

        var headerParts = new List<string>
        {
            $"EXO2-HMAC-SHA256 credential={apiKey}"
        };

        // Extract query parameters and sign them
        var (signedParams, paramsValues) = ExtractRequestParameters(request);
        sigParts.Add(paramsValues);

        if (signedParams.Any())
        {
            headerParts.Add($"signed-query-args={string.Join(";", signedParams)}");
        }

        // Add headers and expiration
        sigParts.Add("");
        sigParts.Add(expiration.ToUnixTimestamp().ToString());
        headerParts.Add($"expires={expiration.ToUnixTimestamp()}");

        // Calculate signature using HMAC-SHA256
        var signature = ComputeHmacSignature(string.Join("\n", sigParts));

        headerParts.Add($"signature={signature}");


        var fullHeader = string.Join(",", headerParts);

        // Set Authorization header
        request.Headers.TryAddWithoutValidation("Authorization", fullHeader);
    }

    private async Task<string> GetRequestBodyAsync(HttpRequestMessage request)
    {
        if (request.Content == null)
            return string.Empty;

        var body = await request.Content.ReadAsStringAsync();
        return body;
    }

    private (List<string> signedParams, string paramsValues) ExtractRequestParameters(HttpRequestMessage request)
    {
        var names = new List<string>();
        var values = new StringBuilder();

        // Ensure query parameters are sorted
        var queryParams = request.RequestUri.Query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(param => param.Split('='))
            .Where(parts => parts.Length == 2)
            .OrderBy(parts => parts[0])
            .ToList();

        foreach (var param in queryParams)
        {
            names.Add(param[0]);
            values.Append(param[1]);
        }

        return (names, values.ToString());
    }

    private string ComputeHmacSignature(string data)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }
}

public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime date)
    {
        return new DateTimeOffset(date).ToUnixTimeSeconds();
    }
}
