using Common.Utils.Extentions;

namespace Core.Models.Lead;

public class BusinessSupportedFormId
{
    public int Id { get; set; }

    public int BusinessId { get; set; }

    public int FormId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsNullSupported { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now.ToBangladeshTime();

    public string CreatedBy { get; set; } = null!;  // Not null in DB

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }
}
