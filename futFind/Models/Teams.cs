using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace futFind.Models
{
    [Table("teams")]
    public class Teams
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int leader { get; set; }

        [Required]
        [MaxLength(30)]
        public string name { get; set; } = null!;

        [MaxLength(50)]
        public string? description { get; set; }

        public int? capacity { get; set; }

        [MaxLength(20)]
        public string? invite_code { get; set; }
    }
}
