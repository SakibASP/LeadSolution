using Core.Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Lead;

public class FormDetails : BaseModel
{
    public string? Name { get; set; }

    [DisplayName("Type")]
    public int? DataTypeId { get; set; }

    [DisplayName("Select Input")]
    public bool IsSelectInput { get; set; } = false;

    [DisplayName("Active")]
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(DataTypeId))]
    public DataTypes? DataTypes { get; set; }
}
