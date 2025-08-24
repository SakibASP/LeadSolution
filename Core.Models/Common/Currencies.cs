using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Common;

public class Currencies
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public string? CurrencyCode { get; set; }
    public string? CurrencyName { get; set; }
    public string? CurrencySymbol { get; set; }
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(CountryId))]
    public Countries? Country { get; set; }
}
