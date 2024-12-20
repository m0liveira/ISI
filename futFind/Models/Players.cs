using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace futFind.Models
{
    [Table("players")]
    public class Players
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Users")]
        public int user_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Games")]
        public int match_id { get; set; }
    }
}
