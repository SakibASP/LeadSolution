namespace Core.ViewModels.Dto.Lead;

public class DynamicFormDto
{
    public int? FormDetailId { get; set; }
    public string? Label { get; set; }
    public string? InputType { get; set; }  // text, email, date, select, etc.
    public string? Value { get; set; }
    public bool IsSelectInput { get; set; } = false;
    public bool IsActive { get; set; }
}
