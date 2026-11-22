using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RobinTTY.PersonalFinanceDashboard.Api.Extensions;
using Serilog;
using Serilog.Core;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class LoggingTests : IDisposable
{
    private readonly string _logDirectory = Path.Combine(Path.GetTempPath(), $"pfd-logging-tests-{Guid.NewGuid()}");

    [Fact]
    public async Task UnreachableSeq_LogsAreWrittenToTheFile()
    {
        await using var logger = CreateLogger("http://localhost:1");

        logger.Information("Synchronized {Count} accounts", 3);

        var file = Assert.Single(Directory.GetFiles(_logDirectory));
        // The logger still has the file open
        await using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        Assert.Contains("\"Count\":3", await new StreamReader(stream).ReadToEndAsync());
    }

    [Fact]
    public async Task ReachableSeq_ReceivesTheLogs()
    {
        var requestMethods = new ConcurrentQueue<string>();
        await using var seq = WebApplication.Create();
        seq.Urls.Add("http://127.0.0.1:0");
        seq.Run(context =>
        {
            requestMethods.Enqueue(context.Request.Method);
            context.Response.StatusCode = StatusCodes.Status201Created;
            return Task.CompletedTask;
        });
        await seq.StartAsync();

        await using (var logger = CreateLogger(seq.Urls.Single()))
        {
            logger.Information("Synchronized {Count} accounts", 3);
        }

        Assert.Contains(HttpMethods.Post, requestMethods);
        Assert.False(Directory.Exists(_logDirectory));
    }

    private Logger CreateLogger(string seqServerUrl) =>
        new LoggerConfiguration()
            .WriteTo.SeqWithFileFallback(seqServerUrl, Path.Combine(_logDirectory, "log-.clef"))
            .CreateLogger();

    public void Dispose()
    {
        if (Directory.Exists(_logDirectory))
        {
            Directory.Delete(_logDirectory, recursive: true);
        }
    }
}