using Core.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Lead;

[Table(nameof(DataTypes))]
public class DataTypes : BaseModel
{
    public string? Name { get; set; }
    public bool IsBootstrap { get; set; } = false;
}
