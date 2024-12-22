using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace futFind.Models
{
    [Table("players")]
    [PrimaryKey(nameof(user_id), nameof(match_id))]
    public class Players
    {
        [ForeignKey("Users")]
        public int user_id { get; set; }

        [ForeignKey("Games")]
        public int match_id { get; set; }

        public Users User { get; set; } = null!;
        public Games Game { get; set; } = null!;
    }
}
