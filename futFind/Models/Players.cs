using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace futFind.Models
{
    [Table("players")]
    [PrimaryKey(nameof(user_id), nameof(match_id))]
    public class Players
    {
        public int user_id { get; set; }
        public int match_id { get; set; }

        [ForeignKey("user_id")]
        public Users User { get; set; } = null!;

        [ForeignKey("match_id")]
        public Games Game { get; set; } = null!;
    }
}
