using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Menu
{
    [Table(nameof(MenuItem))]
    public class MenuItem
    {
        [Key]
        public int MenuId { get; set; }
        public string? MenuName { get; set; }
        public string? MenuUrl { get; set; }
        public int? MenuParentId { get; set; }
        public bool? Active { get; set; }
        public string? FaIcon { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
        [NotMapped]
        public virtual List<MenuItem>? Children { get; set; }
        [ForeignKey(nameof(MenuParentId))]
        public virtual MenuItem? Parent { get; set; }
    }
}
