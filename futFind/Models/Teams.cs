using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace futFind.Models
{
    // Define a tabela "teams" na base de dados associada a esta classe
    [Table("teams")]
    public class Teams
    {
        // Define o campo "id" como chave primária da tabela
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Geração automática de valor para o ID
        public int id { get; set; }

        // Define o campo "leader" como obrigatório. Representa o líder da equipa
        [Required]
        public int leader { get; set; }  // ID do líder da equipa, obrigatório

        // Define o campo "name" como obrigatório, com comprimento máximo de 30 caracteres
        [Required]
        [MaxLength(30)]
        public string name { get; set; } = null!;  // Nome da equipa, obrigatório e não pode ser nulo

        // Define o campo "description" como opcional, com comprimento máximo de 50 caracteres
        [MaxLength(50)]
        public string? description { get; set; }  // Descrição da equipa, opcional

        // Define o campo "capacity" como opcional. Representa a capacidade máxima de jogadores
        public int? capacity { get; set; }  // Capacidade da equipa, opcional

        // Define o campo "invite_code" como opcional, com comprimento máximo de 20 caracteres
        [MaxLength(20)]
        public string? invite_code { get; set; }  // Código de convite para a equipa, opcional
    }
}
