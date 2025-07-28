using System.ComponentModel.DataAnnotations;

namespace Core.Models.Auth;

public class AspNetServiceTypes
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required, StringLength(256)]
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;
}
