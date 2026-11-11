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
    Console.WriteLine($"[Pinger] Sending request: {DateTimeOffset.UtcNow:O}");

    try
    {
        using var response = await client.GetAsync(
            url,
            HttpCompletionOption.ResponseHeadersRead
        );

        Console.WriteLine(
            $"[Pinger] Response: {(int)response.StatusCode} {response.StatusCode}"
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Pinger] Error: {ex}");
    }
}
