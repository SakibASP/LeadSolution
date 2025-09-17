using Common.Utils.Extentions;

namespace Core.Models.Common;

public class EmailSendHistory
{
    public int Id { get; set; }
    public string From { get; set; } = default!;
    public string To { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string Body { get; set; } = default!;
    public DateTime CreatedDate { get; set; } = DateTime.Now.ToBangladeshTime();
}
