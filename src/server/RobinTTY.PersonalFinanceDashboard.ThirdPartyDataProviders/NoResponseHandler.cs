using System.Net;
using System.Text;
using System.Text.Json;
using Polly.Timeout;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// Answers requests that got no response, e.g. because the network is down or GoCardless timed out, with a 503 in
/// the format of GoCardless' errors. They are then reported like other failed requests instead of aborting the
/// synchronization.
/// </summary>
internal sealed class NoResponseHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TimeoutRejectedException &&
                                          !cancellationToken.IsCancellationRequested)
        {
            var error = JsonSerializer.Serialize(new
            {
                summary = "No response from GoCardless",
                detail = exception.Message,
                status_code = (int)HttpStatusCode.ServiceUnavailable
            });
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent(error, Encoding.UTF8, "application/json"),
                RequestMessage = request
            };
        }
    }
}
