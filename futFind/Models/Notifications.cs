using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace futFind.Models
{
    [Table("notifications")]
    public class Notifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [ForeignKey("Games")]
        public int match_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string message { get; set; } = null!;

        public bool seen { get; set; } = false;

        [Required]
        public DateTime timestamp { get; set; } = DateTime.UtcNow;
    }
}
