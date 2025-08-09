using System.ComponentModel;

namespace Core.ViewModels.Dto.Lead;

public class DynamicFormViewModel
{
    public List<DynamicFormDto>? Inputs { get; set; }

    [DisplayName("Business Name")]
    public int BusinessId { get; set; }
}
