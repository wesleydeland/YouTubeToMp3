var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Custom service for yt-dlp conversion and transient file streaming
builder.Services.AddSingleton<DownloadSessionStore>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Endpoint to serve generated audio files directly to the browser and clean up.
app.MapGet("/download/{token}", async (string token, HttpContext httpContext, DownloadSessionStore store) =>
{
    if (!store.TryTake(token, out var session) || session == null)
    {
        return Results.NotFound("Download not found or already retrieved.");
    }

    try
    {
        if (!File.Exists(session.FilePath))
        {
            return Results.NotFound("File missing on server.");
        }

        var fileName = Path.GetFileName(session.FilePath);
        var contentType = session.ContentType ?? "audio/mpeg";

        var stream = File.OpenRead(session.FilePath);

        // Schedule deletion after stream is disposed
        httpContext.Response.OnCompleted(() =>
        {
            try
            {
                stream.Dispose();
            }
            catch
            {
                // ignore
            }

            try
            {
                File.Delete(session.FilePath);
            }
            catch
            {
                // ignore cleanup failures
            }

            return Task.CompletedTask;
        });

        return Results.File(stream, contentType, fileName, enableRangeProcessing: true);
    }
    catch
    {
        try
        {
            if (File.Exists(session.FilePath))
                File.Delete(session.FilePath);
        }
        catch
        {
            // ignore
        }

        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
});

app.Run();

/// <summary>
/// Stores short-lived references to converted files so the browser can download them once.
/// </summary>
public class DownloadSessionStore
{
    private readonly object _lock = new();
    private readonly Dictionary<string, DownloadSession> _sessions = new();
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);

    public string Add(string filePath, string? contentType)
    {
        var token = Guid.NewGuid().ToString("N");
        var session = new DownloadSession
        {
            FilePath = filePath,
            ContentType = contentType,
            CreatedUtc = DateTime.UtcNow
        };

        lock (_lock)
        {
            _sessions[token] = session;
            CleanupExpired_NoLock();
        }

        return token;
    }

    public bool TryTake(string token, out DownloadSession? session)
    {
        lock (_lock)
        {
            if (_sessions.TryGetValue(token, out session))
            {
                _sessions.Remove(token);

                if (session.CreatedUtc + _ttl < DateTime.UtcNow)
                {
                    // Expired: best-effort delete and treat as miss
                    try
                    {
                        if (File.Exists(session.FilePath))
                            File.Delete(session.FilePath);
                    }
                    catch
                    {
                        // ignore
                    }

                    session = null;
                    return false;
                }

                return true;
            }
        }

        session = null;
        return false;
    }

    private void CleanupExpired_NoLock()
    {
        var now = DateTime.UtcNow;
        var expired = _sessions
            .Where(kvp => kvp.Value.CreatedUtc + _ttl < now)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expired)
        {
            if (_sessions.TryGetValue(key, out var s))
            {
                try
                {
                    if (File.Exists(s.FilePath))
                        File.Delete(s.FilePath);
                }
                catch
                {
                    // ignore
                }
            }
            _sessions.Remove(key);
        }
    }
}

public class DownloadSession
{
    public required string FilePath { get; set; }
    public string? ContentType { get; set; }
    public DateTime CreatedUtc { get; set; }
}
