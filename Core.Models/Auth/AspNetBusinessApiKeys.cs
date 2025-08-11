using Core.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Auth;

public class AspNetBusinessApiKeys : BaseModel
{
    public int BusinessId { get; set; }

    [StringLength(128)]
    public string ApiKey { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(BusinessId))]
    public AspNetBusinessInfo? AspNetBusinessInfo { get; set; }
}
