using DataCatalog.Core.Text;

namespace DataCatalog.Core.UnitTests;

/// <summary>
/// The wording the Product Owner approved word for word at request 001, question 13.
/// These tests exist so nobody can quietly reword what a worried person reads at the
/// moment their catalog will not open.
/// </summary>
public class StartupMessagesTests
{
    [Fact]
    public void CatalogFileDamaged_SaysTheFileWasNotChanged()
    {
        var message = StartupMessages.CatalogFileDamaged(@"C:\catalog\datacatalog.db", "file is not a database");

        Assert.Contains("Data Catalog could not start.", message);
        Assert.Contains("The catalog file appears to be damaged. It has NOT been changed", message);
        Assert.Contains("or replaced.", message);
    }

    [Fact]
    public void CatalogFileDamaged_TellsThePersonNotToDeleteTheFile()
    {
        var message = StartupMessages.CatalogFileDamaged(@"C:\catalog\datacatalog.db", "file is not a database");

        Assert.Contains("Do not delete this file", message);
        Assert.Contains("recover your", message);
    }

    [Fact]
    public void CatalogFileDamaged_IncludesTheFullPathSoThePersonKnowsWhichFile()
    {
        var message = StartupMessages.CatalogFileDamaged(@"C:\catalog\datacatalog.db", "file is not a database");

        Assert.Contains(@"File location: C:\catalog\datacatalog.db", message);
    }

    [Fact]
    public void CatalogFileDamaged_KeepsTheTechnicalDetailForWhoeverHelps()
    {
        var message = StartupMessages.CatalogFileDamaged(@"C:\catalog\datacatalog.db", "file is not a database");

        Assert.Contains("Technical detail for whoever helps you:", message);
        Assert.Contains("file is not a database", message);
    }

    [Fact]
    public void AddressAlreadyInUse_ExplainsWhatToDoWithoutJargon()
    {
        var message = StartupMessages.AddressAlreadyInUse("Failed to bind to address http://localhost:5199");

        Assert.Contains("Data Catalog could not start.", message);
        Assert.Contains("Another program on this computer is already using the address", message);
        Assert.Contains("Close that program, then close this window", message);
        Assert.Contains("Failed to bind to address http://localhost:5199", message);
    }

    [Fact]
    public void EveryStartupMessage_StartsByNamingTheCatalog()
    {
        // Whatever went wrong, the first line tells the person which program is talking
        // to them - they may have several windows open.
        Assert.StartsWith("Data Catalog could not start.", StartupMessages.AddressAlreadyInUse("detail"));
        Assert.StartsWith("Data Catalog could not start.", StartupMessages.CatalogFileDamaged("path", "detail"));
        Assert.StartsWith("Data Catalog could not start.", StartupMessages.CouldNotStart("detail"));
    }

    [Fact]
    public void PageText_IsTheWordingTheProductOwnerChose()
    {
        Assert.Equal("Data Catalog", PageText.CatalogName);
        Assert.Equal("The catalog is running. No data sources have been added yet.", PageText.RunningStatus);
    }
}
