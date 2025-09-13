using Common.Extentions;
using Core.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Lead;

public class FormValueDetails
{
    [Key]
    [Required]
    public int? Id { get; set; }

    [Required]
    public int? FormMasterId { get; set; }

    [Required]
    public int? FormId { get; set; }


    [Required]
    public string? FormValue { get; set; }

    [ForeignKey(nameof(FormId))]
    public FormDetails? FormDetails { get; set; }

    [ForeignKey(nameof(FormMasterId))]
    public FormValueMaster? FormValueMaster { get; set; }
}
