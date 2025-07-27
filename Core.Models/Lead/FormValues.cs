using Core.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Lead;

public class FormValues
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public int FormId { get; set; }

    [Required]
    public int BusinessId { get; set; }

    [Required]
    public string FormValue { get; set; }

    [Required]
    public long SubmissionId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }

    [ForeignKey(nameof(FormId))]
    public FormDetails? FormDetails { get; set; }

    [ForeignKey(nameof(BusinessId))]
    public AspNetBusinessInfo? AspNetBusinessInfo { get; set; }
}
