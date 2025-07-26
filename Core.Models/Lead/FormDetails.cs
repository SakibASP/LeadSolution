using Core.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Lead;

[Table(nameof(FormDetails))]
public class FormDetails : BaseModel
{
    public string? Name { get; set; }
    public int? CSharpTypeId { get; set; }
    public int? BootstrapTypeId { get; set; }
    public bool IsNullSupported { get; set; } = true;
    public bool IsSelectInput { get; set; } = false;
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(CSharpTypeId))]
    public DataTypes? CSharpDataType { get; set; }

    [ForeignKey(nameof(BootstrapTypeId))]
    public DataTypes? BootstrapDataType { get; set; }
}
