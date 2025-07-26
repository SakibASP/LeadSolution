using Common.Extentions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Common;

public class BaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.Now.ToBangladeshTime();
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
