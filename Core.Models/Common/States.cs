using Common.Extentions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Common;

public class States
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int CountryId { get; set; }
    public string? Iso2 { get; set; }
    public string? Iso3166_2 { get; set; }
    public string? FipsCode { get; set; }
    public string? Type { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Timezone { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now.ToBangladeshTime();
    public DateTime? UpdatedDate { get; set; }
    public bool IsActive { get; set; } = true;


    [ForeignKey(nameof(CountryId))]
    public Countries? Country { get; set; }
}
