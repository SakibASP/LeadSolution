using Common.Extentions;

namespace Core.ViewModels.Request.Lead;

public class UpdateFormSettingsRequest
{
    public int BusinessId { get; set; }
    public string? Username { get; set; }
    public IList<FormSelectDetails>? FormSelectDetails { get; set; }
}


public class FormSelectDetails
{
    public int FormDetailId { get; set; }
    public bool IsChecked { get; set; }
}