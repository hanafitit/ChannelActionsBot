using System.Net.Http;

var url = Environment.GetEnvironmentVariable("PING_URL")
          ?? "https://channelactionsbot-e6rg.onrender.com/";

var intervalSeconds = int.TryParse(
    Environment.GetEnvironmentVariable("PING_INTERVAL_SECONDS"),
    out var interval
) && interval > 0 ? interval : 30;

using var client = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(10)
};

Console.WriteLine("[Pinger] Started");
Console.WriteLine($"[Pinger] Target: {url}");
Console.WriteLine($"[Pinger] Interval: {intervalSeconds}s");

// First ping immediately
await PingAsync();

// Then ping periodically
using var timer = new PeriodicTimer(
    TimeSpan.FromSeconds(intervalSeconds)
);

while (await timer.WaitForNextTickAsync())
{
    await PingAsync();
}

async Task PingAsync()
{
    using var cts = new CancellationTokenSource(
        TimeSpan.FromSeconds(10)
    );

    Console.WriteLine("[Pinger] HTTP request started");

    try
    {
        using var response = await client.GetAsync(
            url,
            HttpCompletionOption.ResponseHeadersRead,
            cts.Token
        );

        Console.WriteLine(
            $"[Pinger] HTTP {(int)response.StatusCode} {response.StatusCode}"
        );
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("[Pinger] Request timed out after 10 seconds");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Pinger] HTTP error: {ex}");
    }
    finally
    {
        Console.WriteLine("[Pinger] Request finished");
    }
}
