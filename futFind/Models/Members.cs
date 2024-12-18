using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace futFind.Models
{
    [Table("clan_members")]
    public class Members
    {
        [Key]
        [Column(Order = 0)]
        public int user_id { get; set; }

        [Key]
        [Column(Order = 1)]
        public int clan_id { get; set; }

        [ForeignKey("user_id")]
        public Users User { get; set; } = null!;

        [ForeignKey("clan_id")]
        public Teams Team { get; set; } = null!;
    }
}