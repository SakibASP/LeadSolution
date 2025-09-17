using Common.Utils.Extentions;

namespace Core.Models.Lead;

public class FormValueMaster
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public long? SubmissionId { get; set; }
    public int? TotalItems { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now.ToBangladeshTime();
}
