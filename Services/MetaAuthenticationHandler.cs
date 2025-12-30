namespace N10.Services;

public class MetaAuthenticationHandler(IOptions<MetaOptions> options) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.BoltaToken);

        return await base.SendAsync(request, cancellationToken);
    }
}