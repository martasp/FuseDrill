public sealed class MyHttpMessageHandler : HttpClientHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {

        if (request.RequestUri != null && request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase))
        {
            request.RequestUri = new Uri($"https://api.groq.com/openai{request.RequestUri.PathAndQuery}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}

