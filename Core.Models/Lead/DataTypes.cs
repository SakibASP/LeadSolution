using Core.Models.Common;

namespace Core.Models.Lead;

public class DataTypes : BaseModel
{
    public string? Name { get; set; }
    public bool IsActive { get; set; } = false;
}
