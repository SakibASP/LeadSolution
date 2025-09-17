using Common.Utils.Extentions;

namespace Core.ViewModels.Request.Lead;

public class GetFormValueRequest
{
    public int BusinessId { get; set; }
    public DateTime FromDate { get; set; } = DateTime.Now.ToBangladeshTime().AddDays(-7);
    public DateTime ToDate { get; set; } = DateTime.Now.ToBangladeshTime();
}
