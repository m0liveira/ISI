using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace futFind.Models
{
    // Define a tabela "games" na base de dados associada a esta classe
    [Table("games")]
    public class Games
    {
        // Define a chave primária "id" e a geração automática do valor na base de dados
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        // Define a propriedade "host_id" como obrigatória e chave estrangeira que faz referência à tabela "users"
        [Required]
        [ForeignKey("Users")]
        public int host_id { get; set; }  // O utilizador que está a hospedar o jogo (o organizador)

        // Define a propriedade "date" como obrigatória e representa a data e hora do jogo
        [Required]
        public DateTime date { get; set; }

        // Define a propriedade "address" que é opcional, com um comprimento máximo de 200 caracteres
        [MaxLength(200)]
        public string? address { get; set; }  // O endereço onde o jogo irá ocorrer

        // Define a propriedade "capacity" como opcional e representa a capacidade máxima de participantes
        public int? capacity { get; set; }  // Capacidade do local onde o jogo será realizado

        // Define a propriedade "price" como opcional e com tipo de dado NUMERIC com 10 dígitos, sendo 2 após a vírgula
        [Column(TypeName = "NUMERIC(10,2)")]
        public decimal? price { get; set; }  // Preço de participação no jogo

        // Define a propriedade "is_private" como um valor booleano que indica se o jogo é privado
        public bool is_private { get; set; } = false;  // O jogo é privado (true) ou público (false)

        // Define a propriedade "share_code" como opcional e com um comprimento máximo de 15 caracteres
        [MaxLength(15)]
        public string? share_code { get; set; }  // Código de partilha para o jogo (se privado)

        // Define a propriedade "status" como obrigatória e com um valor padrão "Scheduled" (agendado)
        [Required]
        [MaxLength(50)]
        public string status { get; set; } = "Scheduled";  // O estado do jogo, ex: "Scheduled", "In Progress", "Completed"
    }
}
