using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace futFind.Models
{
    // Define a tabela "users" na base de dados associada a esta classe
    [Table("users")]
    public class Users
    {
        internal int Id;  // Atributo interno, não mapeado para a base de dados

        // Define o campo "id" como chave primária da tabela
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Geração automática de valor para o ID
        public int id { get; set; }

        // Define o campo "name" como obrigatório, com comprimento máximo de 30 caracteres
        [Required]
        [MaxLength(30)]
        public string name { get; set; } = null!;  // A propriedade é obrigatória e não pode ser nula

        // Define o campo "email" como obrigatório, com comprimento máximo de 255 caracteres e validação de e-mail
        [Required]
        [MaxLength(255)]
        [EmailAddress]  // Valida o formato do e-mail
        public string email { get; set; } = null!;

        // Define o campo "password" como obrigatório, com comprimento máximo de 50 caracteres
        [Required]
        [MaxLength(50)]
        public string password { get; set; } = null!;

        // Define o campo "phone" como obrigatório, com comprimento máximo de 15 caracteres
        [Required]
        [MaxLength(15)]
        public string phone { get; set; } = null!;  // Número de telefone, obrigatório e não pode ser nulo
    }
}
