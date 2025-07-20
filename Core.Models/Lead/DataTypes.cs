using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Lead;

[Table(nameof(DataTypes))]
public class DataTypes
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsBootstrap { get; set; } = false;
}
