using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace futFind.Models
{
    [Table("users")]
    public class Users
    {
        internal int Id;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [MaxLength(30)]
        public string name { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string email { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string password { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        public string phone { get; set; } = null!;
    }
}
