using System.Net.Http.Headers;

namespace N10.Services;

public class TmdbAuthenticationHandler(IOptions<TmdbOptions> options) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("accept", "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.AccessTokenAuth);

        return await base.SendAsync(request, cancellationToken);
    }
}