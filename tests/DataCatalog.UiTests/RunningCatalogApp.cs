using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace DataCatalog.UiTests;

/// <summary>
/// Starts the real Data Catalog on a port of its own, with its own throwaway catalog
/// file, and stops it when the tests finish. Nothing here touches the catalog file the
/// Product Owner uses.
/// </summary>
public sealed class RunningCatalogApp : IAsyncLifetime
{
    private Process? _catalogProcess;
    private string _catalogFilePath = string.Empty;

    public string Address { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        Address = $"http://localhost:{FindAFreePort()}";
        _catalogFilePath = Path.Combine(
            Path.GetTempPath(),
            $"datacatalog-uitest-{Guid.NewGuid():N}.db");

        var catalogDll = LocateBuiltCatalog();

        var startInfo = new ProcessStartInfo("dotnet", $"exec \"{catalogDll}\"")
        {
            WorkingDirectory = Path.GetDirectoryName(catalogDll)!,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        startInfo.Environment["DATACATALOG_URL"] = Address;
        startInfo.Environment["DATACATALOG_FILE"] = _catalogFilePath;
        startInfo.Environment["DATACATALOG_NO_BROWSER"] = "1";

        _catalogProcess = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The catalog process did not start.");

        await WaitUntilTheCatalogAnswersAsync();
    }

    public async Task DisposeAsync()
    {
        if (_catalogProcess is { HasExited: false })
        {
            _catalogProcess.Kill(entireProcessTree: true);
            await _catalogProcess.WaitForExitAsync();
        }

        _catalogProcess?.Dispose();

        foreach (var path in new[] { _catalogFilePath, _catalogFilePath + "-shm", _catalogFilePath + "-wal" })
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private async Task WaitUntilTheCatalogAnswersAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var giveUpAt = DateTime.UtcNow.AddSeconds(90);

        while (DateTime.UtcNow < giveUpAt)
        {
            if (_catalogProcess is { HasExited: true })
            {
                var output = await _catalogProcess.StandardOutput.ReadToEndAsync();
                var errors = await _catalogProcess.StandardError.ReadToEndAsync();
                throw new InvalidOperationException(
                    $"The catalog stopped before it was ready.{Environment.NewLine}{output}{Environment.NewLine}{errors}");
            }

            try
            {
                var response = await client.GetAsync(Address);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Not listening yet. Keep waiting.
            }

            await Task.Delay(250);
        }

        throw new TimeoutException($"The catalog did not answer at {Address} within 90 seconds.");
    }

    private static int FindAFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string LocateBuiltCatalog()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "DataCatalog.slnx")))
        {
            directory = directory.Parent;
        }

        if (directory is null)
        {
            throw new InvalidOperationException("Could not find the project folder from the test assembly.");
        }

        var catalogDll = Path.Combine(
            directory.FullName, "src", "DataCatalog.Web", "bin", "Debug", "net10.0", "DataCatalog.Web.dll");

        if (!File.Exists(catalogDll))
        {
            throw new InvalidOperationException(
                $"The catalog has not been built. Expected it at {catalogDll}. Run dotnet build first.");
        }

        return catalogDll;
    }
}
