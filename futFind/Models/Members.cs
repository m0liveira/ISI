using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace futFind.Models
{
    [Table("clan_members")]
    [PrimaryKey(nameof(user_id), nameof(clan_id))]
    public class Members
    {
        public int user_id { get; set; }
        public int clan_id { get; set; }

        [ForeignKey("user_id")]
        public Users User { get; set; } = null!;

        [ForeignKey("clan_id")]
        public Teams Team { get; set; } = null!;
    }
}