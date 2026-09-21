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

        var catalogProject = LocateTheCatalogProject();

        // Started with "dotnet run", the same way the Product Owner's start file does it,
        // rather than by running the built file directly. That is not fussiness: running
        // the built file skips the step that serves MudBlazor's own JavaScript, so the
        // catalog looks right, accepts typing, and then dies the moment a button is
        // pressed. Testing it the way it is actually run is the only way to catch that.
        var startInfo = new ProcessStartInfo(
            "dotnet",
            $"run --project \"{catalogProject}\" --configuration Debug --no-build")
        {
            WorkingDirectory = Path.GetDirectoryName(catalogProject)!,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        startInfo.Environment["DATACATALOG_URL"] = Address;
        startInfo.Environment["DATACATALOG_FILE"] = _catalogFilePath;
        startInfo.Environment["DATACATALOG_NO_BROWSER"] = "1";

        _catalogProcess = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The catalog process did not start.");

        // Keep everything the catalog says. When a browser test fails because something
        // went wrong on the server, this is the only place the real reason appears.
        _catalogProcess.OutputDataReceived += RecordLine;
        _catalogProcess.ErrorDataReceived += RecordLine;
        _catalogProcess.BeginOutputReadLine();
        _catalogProcess.BeginErrorReadLine();

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
                throw new InvalidOperationException(
                    $"The catalog stopped before it was ready.{Environment.NewLine}{WhatTheCatalogSaid()}");
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

    private readonly System.Text.StringBuilder _whatTheCatalogSaid = new();

    private void RecordLine(object sender, DataReceivedEventArgs line)
    {
        if (line.Data is null)
        {
            return;
        }

        lock (_whatTheCatalogSaid)
        {
            _whatTheCatalogSaid.AppendLine(line.Data);
        }
    }

    /// <summary>
    /// Everything the catalog has written. Use it in a failing test to find out what went
    /// wrong on the server, rather than guessing from what the browser was left showing.
    /// </summary>
    public string WhatTheCatalogSaid()
    {
        lock (_whatTheCatalogSaid)
        {
            return _whatTheCatalogSaid.ToString();
        }
    }

    private static int FindAFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string LocateTheCatalogProject()
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

        var catalogProject = Path.Combine(
            directory.FullName, "src", "DataCatalog.Web", "DataCatalog.Web.csproj");

        if (!File.Exists(catalogProject))
        {
            throw new InvalidOperationException($"Expected the catalog project at {catalogProject}.");
        }

        return catalogProject;
    }
}
