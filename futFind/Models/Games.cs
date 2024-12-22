using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace futFind.Models
{
    [Table("games")]
    public class Games
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int host_id { get; set; }

        [Required]
        public DateTime date { get; set; }

        [MaxLength(200)]
        public string? address { get; set; }

        public int? capacity { get; set; }

        [Column(TypeName = "NUMERIC(10,2)")]
        public decimal? price { get; set; }

        public bool is_private { get; set; } = false;

        [MaxLength(15)]
        public string? share_code { get; set; }

        [Required]
        [MaxLength(50)]
        public string status { get; set; } = "Scheduled";
    }
}
