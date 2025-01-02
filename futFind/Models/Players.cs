using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace futFind.Models
{
    // Define a tabela "players" na base de dados associada a esta classe
    [Table("players")]
    
    // Define a chave primária composta pelos campos "user_id" e "match_id"
    [PrimaryKey(nameof(user_id), nameof(match_id))]
    public class Players
    {
        // Define a propriedade "user_id", que é a referência ao utilizador associado ao jogador
        public int user_id { get; set; }

        // Define a propriedade "match_id", que é a referência ao jogo associado ao jogador
        public int match_id { get; set; }

        // Define a relação de chave estrangeira entre "user_id" e a tabela "users"
        [ForeignKey("user_id")]
        public Users User { get; set; } = null!;  // Relacionamento com o utilizador que é o jogador

        // Define a relação de chave estrangeira entre "match_id" e a tabela "games"
        [ForeignKey("match_id")]
        public Games Game { get; set; } = null!;  // Relacionamento com o jogo em que o jogador participa
    }
}
