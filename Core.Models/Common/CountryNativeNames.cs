using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace Core.Models.Common;

public class CountryNativeNames
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public string? LanguageCode { get; set; }
    public string? Official { get; set; }
    public string? Common { get; set; }

    [ForeignKey(nameof(CountryId))]
    public Countries? Country { get; set; }
}
