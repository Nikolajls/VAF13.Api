using HtmlAgilityPack;

namespace VAF13.Klubadmin.Domain.Services.Klubadmin;

public static class HtmlDocumentExtensions
{
    public static string GetElementValueById(this HtmlDocument doc, string elementId, bool returnEmptyValueIfNotFound = true)
    {
        var element = doc.GetElementbyId(elementId);
        if (element is null)
        {
            if (returnEmptyValueIfNotFound)
            {
                return string.Empty;
            }

            throw new Exception("");
        }

        var value = element.Attributes.FirstOrDefault(c => c.Name == "value")?.DeEntitizeValue ?? string.Empty;

        return value;
    }

    public static string GetDropdownValue(this HtmlDocument doc, string elementId, bool returnEmptyValueIfNotFound = true)
    {
        var element = doc.GetElementbyId(elementId);
        if (element is null)
        {
            if (returnEmptyValueIfNotFound)
            {
                return string.Empty;
            }

            throw new Exception("");
        }

        var value = element.Attributes.FirstOrDefault(c => c.Name == "data-orgvalue")?.DeEntitizeValue ?? string.Empty;

        return value;
    }
}