using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class DataSynchronizationSubscriptionTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DataSynchronized_IsSentOverWebSocketsWhenASynchronizationRetrievedData()
    {
        // No GoCardless response is configured, so the retrieval fails, which also changes the sync status
        await factory.Seed(TestData.Request(Guid.NewGuid(), DateTime.UtcNow.AddDays(-1),
            TestData.Account(Guid.NewGuid())));
        using var socket = await ConnectGraphQlWs();
        await Send(socket, new
        {
            id = "1", type = "subscribe",
            payload = new { query = "subscription { dataSynchronized { completedAt } }" }
        });

        // The subscription is set up asynchronously, so a synchronization may finish before it
        var next = Receive(socket);
        var deadline = DateTime.UtcNow + Timeout;
        while (!next.IsCompleted && DateTime.UtcNow < deadline)
        {
            await _client.PostGraphQl("mutation { synchronizeTransactionData { boolean } }");
            try
            {
                await next.WaitAsync(TimeSpan.FromMilliseconds(100));
            }
            catch (TimeoutException)
            {
                // Not received yet, so the next synchronization is triggered
            }
        }

        var message = await next.WaitAsync(Timeout);
        Assert.Equal("next", message.GetProperty("type").GetString());
        Assert.Equal(JsonValueKind.String, message.GetProperty("payload").GetProperty("data")
            .GetProperty("dataSynchronized").GetProperty("completedAt").ValueKind);
    }

    private async Task<WebSocket> ConnectGraphQlWs()
    {
        var client = factory.Server.CreateWebSocketClient();
        client.SubProtocols.Add("graphql-transport-ws");
        var socket = await client.ConnectAsync(new Uri("ws://localhost/graphql"), CancellationToken.None);

        await Send(socket, new { type = "connection_init" });
        var ack = await Receive(socket).WaitAsync(Timeout);
        Assert.Equal("connection_ack", ack.GetProperty("type").GetString());

        return socket;
    }

    private static Task Send(WebSocket socket, object message) =>
        socket.SendAsync(JsonSerializer.SerializeToUtf8Bytes(message), WebSocketMessageType.Text, true,
            CancellationToken.None);

    private static async Task<JsonElement> Receive(WebSocket socket)
    {
        var buffer = new byte[4096];
        var message = new StringBuilder();
        WebSocketReceiveResult result;
        do
        {
            result = await socket.ReceiveAsync(buffer, CancellationToken.None);
            message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
        } while (!result.EndOfMessage);

        return JsonElement.Parse(message.ToString());
    }
}