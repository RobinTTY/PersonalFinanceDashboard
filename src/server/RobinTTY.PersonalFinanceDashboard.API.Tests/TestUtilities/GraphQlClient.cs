using System.Net.Http.Json;
using System.Text.Json;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;

internal static class GraphQlClient
{
    /// <summary>
    /// Sends a GraphQL request and returns the response body.
    /// </summary>
    public static async Task<JsonElement> PostGraphQl(this HttpClient client, string query, object? variables = null)
    {
        var response = await client.PostAsJsonAsync("/graphql", new { query, variables });
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
    }

    /// <summary>
    /// Follows the given property names starting at "data".
    /// </summary>
    public static JsonElement Data(this JsonElement response, params string[] path) =>
        path.Aggregate(response.GetProperty("data"), (element, name) => element.GetProperty(name));

    public static string? ErrorCode(this JsonElement response) =>
        response.GetProperty("errors")[0].GetProperty("extensions").GetProperty("code").GetString();

    public static string? ErrorMessage(this JsonElement response) =>
        response.GetProperty("errors")[0].GetProperty("message").GetString();
}