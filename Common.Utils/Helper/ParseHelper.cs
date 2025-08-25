using System.Xml.Linq;

namespace Common.Utils.Helper;

public static class ParseHelper
{
    public static Dictionary<string, string> ParseXmlToDictionary(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml)) return new();

        try
        {
            var doc = XDocument.Parse(xml);
            return doc.Descendants("property")
                      .ToDictionary(
                          p => p.Attribute("key")?.Value ?? "Unknown",
                          p => p.Value
                      );
        }
        catch
        {
            return new() { { "Raw", xml } }; // fallback if invalid XML
        }
    }

}
