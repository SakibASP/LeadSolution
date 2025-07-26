using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Lead;

[Table(nameof(FormValues))]
public class FormValues
{
    public int Id { get; set; }

    public int? FormId { get; set; }

    public string? FormValue { get; set; }
    public long? SubmissionId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    [ForeignKey(nameof(FormId))]
    public FormDetails? FormDetails { get; set; }
}
