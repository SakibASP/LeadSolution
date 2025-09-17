using Common.Utils.Extentions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Common;

public class Cities
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int StateId { get; set; }
    public int CountryId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now.ToBangladeshTime();
    public DateTime? UpdatedDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties (optional, if using EF)
    [ForeignKey(nameof(StateId))]
    public States? State { get; set; }
    [ForeignKey(nameof(CountryId))]
    public Countries? Country { get; set; }
}
