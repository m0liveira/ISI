using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace futFind.Models
{
    // Define a tabela "notifications" na base de dados associada a esta classe
    [Table("notifications")]
    public class Notifications
    {
        // Define a chave primária da tabela "notifications"
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Geração automática do valor para o campo "id"
        public int id { get; set; }

        // Define a propriedade "match_id" como obrigatória e uma chave estrangeira que faz referência à tabela "games"
        [Required]
        [ForeignKey("Games")]  // Relaciona o "match_id" com a tabela "games"
        public int match_id { get; set; }  // ID do jogo associado à notificação

        // Define a propriedade "message" como obrigatória, com comprimento máximo de 50 caracteres
        [Required]
        [MaxLength(50)]  // Limite de 50 caracteres para a mensagem
        public string message { get; set; } = null!;  // Mensagem da notificação, obrigatória

        // Define a propriedade "seen" como um campo opcional do tipo booleano, que indica se a notificação foi visualizada
        public bool seen { get; set; } = false;  // Valor padrão é "false", indicando que a notificação ainda não foi vista

        // Define a propriedade "timestamp" como obrigatória e com o valor de data e hora atual (em UTC) como valor padrão
        [Required]
        public DateTime timestamp { get; set; } = DateTime.UtcNow;  // Data e hora da criação da notificação (em UTC)
    }
}
