using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Common;

public class Countries
{
    public int Id { get; set; }
    public string? CommonName { get; set; }
    public string? OfficialName { get; set; }
    public string? Iso2Code { get; set; }
    public string? Iso3Code { get; set; }
    public string? Region { get; set; }
    public string? SubRegion { get; set; }
    public string? FlagPng { get; set; }
    public string? FlagSvg { get; set; }
    public string? FlagAlt { get; set; }

}
