using DataCatalog.Core.Models;

namespace DataCatalog.Core.Text;

/// <summary>
/// Wording the Product Owner chose, kept in one place so the pages, the tests and any
/// future screen cannot drift apart (P21). None of this is invented by an agent.
///
/// Items marked DRAFTED were written by an agent and approved as part of a plan rather
/// than chosen word for word by the Product Owner. They are flagged so that anyone
/// reviewing the catalog's voice knows which lines to look at first.
/// </summary>
public static class PageText
{
    // ---- Request 001 -------------------------------------------------------

    /// <summary>What the catalog is called, everywhere (001 Q2).</summary>
    public const string CatalogName = "Data Catalog";

    /// <summary>Shown when the catalog holds nothing (001 Q4).</summary>
    public const string RunningStatus = "The catalog is running. No data sources have been added yet.";

    // ---- Request 002, question 12 ------------------------------------------

    public const string AddDataSource = "Add a data source";
    public const string Save = "Save";
    public const string Cancel = "Cancel";
    public const string SaveChanges = "Save changes";
    public const string Edit = "Edit";
    public const string Remove = "Remove";
    public const string KeepIt = "Keep it";
    public const string PutBack = "Put back";
    public const string ViewIt = "View it";
    public const string SaveAnyway = "Save anyway";
    public const string BackToAllDataSources = "Back to all data sources";
    public const string Saved = "Saved";
    public const string History = "History";
    public const string RemovedDataSources = "Removed data sources";

    /// <summary>Shown in place of a field nobody has filled in yet. A gap should look like a gap.</summary>
    public const string NotRecordedYet = "Not recorded yet";

    /// <summary>
    /// A field's value as it should appear on screen. Blank becomes "Not recorded yet"
    /// in one place, so no page can quietly render an empty gap instead.
    /// </summary>
    public static string OrNotRecorded(string? value) =>
        string.IsNullOrWhiteSpace(value) ? NotRecordedYet : value;

    /// <summary>Under the name box when it is blank or only spaces (002 Q6).</summary>
    public const string NameRequired = "Please give this data source a name";

    /// <summary>The heading of the edit page, followed by the data source's name.</summary>
    public const string EditHeadingPrefix = "Edit";

    public static string DuplicateName(string name) =>
        $"A data source called \"{name}\" already exists.";

    public static string ConfirmRemove(string name) =>
        $"Remove \"{name}\"? It will be hidden from the list. You can put it back at any time from Removed data sources.";

    // ---- Request 002, drafted in the plan rather than chosen at question 12 --

    /// <summary>DRAFTED. When saving fails for any reason (002 Q19).</summary>
    public const string CouldNotSave =
        "This data source could not be saved. Nothing you typed has been lost - please try again.";

    /// <summary>DRAFTED. A link to something that never existed (002 Q16).</summary>
    public const string CouldNotBeFound = "That data source could not be found";

    /// <summary>DRAFTED. At the top of a removed data source's page (002 Q16).</summary>
    public const string RemovedNotice =
        "This data source has been removed. Put it back to edit it.";

    /// <summary>DRAFTED. The removed page when nothing has been removed.</summary>
    public const string NothingHasBeenRemoved = "Nothing has been removed.";

    /// <summary>DRAFTED. When leaving a form with something typed in it (002 Q15).</summary>
    public const string DiscardHeading = "Discard what you have typed?";

    /// <summary>DRAFTED.</summary>
    public const string DiscardQuestion =
        "You have typed something that has not been saved. Leaving now will throw it away.";

    /// <summary>DRAFTED.</summary>
    public const string KeepEditing = "Keep editing";

    /// <summary>DRAFTED.</summary>
    public const string Discard = "Discard";

    // ---- Field labels ------------------------------------------------------

    /// <summary>
    /// What each field is called on screen. The history renders labels through here, so
    /// renaming a field renames its whole history with it rather than leaving a mixture.
    /// </summary>
    public static string LabelFor(CatalogField field) => field switch
    {
        CatalogField.Name => "Name",
        CatalogField.Description => "Description",
        CatalogField.SourceSystem => "System",
        CatalogField.Owner => "Owner",
        CatalogField.Contact => "Contact",
        CatalogField.Sensitivity => "Sensitivity",
        CatalogField.UpdateFrequency => "Update frequency",
        _ => throw new ArgumentOutOfRangeException(nameof(field), field, "No label exists for this field.")
    };

    /// <summary>
    /// One line of history, in plain English. DRAFTED shape, following the example the
    /// Product Owner approved at question 12.
    /// </summary>
    public static string DescribeChange(CatalogChange change) => change.Kind switch
    {
        CatalogChangeKind.Added => "Added to the catalog",
        CatalogChangeKind.Removed => "Removed from the catalog",
        CatalogChangeKind.PutBack => "Put back on the catalog",
        CatalogChangeKind.FieldChanged => DescribeFieldChange(change),
        _ => throw new ArgumentOutOfRangeException(nameof(change), change.Kind, "No wording exists for this kind of change.")
    };

    private static string DescribeFieldChange(CatalogChange change)
    {
        var label = change.Field is null ? "A field" : LabelFor(change.Field.Value);
        var from = Quoted(change.OldValue);
        var to = Quoted(change.NewValue);

        return $"{label} changed from {from} to {to}";
    }

    private static string Quoted(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "nothing" : $"\"{value}\"";
}
