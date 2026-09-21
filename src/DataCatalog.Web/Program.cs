using System.Diagnostics;
using DataCatalog.Core;
using DataCatalog.Core.Abstractions;
using DataCatalog.Core.Text;
using DataCatalog.Infrastructure;
using DataCatalog.Web.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

// The address the catalog listens on. Overridable so the browser tests can use a port
// of their own without colliding with a catalog the Product Owner already has running.
var url = Environment.GetEnvironmentVariable("DATACATALOG_URL") ?? "http://localhost:5199";

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls(url);

    var catalogFilePath = ResolveCatalogFilePath(builder.Environment.ContentRootPath);

    builder.Services.AddDbContext<CatalogDbContext>(options =>
        options.UseSqlite($"Data Source={catalogFilePath}"));
    builder.Services.AddScoped<ICatalogRepository, SqliteCatalogRepository>();

    builder.Services.AddRazorComponents().AddInteractiveServerComponents();
    builder.Services.AddMudServices();

    var app = builder.Build();

    // Before anything is served: make sure the catalog file exists and can be read.
    // A missing file is created; a damaged one stops us here and is left untouched.
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        CatalogDatabase.Prepare(context, catalogFilePath);
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
    }

    app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
    app.UseAntiforgery();
    app.MapStaticAssets();
    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

    OpenBrowserWhenCatalogIsReady(app, url);

    app.Run();
    return 0;
}
catch (CatalogFileDamagedException damaged)
{
    Console.WriteLine(StartupMessages.CatalogFileDamaged(
        damaged.CatalogFilePath,
        damaged.InnerException?.Message ?? damaged.Message));
    return 1;
}
catch (Exception failure) when (IsAddressAlreadyInUse(failure))
{
    Console.WriteLine(StartupMessages.AddressAlreadyInUse(failure.Message));
    return 1;
}
catch (Exception failure)
{
    Console.WriteLine(StartupMessages.CouldNotStart(failure.Message));
    return 1;
}

static string ResolveCatalogFilePath(string contentRootPath)
{
    // The start file points this at the project folder, so the catalog file lands
    // where the Product Owner can see it and copy it - that copy is the backup.
    var configuredPath = Environment.GetEnvironmentVariable("DATACATALOG_FILE");

    return string.IsNullOrWhiteSpace(configuredPath)
        ? Path.Combine(contentRootPath, "datacatalog.db")
        : Path.GetFullPath(configuredPath);
}

static bool IsAddressAlreadyInUse(Exception exception)
{
    for (var current = exception; current is not null; current = current.InnerException)
    {
        if (current.GetType().Name.Contains("AddressInUse", StringComparison.OrdinalIgnoreCase) ||
            current.Message.Contains("address already in use", StringComparison.OrdinalIgnoreCase) ||
            current.Message.Contains("only one usage of each socket address", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
    }

    return false;
}

static void OpenBrowserWhenCatalogIsReady(WebApplication app, string address)
{
    if (Environment.GetEnvironmentVariable("DATACATALOG_NO_BROWSER") == "1")
    {
        return;
    }

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        Console.WriteLine($"{PageText.CatalogName} is running at {address}");
        Console.WriteLine("Close this window to stop it.");

        try
        {
            Process.Start(new ProcessStartInfo(address) { UseShellExecute = true });
        }
        catch (Exception couldNotOpenBrowser)
        {
            // The catalog is running, so this is not worth stopping for - but it is
            // never swallowed silently: the person is told what to do instead.
            Console.WriteLine($"Could not open your browser automatically: {couldNotOpenBrowser.Message}");
            Console.WriteLine($"Open this address yourself: {address}");
        }
    });
}
