namespace Core.Models.Common;

public class Audit
{
    public long Id { get; set; }
    public string? TableName { get; set; }
    public string? UserId { get; set; }
    public string? Actions { get; set; }
    public long? KeyValue { get; set; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
    public string? OperatingSystem { get; set; }
    public string? IPAddress { get; set; }
    public string? AreaAccessed { get; set; }
    public DateTime? UpdateDate { get; set; }
}
