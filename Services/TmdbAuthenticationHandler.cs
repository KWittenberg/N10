using System.Net.Http.Headers;

namespace N10.Services;

public class TmdbAuthenticationHandler(IOptions<TmdbOptions> options) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.AccessTokenAuth);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        // request.Headers.Add("accept", "application/json");

        return await base.SendAsync(request, cancellationToken);
    }
}