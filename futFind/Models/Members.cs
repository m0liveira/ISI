using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace futFind.Models
{
    // Define a tabela "clan_members" na base de dados associada a esta classe
    [Table("clan_members")]
    
    // Define a chave primária composta pelos campos "user_id" e "clan_id"
    [PrimaryKey(nameof(user_id), nameof(clan_id))]
    public class Members
    {
        // Define a propriedade "user_id", que é a referência ao utilizador associado ao membro do clã
        public int user_id { get; set; }

        // Define a propriedade "clan_id", que é a referência ao clã ao qual o membro pertence
        public int clan_id { get; set; }

        // Define a relação de chave estrangeira entre "user_id" e a tabela "users"
        [ForeignKey("user_id")]
        public Users User { get; set; } = null!;  // Relacionamento com o utilizador (membro) do clã

        // Define a relação de chave estrangeira entre "clan_id" e a tabela "teams"
        [ForeignKey("clan_id")]
        public Teams Team { get; set; } = null!;  // Relacionamento com o clã (team) ao qual o membro pertence
    }
}
